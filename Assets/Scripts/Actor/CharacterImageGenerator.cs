using System;

public static class CharacterImageGenerator
{
    public static string Generate(Gender gender)
    {
        int avatarCount = GetAvatarCount(gender);

        Random random = new Random();
        int randomNumber = random.Next(avatarCount);

        string avatarStack = getAvatarStack(gender);

        if (gender == Gender.Male)
            return "TestAvatarsMale_" + randomNumber;

        return "TestAvatarsFemale_" + randomNumber;
    }

    public static int GetAvatarCount(Gender gender)
    {
        if (gender == Gender.Male)
            return CharacterManager.Instance.AvatarsMale.Length;
        else
            return CharacterManager.Instance.AvatarsFemale.Length;
    }

    public static string getAvatarStack(Gender gender)
    {
        if (gender == Gender.Male)
            return "TestAvatarsMale_";
        else
            return "TestAvatarsFemale_";
    }
}
