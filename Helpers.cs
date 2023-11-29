#region

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

#endregion

namespace pkmnWildLife;

public class Helpers
{
    public static tr[] csv2trait(string whereItIs)
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