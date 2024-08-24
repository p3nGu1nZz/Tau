using System;
using System.Collections.Generic;

public class NameGenerator
{
    private static readonly List<string> names = new List<string>
    {
        "Alice", "Bob", "Charlie", "Diana", "Edward", "Fiona", "George", "Hannah", "Ian", "Julia",
        "Kevin", "Laura", "Michael", "Nina", "Oscar", "Paula", "Quentin", "Rachel", "Steve", "Tina",
        "Uma", "Victor", "Wendy", "Xander", "Yara", "Zach", "Aaron", "Bella", "Cameron", "Derek",
        "Eleanor", "Frank", "Grace", "Henry", "Isla", "Jack", "Kara", "Liam", "Mia", "Nathan",
        "Olivia", "Peter", "Quincy", "Rebecca", "Sam", "Tara", "Ulysses", "Violet", "Will", "Xena",
        "Yvonne", "Zane", "Amber", "Blake", "Cassandra", "Dean", "Evelyn", "Felix", "Gabriel",
        "Hazel", "Isaac", "Jade", "Kyle", "Luna", "Mason", "Nora", "Owen", "Piper", "Quinn",
        "Riley", "Sean", "Tessa", "Uriel", "Vanessa", "Wyatt", "Ximena", "Yosef", "Zara", "Aiden",
        "Brooke", "Caleb", "Daisy", "Ethan", "Faith", "Gavin", "Harper", "Ivy", "James", "Kaitlyn",
        "Logan", "Megan", "Noah", "Paige", "Ryder", "Sophia", "Tristan", "Vera", "Wesley", "Zoe",
        "Adrian", "Brianna", "Connor", "Delilah", "Eli", "Freya", "Gideon", "Hope", "Jasper", "Kelsey",
        "Landon", "Madeline", "Nolan", "Peyton", "Ronan", "Sienna", "Theo", "Vivian", "Weston", "Zion",
        "Asher", "Brandon", "Carter", "Dylan", "Emilia", "Finn", "Gianna", "Hudson", "Jaxon", "Kylie",
        "Levi", "Mackenzie", "Noelle", "Parker", "Reagan", "Savannah", "Tyler", "Valerie", "William", "Zoey",
        "Avery", "Brayden", "Chloe", "Dominic", "Elijah", "Fiona", "Grayson", "Hunter", "Jordan", "Kayla",
        "Lucas", "Maddox", "Nina", "Peyton", "Riley", "Scarlett", "Thomas", "Victoria", "Wyatt", "Zane"
    };

    private static readonly List<string> suffixes = new List<string>
    {
        "Junior", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"
    };

    private const double SuffixProbability = 0.1;

    public static string GenerateRandomTwoName()
    {
        string name = $"{GetRandomName()} {GetRandomName()}";
        if (new Random().NextDouble() < SuffixProbability)
        {
            name += $" {GetRandomSuffix()}";
        }
        return name;
    }

    private static string GetRandomName()
    {
        Random random = new Random();
        int index = random.Next(names.Count);
        return names[index];
    }

    private static string GetRandomSuffix()
    {
        Random random = new Random();
        int index = random.Next(suffixes.Count);
        return suffixes[index];
    }

    public static void AddName(string name)
    {
        if (!names.Contains(name))
        {
            names.Add(name);
        }
    }

    public static void RemoveName(string name)
    {
        if (names.Contains(name))
        {
            names.Remove(name);
        }
    }

    public static List<string> GetAllNames()
    {
        return new List<string>(names);
    }
}
