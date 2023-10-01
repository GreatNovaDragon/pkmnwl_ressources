using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.IdentityModel.Tokens;
using pkmnWildLife.Data;
using PokeApiNet;
using Ability = PokeApiNet.Ability;
using Item = PokeApiNet.Item;
using Move = PokeApiNet.Move;
using Pokemon = PokeApiNet.Pokemon;
using Type = PokeApiNet.Type;

namespace pkmnWildLife;

public class DBInitializer
{
    public static async Task InitializeDB(ApplicationDbContext context)
    {
        var apiclient = new PokeApiClient();
        await TransferTypes(context, apiclient);
        await TransferMoveClasses(context, apiclient);
        await TransferMoves(context, apiclient);
        await TransferTraits(context);
        await TransferAbilities(context, apiclient);
        await TransferMon(context, apiclient);
        await TransferItems(context, apiclient);
        await TransferLearnsets(context, apiclient);
    }


    private static async Task TransferTypes(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.Types.Any())
        {
            Console.WriteLine("There already are Types");
            return;
        }

        var types = await apiclient.GetNamedResourcePageAsync<Type>(9999, 0);
        var tps = new List<Data.Type>();
        foreach (var TypeR in types.Results)
        {
            var Type = await apiclient.GetResourceAsync(TypeR);
            tps.Add(new Data.Type
            {
                ID = Type.Name,
                Name = Type.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                Name_DE = Type.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
            });
        }

