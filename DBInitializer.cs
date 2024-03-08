#region

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
            tps.Add(
                new Data.Type
                {
                    ID = Type.Name,
                    Name = Type.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                    Name_DE = Type.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
                }
            );
        }

        context.AddRange(tps);
        await context.SaveChangesAsync();

        var relations = new List<DamageRelations>();
        foreach (var t in context.Types)
        {
            var Type = await apiclient.GetResourceAsync<Type>(t.ID);

            var doublefrom = new List<Data.Type>();
            foreach (var df in Type.DamageRelations.DoubleDamageFrom)
                doublefrom.Add(context.Types.FirstOrDefault(d => d.ID == df.Name));

            var doubleto = new List<Data.Type>();
            foreach (var df in Type.DamageRelations.DoubleDamageTo)
                doubleto.Add(context.Types.FirstOrDefault(d => d.ID == df.Name));

            var halfto = new List<Data.Type>();
            foreach (var df in Type.DamageRelations.HalfDamageTo)
                halfto.Add(context.Types.FirstOrDefault(d => d.ID == df.Name));

            var halffrom = new List<Data.Type>();
            foreach (var df in Type.DamageRelations.HalfDamageFrom)
                halffrom.Add(context.Types.FirstOrDefault(d => d.ID == df.Name));

            var noto = new List<Data.Type>();
            foreach (var df in Type.DamageRelations.NoDamageTo)
                noto.Add(context.Types.FirstOrDefault(d => d.ID == df.Name));

            var nofrom = new List<Data.Type>();
            foreach (var df in Type.DamageRelations.NoDamageFrom)
                nofrom.Add(context.Types.FirstOrDefault(d => d.ID == df.Name));

            var dr = new DamageRelations();

            relations.Add(
                new DamageRelations
                {
                    ID = $"relations_{t.ID}",
                    Type = t,

                    doubleDamageFrom = doublefrom,
                    doubleDamageTo = doubleto,
                    halfDamageFrom = halffrom,
                    halfDamageTo = halfto,
                    noDamageFrom = nofrom,
                    noDamageTo = noto
                }
            );
        }

        context.AddRange(relations);
        await context.SaveChangesAsync();
    }

    private static async Task TransferMoveClasses(
        ApplicationDbContext context,
        PokeApiClient apiclient
    )
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
            movecls.Add(
                new MoveClass
                {
                    ID = m.Name,
                    Name = m.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                    Name_DE = m.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name
                }
            );

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
                "dynamax-crystals",
                "sandwhich-ingredients",
                "tm-materials",
                "picnic",
                "species-candies",
                "all-machines",
                "all-mail",
                "plot-advancement"
            };

            if (undesirables.Contains(Item.Category.Name))
                continue;

            var ID = $"{Item.Name}_{Item.Id}";

            var Name =
                Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                    : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect =
                Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                    : Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                        : "No Data";
            itms.Add(
                new Data.Item
                {
                    ID = ID,
                    Name = Name,
                    Name_DE = Name_DE,
                    Effect = Effect
                }
            );
        }

        context.AddRange(itms);
        await context.SaveChangesAsync();
    }

    private static async Task TransferAbilities(
        ApplicationDbContext context,
        PokeApiClient apiclient
    )
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
            var Name =
                Item.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                    : Item.Name;
            var Name_DE = Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;
            var Effect =
                Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                    : Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? Item
                            .FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en")
                            .FlavorText
                        : "No Entry";

            var ShortEffect =
                Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").ShortEffect
                    : Item.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? Item
                            .FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en")
                            .FlavorText
                        : "No Entry";

            // var moves_old = Helpers.csv2ab("abilities_old.csv");

            /* if (moves_old.Where(mn => mn.ability == Name).Any())
             {
                 var mn = moves_old.Where(m => m.ability == Name).FirstOrDefault();
                 Effect = mn.effect;
             }
 */

            abs.Add(
                new Data.Ability
                {
                    ID = ID,
                    Name = Name,
                    Name_DE = Name_DE,
                    Effect = Effect,
                    ShortEffect = ShortEffect,
                    IsTrait = false
                }
            );
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

        var traits = Helpers.Csv2Trait("traits.csv");
        var trs = new List<Data.Ability>();

        foreach (var tr in traits)
            trs.Add(
                new Data.Ability
                {
                    ID = tr.Name.ToLower().Normalize() + "_trait",
                    Effect = tr.effect,
                    Name = tr.Name,
                    Requirement = tr.Requirement,
                    IsTrait = true,
                    Order = tr.Requirement.Contains("Grade")
                        ? int.Parse(tr.Requirement.Replace("Grade ", ""))
                        : 999
                }
            );
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
            var Name =
                m.Names.FirstOrDefault(n => n.Language.Name == "en") != null
                    ? m.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                    : m.Name;

            var Name_DE = m.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;

            var Effect =
                m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                    ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
                    : m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? m
                            .EffectEntries.FirstOrDefault(n => n.Language.Name == "en")
                            .Effect.Replace("1/16", "gradD4")
                            .Replace("1/8", "(2*Grad)D4")
                        : m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                            ? m
                                .FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en")
                                .FlavorText
                            : "No Data";

            if (ID == "ingrain")
                Effect +=
                    " \n während verwurzler aktiv ist, kannst du statt anzugreifen, dich deine normale bewegungsreichweite"
                    + " bewegen ohne das verwurzler aufgehoben wird, rennen ist nicht möglich.";

            var ShortEffect =
                m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
                    ? m.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").ShortEffect
                    : m.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                        ? m
                            .EffectEntries.FirstOrDefault(n => n.Language.Name == "en")
                            .ShortEffect.Replace("1/16", "gradD4")
                            .Replace("1/8", "(2*Grad)D4")
                        : m.FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                            ? m
                                .FlavorTextEntries.FirstOrDefault(n => n.Language.Name == "en")
                                .FlavorText
                            : "No Data";

            if (!m.Priority.Equals(0))
            {
                var prio = $"\n\n Has a priority of {m.Priority}.";
                ShortEffect += prio;
                Effect += prio;
            }

            Effect = accuracy_string_en(m.Accuracy) + Effect;
            ShortEffect = accuracy_string_en(m.Accuracy) + Effect;            

            var Target = m.Target.Name;

            var DamageDice = StrengthToDice(m.Power, 0.8);
            var MType = types.FirstOrDefault(e => e.ID == m.Type.Name);
            var DamageClass = dclass.FirstOrDefault(e => e.ID == m.DamageClass.Name);

            mvs.Add(
                new Data.Move
                {
                    ID = ID,
                    Name = Name,
                    Name_DE = Name_DE,
                    Effect = Effect,
                    ShortEffect = ShortEffect,
                    type = MType,
                    Target = Target,
                    DamageDice = DamageDice,
                    MoveClass = DamageClass
                }
            );

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
            var Order = poke.Order;
            var Dex = species.Order;
            var Name = species.Names.FirstOrDefault(n => n.Language.Name == "en").Name;

            var image = poke.Sprites.Other.OfficialArtwork.FrontDefault;
            if (string.IsNullOrEmpty(image))
                image = poke.Sprites.Other.Home.FrontDefault;

            if (string.IsNullOrEmpty(image))
                image = poke.Sprites.FrontDefault;

            var Name_DE = species.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name;

            var form = apiclient
                .GetResourceAsync(poke.Forms[0])
                .Result.Names.FirstOrDefault(n => n.Language.Name == "en")
                ?.Name;
            var form_de = apiclient
                .GetResourceAsync(poke.Forms[0])
                .Result.Names.FirstOrDefault(n => n.Language.Name == "de")
                ?.Name;

            if (
                form_de != null
                && (
                    form_de.Contains("Gigadynamax")
                    || form_de.Contains("Totem")
                    || ID.Contains("mimigma-busted")
                    || ID.Contains("cramorant-")
                    || ID.Contains("koraidon-")
                    || ID.Contains("miraidon-")
                )
            )
                continue;

            if ((Name == "Pikachu") & !string.IsNullOrEmpty(form))
                continue;
            if (
                form_de != null
                && (
                    Name_DE == "Kikugi"
                    || form_de.Contains("Westliches")
                    || form_de.Contains("Frühling")
                    || ID == "flabebe"
                    || ID == "floette"
                    || ID == "florges"
                    || Name_DE == "Xerneas"
                    || ID == "sinistea"
                    || ID == "poltageist"
                    || ID == "alcremie"
                )
            )
            {
                form = "";
                form_de = "";
            }

            if (Name_DE == Name)
                Name_DE = null;

            var Abilities = new List<Data.Ability>();

            foreach (var a in poke.Abilities)
            {
                var ab = abilities.FirstOrDefault(w => w.ID == a.Ability.Name);

                Abilities.Add(ab);
            }

            if (Name_DE == "Quajutsu")
                Abilities.Add(abilities.FirstOrDefault(w => w.Name == "Battle Bond"));

            if (ID.Contains("power-construct"))
                continue;
            if (ID.Contains("zygarde"))
                Abilities.Add(abilities.FirstOrDefault(w => w.ID == "power-construct"));

            if (ID.Contains("own-tempo"))
                continue;

            if (ID == "rockruff")
                Abilities.Add(abilities.FirstOrDefault(w => w.ID == "own-tempo"));

            if (ID == "minior-blue")
            {
                ID = "minior";
                form_de = "";
                form = "";
            }

            if (ID == "minior-blue-meteor")
            {
                ID = "minior-meteor";
                form_de = "Meteno (Meteor)";
                form = "Minior (Meteor)";
            }

            if (ID.Contains("minior-") & (ID != "minior-meteor"))
                continue;

            var Type1 = types.FirstOrDefault(w => w.ID == poke.Types[0].Type.Name);
            var Type2 =
                poke.Types.Count == 2
                    ? types.FirstOrDefault(w => w.ID == poke.Types[1].Type.Name)
                    : null;

            var HEALTH = StatToInt(poke.Stats[0].BaseStat, 2);

            var isFOCKINGSPECIALMATE = species.IsLegendary || species.IsMythical;
            var DEF_NERF = isFOCKINGSPECIALMATE ? 0.9 : 1;
            var ATK_BUFF = isFOCKINGSPECIALMATE ? 1 : 1.1;
            var ATK = StatToInt(poke.Stats[1].BaseStat, ATK_BUFF);
            var DEF = StatToInt(poke.Stats[2].BaseStat, DEF_NERF) + (isFOCKINGSPECIALMATE ? 3 : 6);
            var SP_ATK = StatToInt(poke.Stats[3].BaseStat, ATK_BUFF);
            ;
            var SP_DEF =
                StatToInt(poke.Stats[4].BaseStat, DEF_NERF) + (isFOCKINGSPECIALMATE ? 3 : 6);
            var SPEED = StatToInt(poke.Stats[5].BaseStat);
            ;

            pokes.Add(
                new Data.Pokemon
                {
                    ID = ID,
                    Dex = Dex,
                    Name = Name,
                    Name_DE = Name_DE,
                    ImageLink = image,
                    Form = form,
                    Form_DE = form_de,
                    Abilities = Abilities,
                    Type1 = Type1,
                    Type2 = Type2,
                    HEALTH = HEALTH,
                    ATK = ATK,
                    DEF = DEF,
                    SP_DEF = SP_DEF,
                    SP_ATK = SP_ATK,
                    SPEED = SPEED
                }
            );
            i++;
            Console.WriteLine($"pokemon {i}");
        }

        context.AddRange(pokes);

        await context.SaveChangesAsync();
    }

    public static async Task TransferLearnsets(
        ApplicationDbContext context,
        PokeApiClient apiclient
    )
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
            var id = poke.ID;
            if (id == "minior")
                id = "minior-blue";

            if (id == "minior-meteor")
                id = "minior-blue-meteor";
            var moves = apiclient.GetResourceAsync<Pokemon>(id).Result.Moves;
            var learnset = new List<Learnset>();
            var m_it = 0;
            foreach (var m in moves)
            {
                var vergd = m.VersionGroupDetails;

                var move = moves_local.FirstOrDefault(mv => mv.ID == m.Move.Name);
                var meth_it = 0;
                foreach (var vers in vergd)
                {
                    var how = vers.MoveLearnMethod.Name;
                    var level = int.Parse(Math.Ceiling((double)vers.LevelLearnedAt / 5).ToString());

                    if (how != "level-up")
                        level = int.MaxValue;

                    var l = new Learnset
                    {
                        ID = Guid.NewGuid().ToString(),
                        move = move,
                        how = how,
                        level = level,
                        mon = poke
                    };

                    var checker = learnset.FirstOrDefault(l => l.move == move && l.how == how);
                    if (checker != null)
                    {
                        if (checker.level > level)
                            learnset.Remove(checker);
                        else
                            continue;
                    }

                    learnset.Add(l);
                    meth_it++;

                    Console.WriteLine(
                        $"MLP ({meth_it}/{vergd.Count()})/({m_it}/{moves.Count})/({p}/{pokemon.Length}  {poke.Name})"
                    );
                }

                m_it++;
                Console.WriteLine($"LP ({m_it}/{moves.Count})/({p}/{pokemon.Length} {poke.Name})");
            }

            context.AddRange(learnset);
            p++;
            Console.WriteLine($"P ({p}/{pokemon.Length} {poke.Name})");

            await context.SaveChangesAsync();
        }
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
        if (!strength.HasValue)
            return null;

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

    public static string accuracy_string_en(int? accuracy)
    {
        if (accuracy.HasValue)
        {
            if (accuracy == 100)
                return "";
            return $"This move has an {accuracy}% chance to succeed. ";
        }

        return "";
    }
}
