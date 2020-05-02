using UnityEngine;
using UnityEngine.UI;

public class BuildItemTile : MonoBehaviour
{
    public IBuildItemBlueprint BuildItemBlueprint;
    public string Image = "";

    public Text NameTextComponent;
    public Text DescriptionTextComponent;

    public void Setup(IBuildItemBlueprint buildItemBlueprint)
    {
        BuildItemBlueprint = buildItemBlueprint;

        Guard.CheckIsNull(NameTextComponent, "NameTextComponent");
        Guard.CheckIsNull(DescriptionTextComponent, "DescriptionTextComponent");

        NameTextComponent.text = BuildItemBlueprint.Name;
        DescriptionTextComponent.text = BuildItemBlueprint.Description;
    }
}
