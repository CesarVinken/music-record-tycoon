using System.Collections.Generic;

public class ObjectInteraction
{
    public ObjectInteractionType ObjectInteractionType;
    public string Name; // the label that is displayed in game
    public ObjectInteractionCharacterRole CharacterRole;
    public List<InteractionStep> InteractionSteps = new List<InteractionStep>();

    private ObjectInteraction(
        ObjectInteractionType objectInteractionType,
        string name, 
        ObjectInteractionCharacterRole characterRole
    )
    {
        ObjectInteractionType = objectInteractionType;
        Name = name;
        CharacterRole = characterRole;
    }

    public static ObjectInteraction Create(ObjectInteractionType objectInteractionType,
        string name,
        ObjectInteractionCharacterRole characterRole = ObjectInteractionCharacterRole.NoCharacter)
    {
        return new ObjectInteraction(objectInteractionType, name, characterRole);
    }

    public ObjectInteraction AddInteractionStep(InteractionStep interactionStep)
    {
        InteractionSteps.Add(interactionStep);
        return this;
    }

    public ObjectInteraction SetObjectInteractionCharacterRole(ObjectInteraction objectInteraction, ObjectInteractionCharacterRole characterRole)
    {
        objectInteraction.CharacterRole = characterRole;
        return objectInteraction;
    }
}
