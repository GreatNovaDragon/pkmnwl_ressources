using pkmnWildLife.Data;
using PokeApiNet;
using Ability = PokeApiNet.Ability;
using Item = PokeApiNet.Item;
using Type = PokeApiNet.Type;

namespace pkmnWildLife;

public class DBInitializer
{
    public static async Task InitializeDB(ApplicationDbContext context)
    {
        
        if (context.DamageDice.Any()) return;
        var dice = new List<string>
        {
            "1d4", "1d6", "1d6", "1d8", "1d10", "1d12", "1d20",
            "2d4", "2d6", "2d6", "2d8", "2d10", "2d12", "2d20",
            "3d4", "3d6", "3d6", "3d8", "3d10", "3d12", "3d20",
            "4d4", "4d6", "4d6", "4d8", "4d10", "4d12", "4d20"
        };

        foreach (var d in dice)
            context.DamageDice.Add(new DamageDice
            {
                ID = Guid.NewGuid().ToString(),
                Effect = "THIS IS AN NORMAL DICE; THERE ARE NO EFFECTS DIPSHIT",
                Name = d
            });

        context.SaveChangesAsync();

        var apiclient = new PokeApiClient();

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

            context.SaveChangesAsync();
        }

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


            context.SaveChangesAsync();
        }
    }


    public static string StrengthToDice(int strength, double nerf)
    {
        var calc = Convert.ToInt32(Math.Floor(strength * nerf / 10));

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