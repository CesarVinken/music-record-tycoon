using UnityEngine;

public class Guard
{
    public static void CheckIsNull(GameObject gameObject, string name)
    {
        if(gameObject == null)
            Debug.LogError("Cannot Find " + name);
    }
}
