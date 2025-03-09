using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;

using FruityLookup.Entities;
using FruityLookup.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security;
using Microsoft.Extensions.Configuration;

namespace FruityLookup;

/// <summary>
/// 
/// </summary>
public enum OutputFormat {
    /// <summary>
    /// Tells FruityLookup to output with a Human Readable Format Containing only Name, Id, Family, Sugar and Carbohydrates
    /// </summary>
    User,
    /// <summary>
    /// Tells FruityLookup to output using the Machine-Readable Json Format Containing all information
    /// </summary>
    Json
}

/// <summary>
/// The <c>FruityLookup</c> class is an interface for interacting with the Fruityvice API inside C#
/// <example>
/// To instantiate Fruitylookup
/// <code>
/// Fruitylookup fruityLookup = new FruityLookup();
/// </code>
/// </example>
/// </summary>
public class FruityLookup {
    readonly HttpClient client = new();
    readonly IMemoryCache cache;
    readonly ILogger<FruityLookup> logger;
    //Only log information when debugging
#if DEBUG
    LogLevel logLevel = LogLevel.Information;
#else
    LogLevel logLevel = LogLevel.Warning;
#endif


    readonly string httpsPath = "https://fruityvice.com/api/fruit/";

    /// <summary>
    /// FruityLookup constructor instantiates the HTTP client to make requests to FruityVice
    /// </summary>
    public FruityLookup() {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = false;
                options.SingleLine = true;
            });
            builder.SetMinimumLevel(logLevel);
        });
        logger = loggerFactory.CreateLogger<FruityLookup>();
        logger.LogInformation("Created FruityLookup Instance");
        cache = new MemoryCache(new MemoryCacheOptions());
        initialiseClient();
    }

    /// <summary>
    /// Requests fruit information from the Fruityvice API and returns the caller an Instantiated Fruit Object
    /// </summary>
    /// <param name="fruitName">Name of fruit to query information of</param>
    /// <returns>Fruit or null</returns>
    public async Task<Fruit?> getFruitInformationAsync(string fruitName) {
        logger.LogInformation("getting `{fruit}` information", fruitName);
        //Check if the fruit isn't already in cache
        if (cache.TryGetValue(fruitName, out Fruit? fruit)) {
            logger.LogInformation("Fetching `{fruitName}` information from cache", fruitName);
            //This Should be impossible
            if (fruit == null) throw new InvalidOperationException("Cached a null fruit inside getFruitInformationAsync");
            return fruit;
        }

        //Fruityvice accepts keywords like sugar and family to allow you to do queries, this breaks the JSON deserialiser as it returns a list
        if (checkQueryKeyword(fruitName)) throw new FruitNotFound();

        string url = getFruitUrl(fruitName);
        Stream json;
        try {
            logger.LogInformation("Fetching JSON information from: {url}", url);
            json = await client.GetStreamAsync(url);
        }
        catch (HttpRequestException ex) {
            logger.LogWarning("Fetching JSON Information resulting in the following error: {message}", ex.Message);

            switch (ex.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    //However users are expected to handle FruitNotFound exceptions
                    throw new FruitNotFound(); //Fruity Vice sends a 404 when an Fruit is Requested that is not in the database
                case HttpStatusCode.InternalServerError:
                    //Ideally I don't want an application using FruityVice to crash because of an Internal Server Error
                    //So null values will be possible and it is up to the Software using the library to check for this
                    logger.LogError("FruityVice API Suffered an Internal Server Error");
                    return null;
                default:
                    logger.LogError("FruityVice API Failed for some unknown reason");
                    return null;
                       
            };
            
        }
        fruit = await JsonSerializer.DeserializeAsync<Fruit>(json);

        if (fruit == null) throw new FruitNotFound();
        logger.LogInformation("Added `{fruit}` to cache", fruitName);
        cache.Set<Fruit>(fruitName, fruit);

        return fruit;
    }
    
    /// <summary>
    /// Returns all the fruits in the FruityVice database
    /// </summary>
    /// <returns>All Fruits as a List of Fruits</returns>
    public async Task<List<Fruit>> getAllFruitAsync() {
        logger.LogInformation("Getting All Fruits from database");
        string url = getAllFruitUrl();
        Stream json = await client.GetStreamAsync(url);
        List<Fruit>? fruits = await JsonSerializer.DeserializeAsync<List<Fruit>>(json);
        if (fruits == null) return []; //Return empty list

        return fruits;
    }

    /// <summary>
    /// Returns all the fruits from a certain family
    /// </summary>
    /// <param name="family">Which fruit family to query</param>
    /// <returns>All fruits of the given family as a list</returns>
    public async Task<List<Fruit>> getFruitsFromFamily(string family) {
        logger.LogInformation($"Accessing all the fruits in the `{family}` family");
        string url = getFruitsFromFamilyUrl(family);
        Stream json;
        try {
            json = await client.GetStreamAsync(url);
        } catch (HttpRequestException err) {
            switch (err.StatusCode) {
                case HttpStatusCode.NotFound:
                    logger.LogWarning("The {family} family could not be found on the fruityVice database", family);
                    return []; //return empty list
                case HttpStatusCode.InternalServerError:
                    logger.LogError("FruityVice is not available because of an internal server error");
                    return []; //Shouldn't fail program return empty list
                default:
                    logger.LogError("Something went wrong requesting fruits of family `{family}`", family);
                    return [];
            }
        }
        List<Fruit>? fruits = await JsonSerializer.DeserializeAsync<List<Fruit>>(json);
        if (fruits == null) return [];

        return fruits;
    }

    //The API also accepts keywords such as all, family, nutrition
    private static bool checkQueryKeyword(string keyword) {
        return keyword.ToLower() switch
        {   //All the listed accepted fields for fruityvice
            "all" or "family" or "genus" or "order" or "nutritions" or "carbohydrates" or "protein" or "fat" or "calories" or "sugar" => true,
            _ => false,
        };
    }

    private void initialiseClient() {
        logger.LogInformation("Creating Http Client for Fruity Vice");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        
    }

    private string getFruitUrl(string fruit) {
        return httpsPath + fruit;
    }

    private string getAllFruitUrl() {
        return httpsPath + "all";
    }

    private string getFruitsFromFamilyUrl(string family) {
        return httpsPath + "family/" + family;
    }
}
