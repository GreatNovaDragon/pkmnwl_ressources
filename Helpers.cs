#region

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Type = pkmnWildLife.Data.Type;

#endregion

namespace pkmnWildLife;

public class Helpers
{
    public static IEnumerable<tr> Csv2Trait(string whereItIs)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        using var reader = new StreamReader(whereItIs);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<tr>().ToArray();
    }

    public static string TypeToColor(Type type)
    {
        return type.ID switch
        {
            "bug" => "#91A119",
            "dark" => "#50413f",
            "dragon" => "#5060e0",
            "electric" => "#f8bf00",
            "fire" => "#e52829",
            "fairy" => "#ed6fed",
            "fighting" => "#fd8f00",
            "flying" => "#a1bbec",
            "ghost" => "#6f416f",
            "grass" => "#3fa029",
            "ground" => "#905121",
            "ice" => "#3fd7fd",
            "normal" => "#9ea09e",
            "poison" => "#9041ca",
            "psychic" => "#ed4178",
            "rock" => "#aea880",
            "steel" => "#60a0b7",
            "water" => "#297fed",
            _ => "#000000"
        };
    }

    public class tr
    {
        [Index(0)] public string Name { get; set; }
        [Index(1)] public string effect { get; set; }
        [Index(2)] public string Requirement { get; set; }
    }
}