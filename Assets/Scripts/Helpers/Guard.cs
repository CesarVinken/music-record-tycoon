using UnityEngine;

public class Guard
{
    public static void CheckIsNull(GameObject gameObject, string name)
    {
        if(gameObject == null)
            Logger.Error("Cannot Find {0}", name);
    }

    public static void CheckIsNull<T>(T gameObject, string name) where T : MonoBehaviour
    {
        if (gameObject == null)
            Logger.Error("Cannot Find {0}", name);
    }

    public static void CheckIsNull(SpriteRenderer spriteRenderer, string name)
    {
        if (spriteRenderer == null)
            Logger.Error("Cannot Find {0}", name);
    }

    public static void CheckIsNull(Sprite sprite, string name)
    {
        if (sprite == null)
            Logger.Error("Cannot Find {0}", name);
    }

    public static void CheckIsEmptyString(string name, string content)
    {
        if(content == "")
            Logger.Error("{0} cannot be an empty string", name);
    }
}
