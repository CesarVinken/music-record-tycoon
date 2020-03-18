
using UnityEngine;

public class ObjectInteraction
{
    public ObjectInteractionType ObjectInteractionType;
    public string Name; // the label that is displayed in game
    public string Reaction; // string reaction. Temporary: should be more sophisticated later (eg. multiple strings in an animated sequence)
    public RoomObject RoomObject;

    public ObjectInteraction(ObjectInteractionType objectInteractionType, string name, string reaction)
    {
        ObjectInteractionType = objectInteractionType;
        Name = name;
        Reaction = reaction;
    }
}
