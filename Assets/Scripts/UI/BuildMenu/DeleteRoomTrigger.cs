using System.Collections.Generic;
using UnityEngine;

public class DeleteRoomTrigger : MonoBehaviour
{
    private Vector2 _midpoint = new Vector2(0, 0);
    private Room _room;

    public static List<DeleteRoomTrigger> DeleteRoomTriggers = new List<DeleteRoomTrigger>();

    public void Awake()
    {
        DeleteRoomTriggers.Add(this);
    }

    public void Setup(Room room)
    {
        _room = room;
        _room.SetDeleteRoomTrigger(this);
        _midpoint = _room.RoomCorners[Direction.Down] + (_room.RoomCorners[Direction.Up] - _room.RoomCorners[Direction.Down]) / 2;
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);
    }

    public void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);
    }

    public void CreateDeleteRoomConfirmation()
    {
        GameObject modal = Instantiate(MainCanvas.Instance.ConfirmationModalPrefab);
        modal.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
        modal.transform.SetParent(MainCanvas.Instance.TriggersContainer.transform);

        modal.GetComponent<ConfirmationModal>().Setup(this, _midpoint);

        gameObject.SetActive(false);
    }

    public void DeleteRoom()
    {
        Room tempRoomCopy = _room;
        _room.RemoveDoorConnectionFromAdjacentRooms();
        _room.RemoveThisRoomFromAdjacentRooms();
        _room.DeleteRoom();
        Logger.Warning(Logger.Building, "Deleting room: {0}", tempRoomCopy.Id);
        tempRoomCopy.CleanUpDeletedRoomTiles();

        CharacterManager.Instance.UpdatePathfindingGrid();

        for (int l = 0; l < RoomManager.Rooms.Count; l++)
        {
            Logger.Log(Logger.Building, "{0} has {1} adjacent rooms", RoomManager.Rooms[l].Id, RoomManager.Rooms[l].AdjacentRooms.Count);
        }
    }

    public static void DeleteAllDeleteRoomTriggers()
    {
        for (int i = 0; i < DeleteRoomTriggers.Count; i++)
        {
            DeleteDeleteRoomTrigger(DeleteRoomTriggers[i]);
        }
        DeleteRoomTriggers.Clear();
    }

    private static void DeleteDeleteRoomTrigger(DeleteRoomTrigger deleteRoomTrigger)
    {
        if (deleteRoomTrigger._room) deleteRoomTrigger._room.SetDeleteRoomTrigger(null);

        Destroy(deleteRoomTrigger.gameObject);
    }

    public void HideDeleteRoomTrigger()
    {
        gameObject.SetActive(false);
        if (ConfirmationModal.CurrentConfirmationModal)
        {
            ConfirmationModal.CurrentConfirmationModal.HideConfirmationModal();
        }
    }

    public void ShowDeleteRoomTrigger()
    {
        if (ConfirmationModal.CurrentConfirmationModal)
        {
            if(ConfirmationModal.CurrentConfirmationModal.DeleteRoomTrigger == this)
            {
                ConfirmationModal.CurrentConfirmationModal.ShowConfirmationModal();
            } else
            {
                gameObject.SetActive(true);
            }
        } 
        else
        {
            gameObject.SetActive(true);
        }
    }
}
