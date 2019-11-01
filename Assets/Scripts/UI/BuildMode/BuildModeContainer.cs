using System.Collections.Generic;
using UnityEngine;

public class BuildModeContainer : MonoBehaviour
{
    public static BuildModeContainer Instance;
    public List<RoomBuildPlot> RoomBuildPlots = new List<RoomBuildPlot>();

    void Awake()
    {
        Instance = this;
        RoomBuildPlots = new List<RoomBuildPlot>();
    }

    public void CreateBuildingPlot(GameObject buildingPlot, Room room, Vector2 startingPoint)
    {
        RoomBuildPlot plot = Instantiate(buildingPlot, transform).GetComponent<RoomBuildPlot>();
        RoomBuildPlots.Add(plot);

        plot.Setup(room, startingPoint);
    }

    public void DestroyBuildingPlots()
    {
        foreach (RoomBuildPlot plot in RoomBuildPlots)
        {
            plot.DestroySelf();
        }
        RoomBuildPlots.Clear();
    }
}
