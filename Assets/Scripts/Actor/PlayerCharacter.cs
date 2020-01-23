using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public Transform NavTransform;
    public NavActor PlayerNav;

    public CharacterActionState CharacterActionState;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterAnimationHandler CharacterAnimationHandler;

    public Room CurrentRoom;
    public string Id;

    void Awake()
    {
        Instance = this;
        Id = "Player"; // TODO: Make generic!

        if (NavTransform == null)
            Logger.Log(Logger.Initialisation, "Could not find NavTransform on character");
        if (PlayerNav == null)
            Logger.Log(Logger.Initialisation, "Could not find PlayerNav on character");

        PlayerLocomotion = gameObject.AddComponent<CharacterLocomotion>();
        CharacterAnimationHandler = gameObject.AddComponent<CharacterAnimationHandler>();
        SetCharacterActionState(CharacterActionState.Idle);
    }

    public void SetCharacterActionState(CharacterActionState newState)
    {
        Logger.Log(Logger.Character, "CharacterActionState set to {0}", newState);
        CharacterActionState = newState;
    }

    public void EnterRoom(Room newRoom)
    {
        Room previousRoom = CurrentRoom;

        if(previousRoom != null && previousRoom != newRoom)
        {
            //Logger.Log("Raise the walls");
            previousRoom.RaiseWallPieces(newRoom);
        }

        CurrentRoom = newRoom;
        //Logger.Log("Lower the walls");

        newRoom.LowerWallPieces();
    }

//    public void LeaveRoom()
//    {
//        CurrentRoom = null;
//    }
}
