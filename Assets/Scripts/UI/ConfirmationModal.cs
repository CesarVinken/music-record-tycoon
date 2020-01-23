﻿using UnityEngine;

public enum BuildAction
{
    CreateRoomPlot,
    DeleteRoom
}

public class ConfirmationModal : MonoBehaviour  // Later on maybe turn this in to a script for a specific confirmation modal that is attached to the confirmationModal GO.
{
    public static ConfirmationModal CurrentConfirmationModal;
    //private BuildingPlot _roomBuildPlot;
    public DeleteRoomTrigger DeleteRoomTrigger;
    private BuildAction _buildAction;
    private Vector2 _midpoint = new Vector2(0, 0);

    //public void Setup(BuildingPlot roomBuildPlot)
    //{
    //    if (CurrentConfirmationModal) CurrentConfirmationModal.DestroyConfirmationModal();

    //    CurrentConfirmationModal = this;
    //    _buildAction = BuildAction.CreateRoomPlot;
    //    _roomBuildPlot = roomBuildPlot;
    //}

    public void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);
    }

    public void Setup(DeleteRoomTrigger deleteRoomTrigger, Vector2 midpoint)
    {
        if (CurrentConfirmationModal)
        {
            CurrentConfirmationModal.ResetDeleteTrigger();
            CurrentConfirmationModal.DestroyConfirmationModal();
        }
        CurrentConfirmationModal = this;
        _buildAction = BuildAction.DeleteRoom;
        DeleteRoomTrigger = deleteRoomTrigger;
        _midpoint = midpoint;
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
                DeleteRoomTrigger.DeleteRoom();
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
        DeleteRoomTrigger.gameObject.SetActive(true);
    }

    public void HideConfirmationModal()
    {
        gameObject.SetActive(false);
    }

    public void ShowConfirmationModal()
    {
        gameObject.SetActive(true);
    }
}
