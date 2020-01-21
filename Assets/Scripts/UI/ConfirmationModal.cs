using UnityEngine;

public enum BuildAction
{
    CreateRoomPlot,
    DeleteRoom
}

public class ConfirmationModal : MonoBehaviour  // Later on maybe turn this in to a script for a specific confirmation modal that is attached to the confirmationModal GO.
{
    public static ConfirmationModal CurrentConfirmationModal;
    //private BuildingPlot _roomBuildPlot;
    private DeleteRoomTrigger _deleteRoomTrigger;
    private BuildAction _buildAction;

    //public void Setup(BuildingPlot roomBuildPlot)
    //{
    //    if (CurrentConfirmationModal) CurrentConfirmationModal.DestroyConfirmationModal();

    //    CurrentConfirmationModal = this;
    //    _buildAction = BuildAction.CreateRoomPlot;
    //    _roomBuildPlot = roomBuildPlot;
    //}

    public void Setup(DeleteRoomTrigger deleteRoomTrigger)
    {
        if (CurrentConfirmationModal)
        {
            CurrentConfirmationModal.ResetDeleteTrigger();
            CurrentConfirmationModal.DestroyConfirmationModal();
        }
        CurrentConfirmationModal = this;
        _buildAction = BuildAction.DeleteRoom;
        _deleteRoomTrigger = deleteRoomTrigger;
    }

    public void Confirm()
    {
        switch (_buildAction)
        {
            //case BuildAction.CreateRoomPlot:
            //    _roomBuildPlot.Build();
            //    BuildMenuWorldSpaceContainer.Instance.DestroyBuildingPlots();
            //    break;
            case BuildAction.DeleteRoom:
                _deleteRoomTrigger.DeleteRoom();
                DeleteRoomTrigger.DeleteAllDeleteRoomTriggers();
                break;
            default:
                Logger.Error(Logger.UI, "The buildAction {0} is not yet implemented", _buildAction);
                break;
        }

        DestroyConfirmationModal();
    }

    //public void Cancel()
    //{
    //    DestroyConfirmationModal();
    //}

    public void DestroyConfirmationModal()
    {
        if (CurrentConfirmationModal == this) CurrentConfirmationModal = null;

        Destroy(gameObject);
    }

    public void ResetDeleteTrigger()
    {
        _deleteRoomTrigger.gameObject.SetActive(true);
    }
}
