
using UnityEngine;

public class ObjectInteraction
{
    public ObjectInteractionType ObjectInteractionType;
    public string Name; // the label that is displayed in game
    public string Reaction; // string reaction. Temporary: should be more sophisticated later (eg. multiple strings in an animated sequence)
    public ObjectInteractionCharacterRole CharacterRole;

    public ObjectInteraction(
        ObjectInteractionType objectInteractionType,
        string name, 
        string reaction,
        ObjectInteractionCharacterRole characterRole = ObjectInteractionCharacterRole.NoCharacter
    )
    {
        ObjectInteractionType = objectInteractionType;
        Name = name;
        Reaction = reaction;
        CharacterRole = characterRole;
    }

    public ObjectInteraction SetObjectInteractionCharacterRole(ObjectInteraction objectInteraction, ObjectInteractionCharacterRole characterRole)
    {
        objectInteraction.CharacterRole = characterRole;
        return objectInteraction;
    }
}
