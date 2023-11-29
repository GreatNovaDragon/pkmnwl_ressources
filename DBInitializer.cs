#region

using Microsoft.IdentityModel.Tokens;
using pkmnWildLife.Data;
using PokeApiNet;
using Ability = PokeApiNet.Ability;
using Item = PokeApiNet.Item;
using Move = PokeApiNet.Move;
using Pokemon = PokeApiNet.Pokemon;
using Type = PokeApiNet.Type;

#endregion

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
        // await TransferItems(context, apiclient);
        await TransferLearnsets(context, apiclient);
    }


    private static async Task TransferTypes(ApplicationDbContext context, PokeApiClient apiclient)
    {
        if (context.Types.Any())
        {
            Console.WriteLine("There already are Types");
            return;
        }

        var tps = new List<Data.Type>();
        await foreach (var TypeR in apiclient.GetAllNamedResourcesAsync<Type>())
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

        var movecls = new List<MoveClass>();

        await foreach (var mc in apiclient.GetAllNamedResourcesAsync<MoveDamageClass>())
        {
            var m = await apiclient.GetResourceAsync(mc);
            movecls.Add(new MoveClass
            {
                ID = m.Name,
                Name = m.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                Name_DE = m.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
            });

            Console.WriteLine(m.Name);
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


        var itms = new List<Data.Item>();
        await foreach (var i in apiclient.GetAllNamedResourcesAsync<Item>())
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

        var abs = new List<Data.Ability>();

        var it = 0;

        await foreach (var i in apiclient.GetAllNamedResourcesAsync<Ability>())
        {
            var Item = await apiclient.GetResourceAsync(i);
            var ID = Item.Name;
            var Name = Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect = Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect.Replace("one stage", "2")
                    .Replace("two stages", "4").Replace("three stages", "6")
                : Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en").FlavorText
                    : "No Entry";


            // var moves_old = Helpers.csv2ab("abilities_old.csv");


            /* if (moves_old.Where(mn => mn.ability == Name).Any())
             {
                 var mn = moves_old.Where(m => m.ability == Name).FirstOrDefault();
                 Effect = mn.effect;
             }
 */

            abs.Add(new Data.Ability
            {
                ID = ID,
                Name = Name,
                Name_DE = Name_DE,
                Effect = Effect,
                IsTrait = false
            });
            it++;
            Console.WriteLine($"Ability {it}");
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
        var mvs = new List<Data.Move>();

        var dclass = context.MoveClasses.ToList();

        var it = 0;
        await foreach (var i in apiclient.GetAllNamedResourcesAsync<Move>())
        {
            var m = await apiclient.GetResourceAsync(i);

            var ID = m.Name;
            var Name = m.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                ? m.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                : m.Name;

            var Name_DE = m.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;

            var Effect = m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                : m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                        .Replace("Has a $effect_chance% chance", $"Roll a {20 - m.EffectChance / 5} or higher on a d20")
                        .Replace("one stage", "2").Replace("two stages", "4").Replace("three stages", "6")
                    : m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en").FlavorText
                        : "No Data";

            Effect = accuracy_string(m.Accuracy) + Effect;
            Effect = Effect.Replace("Inflicts regular damage.", "");

            var Target = m.Target.Name;

            var DamageDice = StrengthToDice(m.Power, 0.75);
            var MType = types.FirstOrDefault(e =>
                e.ID == m.Type.Name);
            var DamageClass =
                dclass.FirstOrDefault(e =>
                    e.ID == m.DamageClass.Name);

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

            it++;
            Console.WriteLine($"Move {it}");
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
        var pokes = new List<Data.Pokemon>();
        var i = 0;
        await foreach (var p in apiclient.GetAllNamedResourcesAsync<Pokemon>())
        {
            var poke = await apiclient.GetResourceAsync(p);
            var species = apiclient.GetResourceAsync(poke.Species).Result;


            var ID = poke.Name;
            var Order = species.Order;
            var Name = species.Names
                .FirstOrDefault(n => n.Language.Name == "en").Name;

            var image = poke.Sprites.Other.DreamWorld.FrontDefault;
            if (image.IsNullOrEmpty()) image = poke.Sprites.Other.OfficialArtwork.FrontDefault;
            if (image.IsNullOrEmpty()) image = poke.Sprites.FrontDefault;

            var Name_DE = species.Names
                .FirstOrDefault(n => n.Language.Name == "de")?.Name;

            var form = "";
            if (!Name.ToLower().Equals(ID.Replace("-", " ")))
                form = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(ID.Replace($"{Name.ToLower()}-", "")
                    .Replace("-", " "));

            if (form.Contains("Gmax") || form == "Totem" || form == "Battle Bond" ||
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
            }


            var Type1 = types.FirstOrDefault(w =>
                w.ID == poke.Types[0].Type.Name);
            var Type2 = poke.Types.Count == 2
                ? types.FirstOrDefault(w =>
                    w.ID == poke.Types[1].Type.Name)
                : null;


            var HEALTH = StatToInt(poke.Stats[0].BaseStat, 2);
            var ATK = StatToInt(poke.Stats[1].BaseStat);
            var DEF = StatToInt(poke.Stats[2].BaseStat);
            var SP_ATK = StatToInt(poke.Stats[3].BaseStat);
            ;
            var SP_DEF = StatToInt(poke.Stats[4].BaseStat);
            var SPEED = StatToInt(poke.Stats[5].BaseStat);
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
            i++;
            Console.WriteLine(
                $"pokemon {i}");
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
        var p = 0;
        foreach (var poke in pokemon)
        {
            var moves = apiclient.GetResourceAsync<Pokemon>(poke.ID).Result.Moves;
            var learnset = new List<Learnset>();
            var m_it = 0;
            foreach (var m in moves)
            {
                var vergd = m.VersionGroupDetails.Where(m => m.VersionGroup.Name == "the-crown-tundra");
                var basedon = "The Crown Tundra";
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
                var meth_it = 0;
                foreach (var vers in vergd)
                {
                    var how = vers.MoveLearnMethod.Name;
                    var level = int.Parse(Math.Ceiling((double)vers.LevelLearnedAt / 5).ToString());

                    if (how != "level-up")
                    {
                        level = 0;
                        if (how != "egg")
                            level = int.MaxValue;
                    }

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
                    meth_it++;

                    Console.WriteLine(
                        $"MLP ({meth_it}/{vergd.Count()})/({m_it}/{moves.Count})/({p}/{pokemon.Length})");
                }

                m_it++;
                Console.WriteLine($"LP ({m_it}/{moves.Count})/({p}/{pokemon.Length})");
            }

            context.AddRange(learnset);
            p++;
            Console.WriteLine($"P ({p}/{pokemon.Length})");


            await context.SaveChangesAsync();
        }

        var lese = context.Learnsets;
        var unknown_Adds = new List<Learnset>();

        foreach (var move in moves_local)
        {
            var apimove = apiclient.GetResourceAsync<Move>(move.ID).Result.LearnedByPokemon;
            foreach (var po in apimove)
            {
                var query = from ls in lese where ls.mon.ID == po.Name && ls.move == move select ls;
                if (query.Any()) continue;
                var mon = context.Pokedex.FirstOrDefault(pman => pman.ID == po.Name);
                if (mon == null) continue;
                var set = new Learnset
                {
                    ID = Guid.NewGuid().ToString(),
                    move = move,
                    how = "Unknown",
                    level = int.MaxValue,
                    source = "THE VOID",
                    mon = mon
                };

                unknown_Adds.Add(set);
            }
        }

        context.AddRange(unknown_Adds);
        await context.SaveChangesAsync();
    }

    public static int StatToInt(int stat, double? nerf = null)
    {
        var calc = stat;
        if (nerf.HasValue)
            calc = Convert.ToInt32(Math.Ceiling(nerf.Value * calc));
        return Convert.ToInt32(calc * ((double)18 / 200));
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

    public static string accuracy_string(int? accuracy)
    {
        if (accuracy.HasValue)
        {
            if (accuracy == 100)
                return "";
            return $"Roll an {accuracy / 5} or lower on a d20 for this move to succeed. ";
        }

        return "";
    }
}