using System.Collections.Generic;
using System.Threading.Tasks;
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
        GameObject modal = GameManager.Instance.InstantiatePrefab(
            MainCanvas.Instance.ConfirmationModalPrefab,
            MainCanvas.Instance.TriggersContainer.transform,
            Camera.main.WorldToScreenPoint(_midpoint));

        modal.GetComponent<ConfirmationModal>().Setup(this, _midpoint);

        gameObject.SetActive(false);
    }

    public async Task DeleteRoom()
    {
        BuilderManager.Instance.DeleteRoom(_room);
        await CharacterManager.Instance.UpdatePathfindingGrid();
        return;
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
