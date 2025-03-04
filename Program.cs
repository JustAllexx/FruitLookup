using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

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
}

class FruityLookup {
    HttpClient client = new();

    public FruityLookup() {
        initialiseClient();
    }

    private void initialiseClient() {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }

    public async Task MakeRequestAsync(String url) {
        Stream json = await client.GetStreamAsync(url);
        Fruit? fruit = await JsonSerializer.DeserializeAsync<Fruit>(json);
        if(fruit == null) {
            Console.WriteLine("Deserialising went wrong");
            return;
        }

        Console.Write(fruit);
    }
}

class FruityLookupCLI {

    public static async Task<int> Main(String[] args) {
        FruityLookup fruityLookup = new();
        String url = "https://fruityvice.com/api/fruit/apple";
        await fruityLookup.MakeRequestAsync(url);

        return 0;
    }
}
