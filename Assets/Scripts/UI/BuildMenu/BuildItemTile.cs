using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildItemTile : MonoBehaviour
{
    public string Name = "";
    public string Description = "";
    public string Image = "";

    public Text NameTextComponent;
    public Text DescriptionTextComponent;

    public void Setup(string name, string description)
    {
        Name = name;
        Description = description;

        Guard.CheckIsNull(NameTextComponent, "NameTextComponent");
        Guard.CheckIsNull(DescriptionTextComponent, "DescriptionTextComponent");

        NameTextComponent.text = Name;
        DescriptionTextComponent.text = Description;
    }
}