        context.AddRange(tps);
        await context.SaveChangesAsync();
    }

    private static async Task TransferMoveClasses(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.MoveClasses.Any())
        {
            Console.WriteLine("There already are MoveClasses");
            return;
        }

        var moveClass = await apiclient.GetNamedResourcePageAsync<MoveDamageClass>(10, 0);
        var movecls = new List<MoveClass>();

        foreach (var TypeR in moveClass.Results)
        {
            var Type = await apiclient.GetResourceAsync(TypeR);
            movecls.Add(new MoveClass
            {
                ID = Type.Name,
                Name = Type.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                Name_DE = Type.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
            });
        }

        context.AddRange(movecls);
        await context.SaveChangesAsync();
    }

    private static async Task TransferItems(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.Items.Any())
        {
            Console.WriteLine("There already are Items");
            return;
        }


        var items = await apiclient.GetNamedResourcePageAsync<Item>(9999, 0);
        var itms = new List<Data.Item>();
        foreach (var i in items.Results)
        {
            var Item = await apiclient.GetResourceAsync(i);

            string[] undesirables =
            {
                "dynamax-crystals", "sandwhich-ingredients", "tm-materials", "picnic", "species-candies",
                "all-machines", "all-mail", "plot-advancement"
            };

            if (undesirables.Contains(Item.Category.Name))
                continue;

            var ID = $"{Item.Name}_{Item.Id}";

            var Name = Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                : Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                    : "No Data";
            itms.Add(new Data.Item
            {
                ID = ID,
                Name = Name,
                Name_DE = Name_DE,
                Effect = Effect
            });
        }

        context.AddRange(itms);
        await context.SaveChangesAsync();
    }

    private static async Task TransferAbilities(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.AbilityDex.Where(m => !m.IsTrait).Any())
        {
            Console.WriteLine("There already are Abilities");
            return;
        }

        var abilities = await apiclient.GetNamedResourcePageAsync<Ability>(9999, 0);
        var abs = new List<Data.Ability>();
        foreach (var i in abilities.Results)
        {
            var Item = await apiclient.GetResourceAsync(i);


            var ID = Item.Name;
            var Name = Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                : Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").ShortEffect
                    : Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en").FlavorText
                        : "No Entry";


            var moves_old = Helpers.csv2ab("abilities_old.csv");


            if (moves_old.Where(mn => mn.ability == Name).Any())
            {
                var mn = moves_old.Where(m => m.ability == Name).FirstOrDefault();
                Effect = mn.effect;
            }


            abs.Add(new Data.Ability
            {
                ID = ID,
                Name = Name,
                Name_DE = Name_DE,
                Effect = Effect,
                IsTrait = false
            });

            Console.WriteLine(Name);
        }

        context.AddRange(abs);
        await context.SaveChangesAsync();
    }

    private static async Task TransferTraits(ApplicationDbContext context)
    {
        if (context.AbilityDex.Where(m => m.IsTrait).Any())
        {
            Console.WriteLine("There already are Traits");
            return;
        }

        var traits = Helpers.csv2trait("traits.csv");
        var trs = new List<Data.Ability>();
        foreach (var tr in traits)
            trs.Add(new Data.Ability
            {
                ID = tr.Name.ToLower().Normalize() + "_trait",
                Effect = tr.effect,
                Name = tr.Name,
                Requirement = tr.Requirement,
                IsTrait = true
            });
        context.AddRange(trs);
        await context.SaveChangesAsync();
    }

    private static async Task TransferMoves(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.MoveDex.Any())
        {
            Console.WriteLine("There already are Moves");
            return;
        }

        var types = context.Types.ToList();
        var moves = await apiclient.GetNamedResourcePageAsync<Move>(999, 0);
        var mvs = new List<Data.Move>();

        var dclass = context.MoveClasses.ToList();
        foreach (var i in moves.Results)
        {
            var m = await apiclient.GetResourceAsync(i);

            var ID = m.Name;
            var Name = m.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? m.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : m.Name;

            var Name_DE = m.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = (m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").ShortEffect
                : m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").ShortEffect
                    : m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en").FlavorText
                        : "No Entry").Replace("$effect_chance%", $"{m.EffectChance}");

            var Target = m.Target.Name;

            var DamageDice = StrengthToDice(m.Power, 0.75);
            var MType = types.FirstOrDefault(e =>
                e.ID == m.Type.Name);
            var DamageClass =
                dclass.FirstOrDefault(e =>
                    e.ID == m.DamageClass.Name);

            var moves_old = Helpers.csv2moves("moves_old.csv");


            if (moves_old.Where(mn => mn.move == Name).Any())
            {
                var mn = moves_old.Where(m => m.move == Name).FirstOrDefault();
                Effect = mn.effect;
            }

            mvs.Add(new Data.Move
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

            Console.WriteLine(Name + "move");
        }


        context.AddRange(mvs);
        await context.SaveChangesAsync();
    }

    private static async Task TransferMon(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.Pokedex.Any())
        {
            Console.WriteLine("There already are Pokemon in the Index");
            return;
        }

        var abilities = context.AbilityDex.ToArray();
        var types = context.Types.ToArray();
        var pkmn = await apiclient.GetNamedResourcePageAsync<Pokemon>(1500, 0);
        var pokes = new List<Data.Pokemon>();
        foreach (var p in pkmn.Results)
        {
            var poke = await apiclient.GetResourceAsync(p);
            var species = apiclient.GetResourceAsync(poke.Species).Result;


            var ID = poke.Name;
            var Order = species.Order;
            var Name = species.Names
                .FirstOrDefault(n => n.Language.Name == "en").Name;

            var image = poke.Sprites.Other.OfficialArtwork.FrontDefault;

            var Name_DE = apiclient.GetResourceAsync(poke.Species).Result.Names
                .FirstOrDefault(n => n.Language.Name == "de")?.Name;

            var form = "";
            if (!Name.ToLower().Equals(ID.Replace("-", " ")))
                form = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ID.Replace($"{Name.ToLower()}-", "")
                    .Replace("-", " "));

            if (form == "Gmax" || form == "Totem" || form == "Battle Bond" ||
                form.Contains("Power Construct")) continue;

            if ((Name == "Pikachu") & (!form.IsNullOrEmpty() || form == "Starter")) continue;

            if ((Name == "Tatsugiri" || Name == "Squawkabilly" || Name == "Miraidon" || Name == "Koraidon") &
                !form.IsNullOrEmpty()) continue;


            if ((Name == "Minior") & form.Contains("Blue"))
                form = form.Replace("Blue", "");
            else if ((Name == "Minior") & !form.Contains("Blue")) continue;

            if (Name_DE == Name) Name_DE = null;

            var Abilities = new List<Data.Ability>();

            foreach (var a in poke.Abilities)
            {
                var ab = abilities.FirstOrDefault(w =>
                    w.ID == a.Ability.Name);

                Abilities.Add(ab);

                Console.WriteLine(Name + " ability " + ab.Name_DE);
            }


            var Type1 = types.FirstOrDefault(w =>
                w.ID == poke.Types[0].Type.Name);
            var Type2 = poke.Types.Count == 2
                ? types.FirstOrDefault(w =>
                    w.ID == poke.Types[1].Type.Name)
                : null;

            var HEALTH = StatToInt(poke.Stats.FirstOrDefault(m => m.Stat.Name == "hp").BaseStat, 2);
            var ATK = StatToInt(poke.Stats.FirstOrDefault(m => m.Stat.Name == "attack").BaseStat);
            var DEF = StatToInt(poke.Stats.FirstOrDefault(m => m.Stat.Name == "defense").BaseStat);
            var SP_ATK = StatToInt(poke.Stats.FirstOrDefault(m => m.Stat.Name == "special-attack").BaseStat);
            ;
            var SP_DEF = StatToInt(poke.Stats.FirstOrDefault(m => m.Stat.Name == "special-defense").BaseStat);
            var SPEED = StatToInt(poke.Stats.FirstOrDefault(m => m.Stat.Name == "speed").BaseStat);
            ;

            pokes.Add(new Data.Pokemon
            {
                ID = ID,
                Order = Order,
                Name = Name,
                Name_DE = Name_DE,
                ImageLink = image,
                Form = form,
                Abilities = Abilities,
                Type1 = Type1,
                Type2 = Type2,
                HEALTH = HEALTH,
                ATK = ATK,
                DEF = DEF,
                SP_DEF = SP_DEF,
                SP_ATK = SP_ATK,
                SPEED = SPEED
            });

            Console.WriteLine(Name);
        }

        context.AddRange(pokes);

        await context.SaveChangesAsync();
    }

    public static async Task TransferLearnsets(ApplicationDbContext context, PokeApiClient apiclient)
    {

        if (context.Learnsets.Any())
        {
            Console.WriteLine("There already are learnsets");
            return;
        }
            
        var moves_local = context.MoveDex.ToArray();
        var pokemon = context.Pokedex.ToArray();
        foreach (var poke in pokemon)
        {
            var moves = apiclient.GetResourceAsync<Pokemon>(poke.ID).Result.Moves;
            var learnset = new List<Learnset>();
            foreach (var m in moves)
            {
                var vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "the-crown-tundra");
                string basedon = "The Crown Tundra";
                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "the-isle-of-armor");
                    basedon = "The Isle of Armor";
                }

                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "sword-shield");
                    basedon = "SwiSh";
                }

                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "the-indigo-disk");
                    basedon = "The Indigo Disk";
                }

                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "the-teal-mask");
                    basedon = "The Teal Mask";
                }

                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "scarlet-violet");
                    basedon = "ScarVio";
                }

                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m =>
                        m.VersionGroup.Name == "brilliant-diamond-and-shining-pearl");
                    basedon = "BDSP";
                }

                if (vergd.IsNullOrEmpty())
                {
                    vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "ultra-sun-ultra-moon");
                    basedon = "USUM";
                }

                var move = moves_local.FirstOrDefault(mv => mv.ID == m.Move.Name);
                foreach (var vers in vergd)
                {
                    var how = vers.MoveLearnMethod.Name;
                    var level = int.Parse(Math.Ceiling((double)vers.LevelLearnedAt / 5).ToString());
                    var l = new Learnset
                    {
                        ID = Guid.NewGuid().ToString(),
                        move = move,
                        how = how,
                        level = level,
                        source = basedon,
                        mon = poke
                    };
                    
                    learnset.Add(l);
                    Console.WriteLine($"{move.ID} {how} {level} {basedon} {poke.ID}");
                }
            }
            
            context.AddRange(learnset);
            context.SaveChanges();
        }
    }

    public static int StatToInt(int stat, double? nerf = null)
    {
        var calc = stat;
        if (nerf.HasValue)
            calc = Convert.ToInt32(Math.Ceiling(nerf.Value * calc));

        if (calc > 240) return 13;
        if (calc > 210) return 12;
        if (calc > 180) return 11;
        if (calc > 150) return 10;
        if (calc > 120) return 9;
        if (calc > 90) return 8;
        if (calc > 75) return 7;
        if (calc > 60) return 6;
        if (calc > 45) return 5;
        if (calc > 30) return 4;
        if (calc > 15) return 3;
        return 2;
    }


    public static string? StrengthToDice(int? strength, double? nerf = null)
    {
        if (!strength.HasValue) return null;

        var calc = strength.Value / 10;
        if (nerf.HasValue)
            calc = Convert.ToInt32(Math.Ceiling(nerf.Value * calc));

        switch (calc)
        {
            case <= 3:
                return "1d4";
            case 4:
                return "1d6";
            case 5:
                return "1d8";
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