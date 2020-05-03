using System;

public class Util
{
    static readonly Random rand = new Random(); 
    public static T GetRandomValue<T>(T[] values)
    {
        lock (rand)
        {
            return values[rand.Next(values.Length)];
        }
    }

    public static Random InitRandomNumber()
    {
        Random random = new Random(DateTime.Now.Millisecond);
        return random;
    }
}
