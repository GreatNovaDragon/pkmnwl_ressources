#region

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#endregion

namespace pkmnWildLife.Data;

public class ApplicationDbContext : DbContext
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

    public DbSet<DamageRelations> DamageRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Character>().HasMany(e => e.Abilities).WithMany();
        builder.Entity<Pokemon>().HasMany(e => e.Abilities).WithMany();
        builder.Entity<Character>().HasMany(e => e.Inventory).WithMany();
        builder.Entity<Character>().HasMany(e => e.Moves).WithMany();

        builder.Entity<DamageRelations>().HasMany(e => e.noDamageTo).WithMany();
        builder.Entity<DamageRelations>().HasMany(e => e.noDamageFrom).WithMany();
        builder.Entity<DamageRelations>().HasMany(e => e.halfDamageTo).WithMany();
        builder.Entity<DamageRelations>().HasMany(e => e.halfDamageFrom).WithMany();
        builder.Entity<DamageRelations>().HasMany(e => e.doubleDamageTo).WithMany();
        builder.Entity<DamageRelations>().HasMany(e => e.doubleDamageFrom).WithMany();

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

    public virtual Type type { get; set; }
    public string? DamageDice { get; set; }
    public virtual MoveClass MoveClass { get; set; }
    public string Target { get; set; }
    public string Effect { get; set; }
}

public class Type
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string? Name_DE { get; set; }
}

public class DamageRelations
{
    public string ID { get; set; }
    public virtual Type Type { get; set; }
    public virtual List<Type> doubleDamageFrom { get; set; }
    public virtual List<Type> doubleDamageTo { get; set; }

    public virtual List<Type> halfDamageFrom { get; set; }
    public virtual List<Type> halfDamageTo { get; set; }

    public virtual List<Type> noDamageFrom { get; set; }
    public virtual List<Type> noDamageTo { get; set; }
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
    public virtual List<Ability> Abilities { get; set; }
    public virtual Type Type1 { get; set; }
    public virtual Type? Type2 { get; set; }

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

    public virtual Pokemon mon { get; set; }

    public virtual Move move { get; set; }
    public string how { get; set; }

    public int level { get; set; }
}

public class Character
{
    public string ID { get; set; }
    public string Name { get; set; }
    public virtual Pokemon species { get; set; }
    public virtual List<Move> Moves { get; set; }
    public virtual List<Ability> Abilities { get; set; }
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
    public virtual List<SkillProfiency> Skills { get; set; }
    public virtual List<Item> Inventory { get; set; }
}

public class SkillProfiency
{
    public string ID { get; set; }
    public virtual Skill skill { get; set; }
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