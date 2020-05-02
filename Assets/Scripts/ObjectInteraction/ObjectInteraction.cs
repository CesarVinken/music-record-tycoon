using System.Collections.Generic;

public class ObjectInteraction
{
    public ObjectInteractionType ObjectInteractionType;
    public string Name; // the label that is displayed in game
    public ObjectInteractionCharacterRole CharacterRole;
    public List<InteractionStep> InteractionSteps = new List<InteractionStep>();

    public ObjectInteraction(
        ObjectInteractionType objectInteractionType,
        string name, 
        ObjectInteractionCharacterRole characterRole = ObjectInteractionCharacterRole.NoCharacter
    )
    {
        ObjectInteractionType = objectInteractionType;
        Name = name;
        CharacterRole = characterRole;
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
