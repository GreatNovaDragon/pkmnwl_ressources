using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.IdentityModel.Tokens;
using pkmnWildLife.Data;
using Westwind.AspNetCore.Markdown;

namespace pkmnWildLife;

public class Helpers
{
    public static string MoveSetRenderer(Learnsets move)
    {
        var ret = @$" 
<div class=""w3-container  w3-cell w3-mobile"" >
<div class=""w3-card"">
      
            <header class=""w3-container w3-blue"">
            <h3><a href=""/moves/Details?id={move.move.ID}""> {move.move.Name}/{move.move.Name_DE} </a>
            </h3>
            </header

<div class =""w3-container"">
            <dl>
                        <dt>
            Learned via 
            </dt>
            <dd>{move.how}</dd>
            <dt>Learned at Level</dt>
            <dd>{move.level}</dd>
            <dt>Move Class</dt>
            <dd>
           {move.move.MoveClass.Name}/{move.move.MoveClass.Name_DE}
            </dd>
<dt>Move Type</dt>
<dd>{move.move.type.Name}/{move.move.type.Name_DE}</dd>
";

        if (!move.move.DamageDice.IsNullOrEmpty())
            ret += @$"
            <dt>Damage Dice</dt>
            <dd>{move.move.DamageDice}</dd>
            ";


        ret += @"<div class=""w3-container w3-blue""><h4>Effect</h4></div>
<div class=""w3-container"">
            ";
        ret += Markdown.Parse(move.move.Effect);
        ret += @"
</div></div></div>";

        return ret;
    }

    public static string MoveRenderer(Move move, bool MoveDetailPage = false)
    {
        var ret = @" <div class=""w3-container"" style=""flex-basis: 30%"">
            <dl>";

        if (!MoveDetailPage)
            ret += @$"<h3><a href=""/moves/Details?id={move.ID}""> {move.Name}/{move.Name_DE} </a>
            </h3>";
        else
            ret += @$"<h1> {move.Name}/{move.Name_DE} </a>
            </h1>";
        ret += @$"               <dt>MoveClass</dt>
        
            <dd>
           ""{move.MoveClass.Name}/{move.MoveClass.Name_DE}""
            </dd>
<dt>Move Type</dt>
<dd>{move.type.Name}/{move.type.Name_DE}</dd>";

        if (!move.DamageDice.IsNullOrEmpty())
            ret += @$"
            <dt>Damage Dice</dt>
            <dd>{move.DamageDice}</dd>
            ";
        else
            ret += @"
            <dt>&nbsp</dt>
            <dd>&nbsp</dd>
            ";

        ret += @"<h4>Effect</h4>
            ";
        ret += Markdown.Parse(move.Effect);
        ret += @"</div>";

        return ret;
    }


    public static mv[] csv2moves(string whereItIs)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        using var reader = new StreamReader(whereItIs);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<mv>().ToArray();
    }

    public static ab[] csv2ab(string whereItIs)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        using var reader = new StreamReader(whereItIs);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<ab>().ToArray();
    }


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

    public class mv
    {
        [Index(0)] public string move { get; set; }
        [Index(1)] public string how_often { get; set; }
        [Index(2)] public string effect { get; set; }
    }

    public class ab
    {
        [Index(0)] public string ability { get; set; }
        [Index(1)] public string effect { get; set; }
    }

    public class tr
    {
        [Index(0)] public string Name { get; set; }
        [Index(1)] public string effect { get; set; }
        [Index(2)] public string Requirement { get; set; }
    }
}