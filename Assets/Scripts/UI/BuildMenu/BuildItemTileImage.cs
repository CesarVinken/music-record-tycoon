using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildItemTileImage : MonoBehaviour, IPointerClickHandler
{
    public BuildItemTile BuildItemTile;

    public void Awake()
    {
        Guard.CheckIsNull(BuildItemTile, "BuildItemTile");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Logger.Log("on image");
        RoomBlueprint blueprint = new RoomBlueprint();
        BuilderManager.Instance.DrawAvailablePlots(blueprint);
    }
}
