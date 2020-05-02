using UnityEngine;

public interface IBuildItemBlueprint
{
    string Name { get; set; }               // the name in the building menu
    string Description { get; set; }        // the description in the building menu
    int Price { get; set; }
}
