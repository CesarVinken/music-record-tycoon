using UnityEngine;

public class DeleteObjectMode : MonoBehaviour
{
    public void ToggleDeleteMode()
    {
        if (BuilderManager.InDeleteObjectMode)
        {
            BuilderManager.Instance.DeactivateDeleteRoomMode();
        }
        else
        {
            BuilderManager.Instance.ActivateDeleteRoomMode();
        }
    }
}
