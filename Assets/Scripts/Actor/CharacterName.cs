public class CharacterName
{
    public string FirstName;
    public string LastName;
    public bool HasLastName;

    public CharacterName(string firstName)
    {
        FirstName = firstName;
        HasLastName = false;
    }

    public CharacterName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        HasLastName = true;
    }

    public CharacterName(string[] fullName)
    {
        if (fullName.Length != 2) Logger.Error(Logger.Character, "Character name constructor received too many arguments ({0})", fullName.Length);

        FirstName = fullName[0];
        LastName = fullName[1];
        HasLastName = true;
    }
}
