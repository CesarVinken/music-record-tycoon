using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Room CurrentRoom;

    public string Id;
    public CharacterName CharacterName;
    public int Age;
    public Gender Gender;
    public string Image;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterAnimationHandler CharacterAnimationHandler;

    public CharacterActionState CharacterActionState { get { return _characterActionState; } private set { _characterActionState = value; } }
    [SerializeField]
    private CharacterActionState _characterActionState = 0;

    public List<ObjectInteractionType> PossibleObjectInteractions = new List<ObjectInteractionType>();

    public List<Song> RecordedSongs = new List<Song>();  // TODO make visible in inspector
    public CharacterRoutine PlannedRoutine;

    public void Awake()
    {
        PlayerLocomotion = gameObject.GetComponent<CharacterLocomotion>();
        PlayerLocomotion.Character = this;

        CharacterAnimationHandler = gameObject.AddComponent<CharacterAnimationHandler>();

        if (PlayerLocomotion == null)
            Logger.Log(Logger.Initialisation, "Could not find PlayerLocomotion component on character");
        if (CharacterAnimationHandler == null)
            Logger.Log(Logger.Initialisation, "Could not find CharacterAnimationHandler component on character");

        PlannedRoutine = new CharacterRoutine(this);
    }

    public void Start()
    {
        InvokeRepeating("UpdatePlannedRoutine", .0001f, 6.0f);
    }

    public async void UpdatePlannedRoutine()
    {
        if (CharacterActionState != CharacterActionState.Idle)
            return;

        if (PlannedRoutine.RoutineTasks.Count == 0)
        {
            PlannedRoutine.TryGetNewRoutineTask();
            if (PlannedRoutine.RoutineTasks.Count == 0) return;
        }

        PlannedRoutine.InRoutine = true;
        await PlannedRoutine.RoutineTasks[0].Run();
        if(PlannedRoutine.InRoutine)
        {
            PlannedRoutine.InRoutine = false;
            CharacterActionState = CharacterActionState.Idle;
            if(PlannedRoutine.RoutineTasks.Count > 0)
            {
                PlannedRoutine.RoutineTasks.RemoveAt(0);
            }
        }
    }

    public void Setup(CharacterName name, int age, Gender gender, string image)
    {
        Id = Guid.NewGuid().ToString();
        CharacterName = name;
        Age = age;
        Gender = gender;
        Image = image;

        SetCharacterActionState(CharacterActionState.Idle);
    }

    public void EnterRoom(Room newRoom)
    {
        CurrentRoom = newRoom;
        Logger.Log(Logger.Locomotion, "Lower the walls in new room");

        newRoom.LowerWallPieces();
    }

    public void LeaveRoom()
    {
        CurrentRoom = null;
    }

    public void SetCharacterActionState(CharacterActionState newState)
    {
        if (CharacterActionState == newState) 
            return;

        if (CharacterActionState == CharacterActionState.RoutineAction) 
            PlannedRoutine.InterruptRoutine();

        Logger.Log(Logger.Character, "CharacterActionState of {0} set from {1} to {2}", FullName(), CharacterActionState, newState);
        CharacterActionState = newState;
    }
    
    public string FullName()
    {
        return CharacterNameGenerator.GetName(CharacterName);
    }

}
