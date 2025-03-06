using System.Net.Http.Headers;
using System.Text.Json;
using System.CommandLine;

using FruityLookup.Entities;
using System.Runtime.CompilerServices;

namespace FruityLookup;

public enum OutputFormat {
    User,
    Json
}

public class FruityLookup {
    readonly HttpClient client = new();
    readonly string httpsPath = "https://fruityvice.com/api/fruit/";

    public FruityLookup() {
        initialiseClient();
    }

    public async Task<Fruit?> getFruitInformationAsync(string fruitName) {
        try {
            string url = getFruitUrl(fruitName);
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

    private string getFruitUrl(string fruit) {
        return httpsPath + fruit;
    }
}

public class FruityLookupCLI {

    private readonly static RootCommand rootCommand = new RootCommand("CLI for accessing fruit information from FruityVice");
    private readonly static FruityLookup fruity = new();

    private static async Task rootCommandHandler(List<string> fruitList, OutputFormat format, string outputFile) {
        //TextWriter is the most common subclass of Console.Out and StreamWriter
        //Allows us to reduce duplicated code
        if (!string.IsNullOrEmpty(outputFile)) {
            string currentDirectory = Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(currentDirectory, outputFile);
            using TextWriter output = new StreamWriter(outputPath);
            await writeInformationAsync(output, fruitList, format);
        } else {
            await writeInformationAsync(Console.Out, fruitList, format);
        }
    }

    //Takes in a Writer 
    private static async Task writeInformationAsync(TextWriter output, List<string> fruitList, OutputFormat format) {
        foreach (string fruit in fruitList) {
            Fruit? FruitInfo = await fruity.getFruitInformationAsync(fruit);
            switch (format) {
                case OutputFormat.User:
                    await output.WriteLineAsync(FruitInfo?.ToString("US"));
                    break;
                case OutputFormat.Json:
                    await output.WriteLineAsync(FruitInfo?.ToString("JS"));
                    break;
                default:
                    await output.WriteLineAsync("Formatting option not recognised!");
                    return;
            }
        }
    }

    public static void buildCommands() {
        // Creates argument builders for the root command
        var fruitListArgument = new Argument<List<string>>(
            name: "Fruit List",
            description: "List of fruit names to lookup on FruityVice"
            );
        

        var formatOption = new Option<OutputFormat>(
            name: "--format",
            description: "What format to output the fruits with",
            getDefaultValue: () => OutputFormat.User
        );
        formatOption.AddAlias("-f");

        var outputFileOption = new Option<string>(
            name: "--output",
            description: "Where to save output on disk",
            getDefaultValue: () => ""
        );
        outputFileOption.AddAlias("-o");

        rootCommand.Add(fruitListArgument);
        rootCommand.AddGlobalOption(formatOption);
        rootCommand.AddOption(outputFileOption);
        rootCommand.SetHandler(rootCommandHandler,
            fruitListArgument, formatOption, outputFileOption);
        
    }

    public static async Task<int> Main(string[] args) {
        buildCommands();
        await rootCommand.InvokeAsync(args);

        return 0;
    }
}
