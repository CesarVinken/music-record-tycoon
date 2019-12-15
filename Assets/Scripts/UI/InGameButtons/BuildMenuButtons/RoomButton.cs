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
        //Room room = RoomPrefab.GetComponent<Room>();    //Temporary!
        RoomBlueprint blueprint = new RoomBlueprint();
        //GameManager.Instance.BuilderManager.SetSelectedRoom(blueprint);
    }
}
