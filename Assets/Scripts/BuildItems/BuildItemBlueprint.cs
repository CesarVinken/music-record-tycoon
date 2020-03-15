
//The base object for the blueprint versions of rooms or objects. The blueprints will provide the needed data for the objects in the game, the object representations in the UI etc.
public abstract class BuildItemBlueprint
{
    public string Name = "";        // the name in the building menu
    public string Description = ""; // the description in the building menu

    public BuildItemBlueprint(string name, string description)
    {
        Guard.CheckIsEmptyString("Name", name);
        Guard.CheckIsEmptyString("Description", description);

        Name = name;
        Description = description;
    }
}
