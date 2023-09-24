using pkmnWildLife.Data;
using PokeApiNet;
using Ability = PokeApiNet.Ability;
using Item = PokeApiNet.Item;
using Move = PokeApiNet.Move;
using Type = PokeApiNet.Type;

namespace pkmnWildLife;

public class DBInitializer
{
    public static async Task InitializeDB(ApplicationDbContext context)
    {
        var apiclient = new PokeApiClient();

        await TransferTypes(context, apiclient);

        await TransferMoveClasses(context, apiclient);

        await TransferItems(context, apiclient);

        await TransferAbilities(context, apiclient);

        await TransferMoves(context, apiclient);
    }


    private static async Task TransferTypes(ApplicationDbContext context, PokeApiClient apiclient)
    {
        var types = await apiclient.GetNamedResourcePageAsync<Type>(9999, 0);

        foreach (var TypeR in types.Results)
        {
            var Type = await apiclient.GetResourceAsync(TypeR);
            context.Types.Add(new Data.Type
            {
                ID = Type.Id.ToString(),
                Name = Type.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                Name_DE = Type.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
            });
        }

        context.SaveChangesAsync();
    }

    private static async Task TransferMoveClasses(ApplicationDbContext context, PokeApiClient apiclient)
    {
        var moveClass = await apiclient.GetNamedResourcePageAsync<MoveDamageClass>(10, 0);

        foreach (var TypeR in moveClass.Results)
        {
            var Type = await apiclient.GetResourceAsync(TypeR);
            context.MoveClass.Add(new MoveClass
            {
                ID = Type.Id.ToString(),
                Name = Type.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                Name_DE = Type.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
            });
        }

        context.SaveChangesAsync();
    }

    private static async Task TransferItems(ApplicationDbContext context, PokeApiClient apiclient)
    {
        var items = await apiclient.GetNamedResourcePageAsync<Item>(9999, 0);
        Console.Write(items.Results.Count);
        foreach (var i in items.Results)
        {
            var Item = await apiclient.GetResourceAsync(i);


            var ID = Item.Id.ToString();
            var Name = Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                : Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                    : "No Data";
            Console.WriteLine($"{ID} {Name} {Name_DE} {Effect}");
            context.Items.Add(new Data.Item
            {
                ID = ID,
                Name = Name,
                Name_DE = Name_DE,
                Effect = Effect
            });
        }

        context.SaveChangesAsync();
    }

    private static async Task TransferAbilities(ApplicationDbContext context, PokeApiClient apiclient)
    {
        var abilities = await apiclient.GetNamedResourcePageAsync<Ability>(9999, 0);

        foreach (var i in abilities.Results)
        {
            var Item = await apiclient.GetResourceAsync(i);


            var ID = Item.Id.ToString();
            var Name = Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                : Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                    : Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en").FlavorText
                        : "No Entry";

            context.Abilities.Add(new Data.Ability
            {
                ID = ID,
                Name = Name,
                Name_DE = Name_DE,
                Effect = Effect
            });
        }

        context.SaveChangesAsync();
    }

    private static async Task TransferMoves(ApplicationDbContext context, PokeApiClient apiclient)
    {
        var moves = await apiclient.GetNamedResourcePageAsync<Move>(999, 0);

        foreach (var i in moves.Results)
        {
            var m = await apiclient.GetResourceAsync(i);

            var ID = m.Id.ToString();
            var Name = m.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? m.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : m.Name;

            var Name_DE = m.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = (m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                : m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                    : m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en").FlavorText
                        : "No Entry").Replace("$effect_chance%", $"{m.EffectChance}");

            var Target = m.Target.Name;

            var DamageDice = StrengthToDice(m.Power, 0.75);
            var MType = context.Types.FirstOrDefault(e =>
                e.ID == apiclient.GetResourceAsync(m.Type).Result.Id.ToString());
            var DamageClass =
                context.MoveClass.FirstOrDefault(e =>
                    e.ID == apiclient.GetResourceAsync(m.DamageClass).Result.Id.ToString());
            context.Moves.Add(new Data.Move
            {
                ID = ID,
                Name = Name,
                Name_DE = Name_DE,
                Effect = Effect,
                type = MType,
                Target = Target,
                DamageDice = DamageDice,
                MoveClass = DamageClass
            });
        }

        context.SaveChangesAsync();
    }


    public static string? StrengthToDice(int? strength, double nerf)
    {
        if (!strength.HasValue) return null;

        var calc = Convert.ToInt32(Math.Floor(strength.Value * nerf / 10));

        switch (calc)
        {
            case <= 3:
                return "1d4";
            case 4:
                return "1d6";
            case 5:
                return "2d8";
            case 6:
                return "1d10";
            case 7:
                return "2d6";
            case 8:
                return "3d4";
            case 9:
                return "2d8";
            case 10:
                return "4d4";
            case 11:
                return "2d10";
            case 12:
                return "2d10";
            case 13:
                return "2d12";
            case 14:
                return "4d6";
            case 15:
                return "4d6";
            case 16:
                return "4d6";
            case 17:
                return "4d6";
            case 18:
                return "4d8";
            case 19:
                return "4d8";
            case 20:
                return "4d8";
            case 21:
                return "2d20";
            case 22:
                return "4d10";
            case 23:
                return "4d10";
            case 24:
                return "4d10";
            case 25:
                return "4d10";
            default:
                return "1d2";
        }
    }
}