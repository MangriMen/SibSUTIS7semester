using lab15.Models;

static void printAbonents(ILookup<string, List<int>> abonents)
{
    foreach (var abonent in abonents)
    {
        var name_ = abonent.Key;
        var phones_ = string.Join(", ", abonent.Single());
        var encodedRegistry = $"{name_}:{phones_}";
        Console.WriteLine(encodedRegistry);
    }
}

var abonentList = new AbonentList();

var a = abonentList.GetAbonents();

abonentList.Add("Kyle", 25565);
abonentList.Add("Kyle", 25566);
abonentList.Add("Ale", 25565);

a = abonentList.GetAbonents();

printAbonents(a);
