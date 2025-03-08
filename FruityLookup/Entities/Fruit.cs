using System.Globalization;
using System.Text.Json;
namespace FruityLookup.Entities;

//Two record classes needed for JSON Deserialisation
/// <summary>
/// The nutrition information for fruits, used by the <c>Fruit</c> class
/// </summary>
public record class Nutrition {
    public int calories { get; init; }
    public double fat { get; init; }
    public double sugar { get; init; }
    public double carbohydrates { get; init; }
    public double protein { get; init; }
}

/// <summary>
/// The Fruit record stores information about each fruit, such as its name, genus and nutritional information
/// </summary>
// IFormattable means it can be used in String formatting $"{fruit:JS}" for JSON
public record class Fruit : IFormattable {
    public required string name { get; init; }
    public required int id { get; init; }
    public required string family { get; init; }
    public required string order { get; init; }
    public required string genus { get; init; }
    public required Nutrition nutritions { get; init; }

    /// <summary>
    /// Public function to access the Fruits information in a human readable way
    /// </summary>
    /// <returns>String of information</returns>
    public string ToUserString() {

        return $"""
            Name: {name}
            ID: {id}
            Family: {family}
            Sugar: {nutritions.sugar}g
            Carbohydrates: {nutritions.carbohydrates}g

            """;
    }

    /// <summary>
    /// Convert Fruit Information to JSON String
    /// </summary>
    /// <returns>JSON String</returns>
    public string ToJsonString() {
        return JsonSerializer.Serialize(this);
    }

    // IFormattable Fruit method overrides

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
