using UnityEngine;

public class DeleteRoomModeButton : MonoBehaviour
{
    public void EnterDeleteRoomMode()
    {
        BuilderManager.Instance.EnterDeleteRoomMode();
    }
}
