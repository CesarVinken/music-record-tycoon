using System.Collections.Generic;
using UnityEngine;

public class BuildMenuWorldSpaceContainer : MonoBehaviour
{
    public static BuildMenuWorldSpaceContainer Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CreateBuildingPlot(GameObject buildingPlot, RoomBlueprint roomBlueprint, Vector2 startingPoint, ObjectRotation roomRotation)
    {
        if (BuilderManager.Instance.BuildingPlots.ContainsKey(startingPoint)) return;

        BuildingPlot plot = Instantiate(buildingPlot, transform).GetComponent<BuildingPlot>();
        BuilderManager.Instance.BuildingPlots.Add(startingPoint, plot);
        //Logger.Log("The starting points for this plot should be {0}", startingPoint);

        plot.Setup(roomBlueprint, startingPoint, roomRotation);
    }

    public void DestroyBuildingPlots()
    {
        foreach (KeyValuePair<Vector2, BuildingPlot> plot in BuilderManager.Instance.BuildingPlots)
        {
            plot.Value.DestroySelf();
        }
        BuilderManager.Instance.BuildingPlots.Clear();
        BuilderManager.Instance.BuildingPlotLocations.Clear();
    }

}
