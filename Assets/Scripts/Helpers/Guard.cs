using UnityEngine;

public class Guard
{
    public static void CheckIsNull(GameObject gameObject, string name)
    {
        if(gameObject == null)
            Logger.Error("Cannot Find " + name);
    }

    public static void CheckIsNull<T>(T gameObject, string name) where T : MonoBehaviour
    {
        Debug.Log(gameObject);
        if (gameObject == null)
            Logger.Error("Cannot Find " + name);
    }

    public static void CheckIsNull(SpriteRenderer spriteRenderer, string name)
    {
        if (spriteRenderer == null)
            Logger.Error("Cannot Find " + name);
    }

    public static void CheckIsNull(Sprite sprite, string name)
    {
        if (sprite == null)
            Logger.Error("Cannot Find " + name);
    }
}
