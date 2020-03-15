
//The base object for the blueprint versions of rooms or objects. The blueprints will provide the needed data for the objects in the game, the object representations in the UI etc.
public abstract class BuildItemBlueprint
{
    public string Name = "";
    public string Description = "";

    public BuildItemBlueprint(string name, string description)
    {
        Guard.CheckIsEmptyString("Name", name);
        Guard.CheckIsEmptyString("Description", description);

        Name = name;
        Description = description;
    }
}
