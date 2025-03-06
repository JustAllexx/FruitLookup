using FruityLookup.Entities;
using System.CommandLine;

namespace FruityLookup.CLI;

/// <summary>
/// The FruityLookup Command Line interface allows FruityLookup functions to be called from the command line
/// <example>
/// For information on how to use the CLI, execute this from the command line
/// <c>FruityLookup.exe -h</c>
/// </example>
/// </summary>
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

    private static void buildCommands() {
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

    /// <summary>
    /// Entry point for the Fruity Lookup CLI
    /// </summary>
    /// <param name="args">The Command Line arguments to be processed</param>
    /// <returns></returns>
    public static async Task<int> Main(string[] args) {
        

        buildCommands();
        await rootCommand.InvokeAsync(args);
        

        return 0;
    }
}
