using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
        String json = await client.GetStringAsync(url);
        Console.Write(json);
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
