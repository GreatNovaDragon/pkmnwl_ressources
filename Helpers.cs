#region

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

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

    public class tr
    {
        [Index(0)] public string Name { get; set; }
        [Index(1)] public string effect { get; set; }
        [Index(2)] public string Requirement { get; set; }
    }
}