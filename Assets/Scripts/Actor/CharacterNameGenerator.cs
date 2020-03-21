using System;

public enum NameType
{
    RandomFirstName,
    RandomName,
    RandomLastName,
    RandomPseudonym,
    ExistingPseudonym,
    ExistingCombination
}


public static class CharacterNameGenerator
{
    public struct NameCombination
    {
        public NameCombination(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        public string FirstName;
        public string LastName;
    }

    public static CharacterName GenerateName()
    {
        Random random = new Random();
        int randomNumber = random.Next(20);
        //Logger.Log("randomNumber {0}", randomNumber);
        switch (randomNumber)
        {
            case int n when (n == 0): // pick existing pseudonym
                return new CharacterName(PickName(NameType.ExistingPseudonym));
            case int n when (n <= 3): // pick existing first-last name combination
                return new CharacterName(PickFullName(NameType.ExistingCombination));
            case int n when (n <= 6): // pick random pseudonym
                return new CharacterName(PickName(NameType.RandomPseudonym));
            default:  // pick random first-last name combination
                return new CharacterName(PickFullName(NameType.RandomName));
        }
    }

    public static string GetName(CharacterName characterName)
    {
        if (characterName.HasLastName) return characterName.FirstName + " " + characterName.LastName;

        return characterName.FirstName;
    }

    public static string PickName(NameType nameType)
    {
        switch (nameType)
        {
            case NameType.RandomFirstName:
                return Util.GetRandomValue(FirstNames);
            case NameType.RandomLastName:
                return Util.GetRandomValue(LastNames);
            case NameType.RandomPseudonym:
                return Util.GetRandomValue(RandomPseudonyms);
            case NameType.ExistingPseudonym:
                return Util.GetRandomValue(ExistingPseudonyms);
            case NameType.ExistingCombination:
                NameCombination name = Util.GetRandomValue(ExistingCombinations);
                return "todo";
            default:
                Logger.Error(Logger.Character, "Not implemented name type {0}", nameType);
                return "";
        }
    }

    public static string[] PickFullName(NameType nameType)
    {
        switch (nameType)
        {
            case NameType.RandomName:
                return new string[] { Util.GetRandomValue(FirstNames), Util.GetRandomValue(LastNames) };
            case NameType.ExistingCombination:
                NameCombination name = Util.GetRandomValue(ExistingCombinations);
                return new string[] { name.FirstName, name.LastName };
            default:
                Logger.Error(Logger.Character, "Not implemented name type {0}", nameType);
                return new string[] { };
        }
        
    }

    public static string[] ExistingPseudonyms =
    {
        "Prince",
        "Slash",
        "Sting"
    };

    public static string[] RandomPseudonyms =
    {
        "Smog",
        "Gorilla",
        "Myst"
    };

    public static NameCombination[] ExistingCombinations =
    {
        new NameCombination("John", "Lennon"),
        new NameCombination("Paul", "McCartney"),
        new NameCombination("George", "Harrison"),
        new NameCombination("Ringo", "Starr"),
        new NameCombination("Frank", "Zappa"),
    };

    public static string[] FirstNames = { 
        "Dave",
        "George",
        "John",
        "Paul",
        "Pete",
        "Ringo",
        "Roger",
    };

    public static string[] LastNames =
    {
        "Brown",
        "Davis",
        "Johnson",
        "Jones",
        "Miller",
        "Smith",
        "Williams",
        "Wilson",
    };
}
