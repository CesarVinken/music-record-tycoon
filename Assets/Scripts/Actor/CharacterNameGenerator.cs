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

    public static CharacterName GenerateName(Gender gender)
    {
        Random random = new Random();
        int randomNumber = random.Next(20);
        //Logger.Log("randomNumber {0}", randomNumber);
        switch (randomNumber)
        {
            case int n when (n == 0): // pick existing pseudonym
                return new CharacterName(PickName(NameType.ExistingPseudonym, gender));
            case int n when (n <= 3): // pick existing first-last name combination
                return new CharacterName(PickFullName(NameType.ExistingCombination, gender));
            case int n when (n <= 6): // pick random pseudonym
                return new CharacterName(PickName(NameType.RandomPseudonym, gender));
            default:  // pick random first-last name combination
                return new CharacterName(PickFullName(NameType.RandomName, gender));
        }
    }

    public static string GetName(CharacterName characterName)
    {
        if (characterName.HasLastName) return characterName.FirstName + " " + characterName.LastName;

        return characterName.FirstName;
    }

    public static string PickName(NameType nameType, Gender gender)
    {
        switch (nameType)
        {
            case NameType.RandomFirstName:
                return gender == Gender.Male 
                    ? Util.GetRandomValue(FirstNamesMale)
                    : Util.GetRandomValue(FirstNamesFemale);
            case NameType.RandomLastName:
                return Util.GetRandomValue(LastNames);
            case NameType.RandomPseudonym:
                return Util.GetRandomValue(RandomPseudonyms);
            case NameType.ExistingPseudonym:
                return gender == Gender.Male
                    ? Util.GetRandomValue(ExistingPseudonymsMale)
                    : Util.GetRandomValue(ExistingPseudonymsFemale);
            default:
                Logger.Error(Logger.Character, "Not implemented name type {0}", nameType);
                return "";
        }
    }

    public static string[] PickFullName(NameType nameType, Gender gender)
    {
        switch (nameType)
        {
            case NameType.RandomName:
                string firstName = gender == Gender.Male
                    ? Util.GetRandomValue(FirstNamesMale)
                    : Util.GetRandomValue(FirstNamesFemale);
                return new string[] { firstName, Util.GetRandomValue(LastNames) };
            case NameType.ExistingCombination:
                NameCombination name = gender == Gender.Male
                    ? Util.GetRandomValue(ExistingCombinationsMale)
                    : Util.GetRandomValue(ExistingCombinationsFemale);
                return new string[] { name.FirstName, name.LastName };
            default:
                Logger.Error(Logger.Character, "Not implemented name type {0}", nameType);
                return new string[] { };
        }
        
    }

    public static Gender PickGender()
    {
        Random random = new Random();
        int randomNumber = random.Next(2);
        if (randomNumber == 0) return Gender.Female;
        else return Gender.Male;
    }

    public static string[] ExistingPseudonymsMale =
    {
        "Prince",
        "Slash",
        "Sting",
        "Bono",
    };

    public static string[] ExistingPseudonymsFemale =
    {
        "Shakira",
        "Björk",
        "Madonna",
        "Beyoncé",
    };

    public static string[] RandomPseudonyms =
    {
        "Smog",
        "Gorilla",
        "Myst",
        "The Druid"
    };

    public static NameCombination[] ExistingCombinationsMale =
    {
        new NameCombination("John", "Lennon"),
        new NameCombination("Paul", "McCartney"),
        new NameCombination("George", "Harrison"),
        new NameCombination("Ringo", "Starr"),
        new NameCombination("Frank", "Zappa"),
    };

    public static NameCombination[] ExistingCombinationsFemale =
{
        new NameCombination("Alicia", "Keys"),
        new NameCombination("Aretha", "Franklin"),
        new NameCombination("Britney", "Spears"),
        new NameCombination("Janis", "Joplin"),
        new NameCombination("Kate", "Bush"),
    };

    public static string[] FirstNamesMale = {
        "Dave",
        "George",
        "John",
        "Paul",
        "Pete",
        "Ringo",
        "Roger",
    };

    public static string[] FirstNamesFemale = {
        "Emily",
        "Helen",
        "Jennifer",
        "Karen",
        "Lisa",
        "Sarah",
        "Susan",
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
