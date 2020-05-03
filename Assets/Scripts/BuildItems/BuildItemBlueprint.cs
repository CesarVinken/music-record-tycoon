
//The base object for the blueprint versions of rooms or objects. The blueprints will provide the needed data for the objects in the game, the object representations in the UI etc.
public abstract class BuildItemBlueprint<T> : IBuildItemBlueprint
{
    public string Name { get; set; }        // the name in the building menu
    public string Description { get; set; } // the description in the building menu
    public int Price { get; set; }

    protected abstract T WithName(string name);
    protected abstract T WithMenuDescription(string description);
    protected abstract T WithPrice(int price);
}
