using System;

public static class CharacterAgeGenerator
{
    public static int Generate()
    {
        Random random = new Random();
        int randomNumber = random.Next(18, 50);

        return randomNumber;
    }
}
