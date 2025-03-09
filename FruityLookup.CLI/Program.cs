using FruityLookup.Entities;
using FruityLookup.Exceptions;
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
            foreach (string fruitString in fruitList) {
                // await writeInformationAsync(output, fruitList, format);
                try {
                    Fruit? fruit = await fruity.getFruitInformationAsync(fruitString);
                    if (fruit == null) continue;
                    await writeFruitInformationAsync(fruit, output, format);
                } catch (FruitNotFound) {
                    await output.WriteLineAsync(fruitString + " not in FruityVice database");
                }
            }
        } else {
            foreach (string fruitString in fruitList) {
                try {
                    Fruit? fruit = await fruity.getFruitInformationAsync(fruitString);
                    if (fruit == null) continue;
                    await writeFruitInformationAsync(fruit, Console.Out, format);
                }
                catch (FruitNotFound) {
                    await Console.Out.WriteLineAsync(fruitString + " not in FruityVice database");
                }
            }
        }
    }

    private static async Task allCommandHandler(OutputFormat format, string outputFile) {
        List<Fruit> fruitList = await fruity.getAllFruitAsync();

        if (!string.IsNullOrEmpty(outputFile)) {
            //Get CurrentDirectory returns the Directory of where the executable is being run from
            //This allows users to do relative addressing from the command line
            // eg.  ../../output.txt
            string currentDirectory = Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(currentDirectory, outputFile);
            using TextWriter output = new StreamWriter(outputPath);

            await writeFruitInformationAsync(fruitList, output, format);
        }
        else {
            await writeFruitInformationAsync(fruitList, Console.Out, format);
        }
    }

    private static async Task familyCommandHandler(string familyName, OutputFormat format, string outputFile) {
        List<Fruit> fruitList = await fruity.getFruitsFromFamily(familyName);
        
        if (!string.IsNullOrEmpty(outputFile)) {
            string currentDirectory = Directory.GetCurrentDirectory();
            string outputPath = Path.Combine(currentDirectory, outputFile);
            using TextWriter output = new StreamWriter(outputPath);

            await writeFruitInformationAsync(fruitList, output, format);
        }
        else {
            await writeFruitInformationAsync(fruitList, Console.Out, format);
        }
    }

    private static async Task writeFruitInformationAsync(List<Fruit> fruits, TextWriter output, OutputFormat format) {
        foreach (Fruit fruit in fruits) {
            // await writeInformationAsync(output, fruitList, format);
            if (fruit != null) {
                await writeFruitInformationAsync(fruit, output, format);
            }
        }
    }

    private static async Task writeFruitInformationAsync(Fruit fruit, TextWriter output, OutputFormat format) {        
        switch (format) {
            case OutputFormat.User:
                await output.WriteLineAsync(fruit.ToString("US"));
                break;
            case OutputFormat.Json:
                await output.WriteLineAsync(fruit.ToString("JS"));
                break;
            default:
                await output.WriteLineAsync("Formatting option not recognised!");
                return;
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

        var familyCommand = new Command("family", "Gets all fruits belonging to a specific family");
        var familyArgument = new Argument<string>("family", "What family of fruits to collect");
        familyCommand.AddArgument(familyArgument);
        familyCommand.SetHandler(familyCommandHandler, familyArgument, formatOption, outputFileOption);

        var allCommand = new Command("all", "Gets all the fruits in the fruityvice database");
        allCommand.SetHandler(allCommandHandler, formatOption, outputFileOption);

        rootCommand.Add(fruitListArgument);
        rootCommand.AddGlobalOption(formatOption);
        rootCommand.AddGlobalOption(outputFileOption);

        rootCommand.AddCommand(familyCommand);
        rootCommand.AddCommand(allCommand);

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
