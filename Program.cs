using System.Net.Http.Headers;
using System.Text.Json;
using System.CommandLine;

using FruityLookup.Entities;

namespace FruityLookup;

public enum OutputFormat {
    User,
    Json
}

public class FruityLookup {
    readonly HttpClient client = new();
    readonly String httpsPath = "https://fruityvice.com/api/fruit/";

    public FruityLookup() {
        initialiseClient();
    }

    public async Task<Fruit?> getFruitInformationAsync(String fruitName) {
        try {
            String url = getFruitUrl(fruitName);
            Stream json = await client.GetStreamAsync(url);
            Fruit? fruit = await JsonSerializer.DeserializeAsync<Fruit>(json);
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

    private String getFruitUrl(String fruit) {
        return httpsPath + fruit;
    }
}

public class FruityLookupCLI {

    public static RootCommand rootCommand = new RootCommand("CLI for accessing fruit information from FruityVice");
    public static FruityLookup fruity = new();

    public static void buildCommands() {
        var fruitListArgument = new Argument<List<String>>(
            name: "Fruit List",
            description: "List of fruit names to lookup on FruityVice"
            );

        var formatOption = new Option<OutputFormat>(
            name: "--format",
            description: "What format to output the fruits with",
            getDefaultValue: () => OutputFormat.User
        );

        rootCommand.Add(fruitListArgument);
        rootCommand.AddGlobalOption(formatOption);
        rootCommand.SetHandler(async (fruitList, format) =>
        {
            foreach (String fruit in fruitList) {
                Fruit? FruitInfo = await fruity.getFruitInformationAsync(fruit);
                switch(format) {
                    case OutputFormat.User:
                        //? mark just returns null if getFruitInformation can't access the fruit
                        Console.WriteLine(FruitInfo?.ToString("US"));
                        break;
                    case OutputFormat.Json:
                        Console.WriteLine(FruitInfo?.ToString("JS"));
                        break;
                    default:
                        Console.WriteLine("Formatting option not recognised!");
                        return;
                }
            }
        }, fruitListArgument, formatOption);
        
    }

    public static async Task<int> Main(string[] args) {
        buildCommands();
        await rootCommand.InvokeAsync(args);

        return 0;
    }
}
