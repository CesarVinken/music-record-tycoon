using UnityEngine;

public class RoomButton : MonoBehaviour
{
    public GameObject RoomPrefab;

    public void Awake()
    {
        RoomPrefab = GameManager.Instance.BuilderManager.Room1Prefab;
    }

    public void SelectRoom()
    {
        GameManager.Instance.BuilderManager.SetSelectedRoom(RoomPrefab);
    }
}
