// See https://aka.ms/new-console-template for more information

using PokeApiNet;


PokeApiClient apiclient = new PokeApiClient();

NamedApiResourceList<PokeApiNet.Type> types = await apiclient.GetNamedResourcePageAsync<PokeApiNet.Type>(9999, 0);

foreach (var t in types.Results)
{
    PokeApiNet.Type Type = await apiclient.GetResourceAsync(t);
    Console.WriteLine(Type.Names.FirstOrDefault(n => n.Language.Name == "en").Name);
}


NamedApiResourceList<PokeApiNet.Item> items = await apiclient.GetNamedResourcePageAsync<Item>(9999, 0);
        
foreach (var i in items.Results)
{
    Item Item = await apiclient.GetResourceAsync(i);
        Console.WriteLine(Item.Id);
        Console.WriteLine(Item.Name);
        Console.WriteLine(Item.Names.FirstOrDefault(n => n.Language.Name == "en").Name);
        Console.WriteLine(Item.Names.FirstOrDefault(n => n.Language.Name == "de")?.Name);
        Console.WriteLine(Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de") != null
            ? Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "de").Effect
            : (Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en") != null
                ?
                Item.EffectEntries.FirstOrDefault(n => n.Language.Name == "en").Effect
                    : "No Entry"));
        Console.WriteLine("----");
        
    
}