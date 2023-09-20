using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace pkmnWildLife.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ability> Abilities { get; set; }
    public DbSet<DamageDice> DamageDice { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Pokemon> Pokedex { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<AttackMod> AttackModifiers { get; set; }
    public DbSet<Encounter> Encounters { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Character>().HasMany(e => e.Abilities).WithMany();
        builder.Entity<Pokemon>().HasMany(e => e.Abilities).WithMany();
        builder.Entity<Encounter>().HasMany(e => e.Enemies);
        base.OnModelCreating(builder);
    }
}

public class NameEffectDuo
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Effect { get; set; }
}

public class Ability : NameEffectDuo
{
}

public class DamageDice : NameEffectDuo
{
}

public class Move : NameEffectDuo
{
    public int type { get; set; }
    public DamageDice DamageDice { get; set; }
    public int MoveClass { get; set; }
}

public enum Type
{
    NORMAL = 1,
    FIGHTING,
    FLYING,
    POISON,
    GROUND,
    ROCK,
    BUG,
    GHOST,
    STEEL,
    FIRE,
    WATER,
    GRASS,
    ELECTRIC,
    PSYCHIC,
    ICE,
    DRAGON,
    DARK,
    FAIRY,
    UNKOWN,
    SHADOW,
    TERA_ALL
}

public enum MoveClass
{
    STATUS,
    PHYSICAL,
    SPECIAL
}

public class Item : NameEffectDuo
{
}

public class Pokemon
{
    public string ID { get; set; }

    public string Name { get; set; }
    public Statblock stats { get; set; }
    public List<Learnsets> learnset { get; set; }
    public List<Ability> Abilities { get; set; }
    public int Type1 { get; set; }
    public int Type2 { get; set; }
}

public class Learnsets
{
    public string ID { get; set; }
    public Move move { get; set; }
    public string when { get; set; }
}

public class Statblock
{
    public string ID { get; set; }
    public int HEALTH { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int SP_ATK { get; set; }
    public int SP_DEF { get; set; }
    public int SPEED { get; set; }
}

public class Character
{
    public string ID { get; set; }
    public string Name { get; set; }
    public IdentityUser Owner { get; set; }
    public Pokemon species { get; set; }
    public Statblock UserStatAdditions { get; set; }
    public List<Move> Moves { get; set; }
    public List<Ability> Abilities { get; set; }
    public int Level { get; set; }
    public int TeraType { get; set; }
    public int Grade { get; set; }
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int TempMaxHP { get; set; }
    public int Grit { get; set; }
    public int MaxGrit { get; set; }
    public List<SkillProfiency> Skills { get; set; }
    public List<Item> Inventory { get; set; }
}

public class SkillProfiency
{
    public string ID { get; set; }
    public Skill skill { get; set; }
    public int level { get; set; }
}

public class Skill : NameEffectDuo
{
}

public class AttackMod : NameEffectDuo
{
}

public class Encounter
{
    public string ID { get; set; }
    public List<Character> Enemies { get; set; }
}