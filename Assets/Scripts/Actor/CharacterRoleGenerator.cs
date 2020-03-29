
using System;

public class CharacterRoleGenerator
{
    public static Role Generate()
    {
        Array values = Enum.GetValues(typeof(Role));
        Random random = new Random();
        Role randomRole = (Role)values.GetValue(random.Next(values.Length));
        return randomRole;
    }
}
