using UnityEngine;
using UnityEngine.EventSystems;

public static class PointerHelper
{
    public static bool IsPointerOverGameObject()
    {
        if (GameManager.Instance.CurrentPlatform == Platform.PC) return EventSystem.current.IsPointerOverGameObject();

        if (Input.touchCount > 0) return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return false;
    }
}
