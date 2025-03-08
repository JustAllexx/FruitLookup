﻿using System.Net.Http.Headers;
using System.Text.Json;
using System.CommandLine;

using FruityLookup.Entities;
using Microsoft.Extensions.Caching.Memory;

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
    readonly string httpsPath = "https://fruityvice.com/api/fruit/";

    /// <summary>
    /// FruityLookup constructor instantiates the HTTP client to make requests to FruityVice
    /// </summary>
    public FruityLookup() {
        cache = new MemoryCache(new MemoryCacheOptions());
        initialiseClient();
    }

    /// <summary>
    /// Requests fruit information from the Fruityvice API and returns the caller an Instantiated Fruit Object
    /// </summary>
    /// <param name="fruitName">Name of fruit to query information of</param>
    /// <returns>Fruit or null</returns>
    public async Task<Fruit?> getFruitInformationAsync(string fruitName) {
        try {

            if (cache.TryGetValue(fruitName, out Fruit? fruit)) {
                return fruit;
            }


            string url = getFruitUrl(fruitName);
            Stream json = await client.GetStreamAsync(url);
            fruit = await JsonSerializer.DeserializeAsync<Fruit>(json);

            if (fruit == null) return null;
            cache.Set<Fruit>(fruitName, fruit);

            return fruit;
        }
        catch (HttpRequestException ex) {
            Console.WriteLine($"Request failed with error code: {ex.StatusCode}");
        }

        return null;
    }

    private void initialiseClient() {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        
    }

    private string getFruitUrl(string fruit) {
        return httpsPath + fruit;
    }
}
