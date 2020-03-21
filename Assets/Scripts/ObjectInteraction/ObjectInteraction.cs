
using UnityEngine;

public class ObjectInteraction
{
    public ObjectInteractionType ObjectInteractionType;
    public string Name; // the label that is displayed in game
    public string Reaction; // string reaction. Temporary: should be more sophisticated later (eg. multiple strings in an animated sequence)
    public ObjectInteractionLocationType ObjectInteractionLocationType;

    public ObjectInteraction(
        ObjectInteractionType objectInteractionType,
        string name, 
        string reaction,
        ObjectInteractionLocationType objectInteractionLocationType = ObjectInteractionLocationType.AtRoomObject
    )
    {
        ObjectInteractionType = objectInteractionType;
        Name = name;
        Reaction = reaction;
        ObjectInteractionLocationType = objectInteractionLocationType;
    }

    public ObjectInteraction SetObjectInteractionLocationType(ObjectInteraction objectInteraction, ObjectInteractionLocationType objectInteractionLocationType)
    {
        objectInteraction.ObjectInteractionLocationType = objectInteractionLocationType;
        return objectInteraction;
    }
}
