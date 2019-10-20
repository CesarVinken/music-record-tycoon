using UnityEngine;

public class BuildModeContainer : MonoBehaviour
{
    public static BuildModeContainer Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CreateBuildingPlot(GameObject buildingPlot)
    {
        Instantiate(buildingPlot, transform);
    }

    public void DestroyBuildingPlots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
