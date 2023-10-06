using System.ComponentModel.DataAnnotations.Schema;
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

    public DbSet<Ability> AbilityDex { get; set; }
    public DbSet<Move> MoveDex { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Type> Types { get; set; }
    public DbSet<MoveClass> MoveClasses { get; set; }
    public DbSet<Pokemon> Pokedex { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<AttackMod> AttackModifiers { get; set; }
    public DbSet<Learnset> Learnsets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Character>().HasMany(e => e.Abilities).WithMany();
        builder.Entity<Pokemon>().HasMany(e => e.Abilities).WithMany();
        builder.Entity<Character>().HasMany(e => e.Inventory).WithMany();
        builder.Entity<Character>().HasMany(e => e.Moves).WithMany();
        base.OnModelCreating(builder);
    }
}

public class Ability
{
    public string ID { get; set; }

    public string? Name_DE { get; set; }
    public string Name { get; set; }
    public string Effect { get; set; }

    public bool IsTrait { get; set; }
    public string? Requirement { get; set; }
}

public class Move
{
    public string ID { get; set; }
    public string? Name_DE { get; set; }
    public string Name { get; set; }

    public Type type { get; set; }
    public string? DamageDice { get; set; }
    public MoveClass MoveClass { get; set; }
    public string Target { get; set; }
    public string Effect { get; set; }
}

public class Type
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string? Name_DE { get; set; }
}

public class MoveClass
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string? Name_DE { get; set; }
}

public class Item
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Effect { get; set; }
    public string? Name_DE { get; set; }
}

public class Pokemon
{
    [Column(Order = 2)] public string ID { get; set; }

    [Column(Order = 0)] public int Order { get; set; }

    public string? ImageLink { get; set; }

    public string Name { get; set; }
    public string? Name_DE { get; set; }

    public string? Form { get; set; }
    public List<Ability> Abilities { get; set; }
    public Type Type1 { get; set; }
    public Type? Type2 { get; set; }

    public int HEALTH { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int SP_ATK { get; set; }
    public int SP_DEF { get; set; }
    public int SPEED { get; set; }
}


public class Learnset
{
    public string ID { get; set; }

    public Pokemon mon { get; set; }

    public Move move { get; set; }
    public string how { get; set; }

    public int level { get; set; }

    public string? source { get; set; }
}

public class Character
{
    public string ID { get; set; }
    public string Name { get; set; }
    public IdentityUser Owner { get; set; }
    public IdentityUser DM { get; set; }
    public Pokemon species { get; set; }
    public List<Move> Moves { get; set; }
    public List<Ability> Abilities { get; set; }
    public int Level { get; set; }
    public int? TeraType { get; set; }
    public int Grade { get; set; }
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int TempMaxHP { get; set; }

    public int HEALTH_bonus { get; set; }
    public int ATK_bonus { get; set; }
    public int DEF_bonus { get; set; }
    public int SP_ATK_bonus { get; set; }
    public int SP_DEF_bonus { get; set; }
    public int SPEED_bonus { get; set; }

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

public class Skill
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string? Name_DE { get; set; }

    public string Effect { get; set; }
}

public class AttackMod
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string? Name_DE { get; set; }

    public string Effect { get; set; }
}