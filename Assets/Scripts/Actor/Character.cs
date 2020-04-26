using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Room CurrentRoom;

    public string Id;
    public CharacterName Name;
    public int Age;
    public Gender Gender;
    public string Image;

    public NavActor NavActor;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterAnimationHandler CharacterAnimationHandler;

    public CharacterActionState CharacterActionState;

    public List<ObjectInteractionType> PossibleObjectInteractions = new List<ObjectInteractionType>();

    public List<Song> RecordedSongs = new List<Song>();  // TODO make visible in inspector

    void Awake()
    {
        PlayerLocomotion = gameObject.AddComponent<CharacterLocomotion>();
        PlayerLocomotion.Character = this;

        CharacterAnimationHandler = gameObject.AddComponent<CharacterAnimationHandler>();

        if (PlayerLocomotion == null)
            Logger.Log(Logger.Initialisation, "Could not find PlayerLocomotion component on character");
        if (CharacterAnimationHandler == null)
            Logger.Log(Logger.Initialisation, "Could not find CharacterAnimationHandler component on character");
    }

    public void Setup(CharacterName name, int age, Gender gender, string image)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Age = age;
        Gender = gender;
        Image = image;

        SetCharacterActionState(CharacterActionState.Idle);
    }

    public void EnterRoom(Room newRoom)
    {
        Room previousRoom = CurrentRoom;

        CurrentRoom = newRoom;
        Logger.Log("Lower the walls in new room");

        newRoom.LowerWallPieces();
    }

    public void LeaveRoom()
    {
        CurrentRoom = null;
    }

    public void SetCharacterActionState(CharacterActionState newState)
    {
        Logger.Log(Logger.Character, "CharacterActionState of {0}({1}) set to {2}", Name, Id, newState);
        CharacterActionState = newState;
    }
}
