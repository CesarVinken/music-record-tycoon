using UnityEngine;

public class ConfirmationModal : MonoBehaviour  // Later on maybe turn this in to a script for a specific confirmation modal that is attached to the confirmationModal GO.
{
    public static ConfirmationModal CurrentConfirmationModal;
    private RoomBuildPlot _roomBuildPlot;

    public void Setup(RoomBuildPlot roomBuildPlot)
    {
        CurrentConfirmationModal = this;
        _roomBuildPlot = roomBuildPlot;
    }

    public void Confirm()
    {
        _roomBuildPlot.Build();
        DestroyConfirmationModal();
        BuildModeContainer.Instance.DestroyBuildingPlots();
    }

    public void Cancel()
    {
        DestroyConfirmationModal();
    }

    public void DestroyConfirmationModal()
    {
        Destroy(gameObject);
    }
}
