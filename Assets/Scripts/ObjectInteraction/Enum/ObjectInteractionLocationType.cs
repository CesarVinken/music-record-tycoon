public enum ObjectInteractionCharacterRole
{
    NoCharacter,            // Player triggers an interaction that does not require any character
    CharacterAtRoomObject,  // Player selects character for interaction, character will do interaction in front of room object
    CharacterInRoom         // Player selects character for interaction, character will do interaction as soon as they enter the room
}