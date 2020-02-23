using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Room CurrentRoom;

    public string Id;
    public string Name;
    public int Age;
    public Gender Gender;
    public string Image;

    public NavActor NavActor;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterAnimationHandler CharacterAnimationHandler;

    public CharacterActionState CharacterActionState;

    void Awake()
    {
        if (PlayerLocomotion == null)
            Logger.Log(Logger.Initialisation, "Could not find PlayerLocomotion component on character");
        if (CharacterAnimationHandler == null)
            Logger.Log(Logger.Initialisation, "Could not find CharacterAnimationHandler component on character");

        SetCharacterActionState(CharacterActionState.Idle);
    }

    public void Setup(string name, int age, Gender gender, string image)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Age = age;
        Gender = gender;
        Image = image;
    }

    public void EnterRoom(Room newRoom)
    {
        Room previousRoom = CurrentRoom;

        if (previousRoom != null && previousRoom != newRoom)
        {
            Logger.Log("Raise the walls");
            previousRoom.RaiseWallPieces(newRoom);
        }

        CurrentRoom = newRoom;
        Logger.Log("Lower the walls");

        newRoom.LowerWallPieces();
    }

    public void LeaveRoom()
    {
        CurrentRoom = null;
    }

    public void SetCharacterActionState(CharacterActionState newState)
    {
        Logger.Log(Logger.Character, "CharacterActionState of {0} set to {1}", Id, newState);
        CharacterActionState = newState;
    }
}
