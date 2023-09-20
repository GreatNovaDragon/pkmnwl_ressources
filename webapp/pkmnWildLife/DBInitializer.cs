using pkmnWildLife.Data;

namespace pkmnWildLife;

public class DBInitializer
{
    public static void InitializeDB(Data.ApplicationDbContext context)
    {

        var dice = new List<String> { "1d4", "1d6", "1d6", "1d8", "1d10", "1d12", "1d20"
                                    , "2d4", "2d6", "2d6", "2d8", "2d10", "2d12", "2d20"
                                    ,"3d4", "3d6", "3d6", "3d8", "3d10", "3d12", "3d20"};

        foreach (var d in dice)
        {
            context.DamageDice.Add(new DamageDice()
            {
                ID = Guid.NewGuid().ToString(),
                Effect = "AN DICE",
                Name = d
            });
        }
        context.SaveChangesAsync();

        
    }
}

