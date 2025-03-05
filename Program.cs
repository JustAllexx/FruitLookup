using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.CommandLine;
using static System.Net.WebRequestMethods;
using System.Globalization;

//Two record classes needed for JSON Deserialisation
public record class Nutrition {
    public int? calories { get; init; }
    public double fat { get; init; }
    public double sugar { get; init; }
    public double carbohydrates {get; init; }
    public double protein { get; init; }
}

public record class Fruit : IFormattable {
    public String? name { get; init; }
    public int? id { get; init; }
    public String? family { get; init; }
    public String? order { get; init; }
    public String? genus { get; init; }
    public Nutrition? nutritions { get; init; }

    public string ToUserString() {

        return $"""
            Name: {name}
            ID: {id}
            Family: {family}
            Sugar: {nutritions.sugar}
            Carbohydrates: {nutritions.carbohydrates}
            """;
    }

    public string ToJsonString() {
        return JsonSerializer.Serialize(this);
    }

    // IFormattable Fruit methods

    public override string ToString() {
        return this.ToString("US", CultureInfo.CurrentCulture);
    }

    public string ToString(string format) {
        return this.ToString(format, CultureInfo.CurrentCulture);
    }

    public string ToString(string? format, IFormatProvider? provider) {
        if (String.IsNullOrEmpty(format)) { format = "G"; }
        switch (format.ToUpper()) {
            case "G":
            case "US":
                return this.ToUserString();
            case "JS":
                return this.ToJsonString();
            default:
                throw new FormatException($"{format} is not supported inside Fruit");
        }

    }
}

class FruityLookup {
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

class FruityLookupCLI {

    public static RootCommand rootCommand = new RootCommand("CLI for accessing fruit information from FruityVice");
    public static FruityLookup fruity = new();

    public static void buildCommands() {
        var fruitListArgument = new Argument<List<String>>(
            name: "FruitList",
            description: "List of fruit names to lookup on FruityVice"
            );

        rootCommand.Add(fruitListArgument);
        rootCommand.SetHandler(async (fruitList) =>
        {
            foreach(String fruit in fruitList) {
                Fruit? FruitInfo = await fruity.getFruitInformationAsync(fruit);
                //? mark just returns null if getFruitInformation can't access the fruit
                Console.WriteLine(FruitInfo?.ToString("JS"));
            }
        }, fruitListArgument);
        
    }

    public static async Task<int> Main(String[] args) {
        // Debugging
        FruityLookup fruityLookup = new();
        Fruit? fruit = await fruityLookup.getFruitInformationAsync("apple");
        Console.WriteLine(fruit);
        // ---------------------------------------------------------------------

        buildCommands();
        await rootCommand.InvokeAsync(args);

        return 0;
    }
}
