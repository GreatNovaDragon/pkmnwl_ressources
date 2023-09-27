using Microsoft.IdentityModel.Tokens;
using pkmnWildLife.Data;
using Westwind.AspNetCore.Markdown;

namespace pkmnWildLife;

public class Helpers
{
    public static string MoveSetRenderer(Learnsets move)
    {
        var ret = @$" <div class=""move"" style=""flex-basis: 30%"">
            <dl>
            
            <h3><a href=""/moves/Details?id={move.move.ID}""> {move.move.Name}/{move.move.Name_DE} </a>
            </h3>
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
        else
            ret += @"
            <dt>&nbsp</dt>
            <dd>&nbsp</dd>
            ";

        ret += @"<h4>Effect</h4>
            ";
        ret += Markdown.Parse(move.move.Effect);
        ret += @"</div>";

        return ret;
    }

    public static string MoveRenderer(Move move, bool MoveDetailPage = false)
    {
        var ret = @" <div class=""move"" style=""flex-basis: 30%"">
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
}