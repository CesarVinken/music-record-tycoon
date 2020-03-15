
using UnityEngine;

public class ObjectInteraction
{
    public ObjectInteractionName ObjectInteractionName;
    public string Name; // the label that is displayed in game
    public string Reaction; // string reaction. Temporary: should be more sophisticated later (eg. multiple strings in an animated sequence)

    public ObjectInteraction(ObjectInteractionName objectInteractionName, string name, string reaction)
    {
        ObjectInteractionName = objectInteractionName;
        Name = name;
        Reaction = reaction;
    }
}
