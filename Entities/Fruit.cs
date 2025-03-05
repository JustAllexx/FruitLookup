using System.Globalization;
using System.Text.Json;
namespace FruityLookup.Entities;

//Two record classes needed for JSON Deserialisation
public record class Nutrition {
    public int calories { get; init; }
    public double fat { get; init; }
    public double sugar { get; init; }
    public double carbohydrates { get; init; }
    public double protein { get; init; }
}

public record class Fruit : IFormattable {
    public required String name { get; init; }
    public required int id { get; init; }
    public required String family { get; init; }
    public required String order { get; init; }
    public required String genus { get; init; }
    public required Nutrition nutritions { get; init; }

    public string ToUserString() {

        return $"""
            Name: {name}
            ID: {id}
            Family: {family}
            Sugar: {nutritions.sugar}g
            Carbohydrates: {nutritions.carbohydrates}g
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
        if (string.IsNullOrEmpty(format)) { format = "G"; }
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
