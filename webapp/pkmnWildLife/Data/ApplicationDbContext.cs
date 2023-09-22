﻿using Microsoft.AspNetCore.Identity;
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
    public DbSet<Type> Types { get; set; }
    public DbSet<MoveClass> MoveClass { get; set; }
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

public class Ability
{
    public string ID { get; set; }

    public string? Name_DE { get; set; }
    public string Name { get; set; }
    public string Effect { get; set; }
}

public class DamageDice
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string Effect { get; set; }
}

public class Move
{
    public string ID { get; set; }
    public string? Name_DE { get; set; }
    public string Name_EN { get; set; }

    public int type { get; set; }
    public DamageDice DamageDice { get; set; }
    public int MoveClass { get; set; }
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
    public string ID { get; set; }

    public string Name { get; set; }
    public string? Name_DE { get; set; }

    public List<Learnsets> learnset { get; set; }
    public List<Ability> Abilities { get; set; }
    public int Type1 { get; set; }
    public int Type2 { get; set; }

    public int HEALTH { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int SP_ATK { get; set; }
    public int SP_DEF { get; set; }
    public int SPEED { get; set; }
}

public class Learnsets
{
    public string ID { get; set; }
    public Move move { get; set; }
    public string when { get; set; }
}

public class Character
{
    public string ID { get; set; }
    public string Name { get; set; }
    public IdentityUser Owner { get; set; }
    public Pokemon species { get; set; }
    public List<Move> Moves { get; set; }
    public List<Ability> Abilities { get; set; }
    public int Level { get; set; }
    public int TeraType { get; set; }
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

public class Encounter
{
    public string ID { get; set; }
    public List<Character> Enemies { get; set; }
}