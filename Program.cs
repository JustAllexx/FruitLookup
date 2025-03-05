using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.CommandLine;
using static System.Net.WebRequestMethods;

//Two record classes needed for JSON Deserialisation
public record class Nutrition {
    public int? calories { get; init; }
    public double fat { get; init; }
    public double sugar { get; init; }
    public double carbohydrates {get; init; }
    public double protein { get; init; }
}

public record class Fruit {
    public String? name { get; init; }
    public int? id { get; init; }
    public String? family { get; init; }
    public String? order { get; init; }
    public String? genus { get; init; }
    public Nutrition? nutritions { get; init; }

    public override string ToString() {

        return $"""
            Name: {name}
            ID: {id}
            Family: {family}
            Sugar: {nutritions.sugar}
            Carbohydrates: {nutritions.carbohydrates}
            """;
    }
}

class FruityLookup {
    HttpClient client = new();
    String httpsPath = "https://fruityvice.com/api/fruit/";

    public FruityLookup() {
        initialiseClient();
    }

    public async Task<Fruit?> getFruitInformationAsync(String fruitName) {
        String url = getFruitUrl(fruitName);
        Stream json = await client.GetStreamAsync(url);
        Fruit? fruit = await JsonSerializer.DeserializeAsync<Fruit>(json);
        if(fruit == null) {
            Console.WriteLine("Deserialising went wrong");
            return null;
        }

        return fruit;
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

    public static async Task<int> Main(String[] args) {
        FruityLookup fruityLookup = new();
        Fruit? fruit = await fruityLookup.getFruitInformationAsync("apple");
        Console.WriteLine(fruit);

        return 0;
    }
}
