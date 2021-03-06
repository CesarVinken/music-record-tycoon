﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterRoutineTask
{
    public Character Character;
    public CharacterRoutineTypeName CharacterRoutineTypeName;
    public int Duration;
    public RoomObjectGO TaskLocationRoomObject = null;

    public CharacterRoutineTask(CharacterRoutineTypeName routineTypeName, Character character)
    {
        CharacterRoutineTypeName = routineTypeName;
        Character = character;

        // get all possible room object types for routineTypeName
        List<RoomObjectName> roomObjectNames = GetRoomObjectsForTask(routineTypeName);

        if(roomObjectNames.Count == 0) // routines that do not require an object to interact with, such as Idle animations
        {
            Logger.Log(Logger.Interaction, "This character routine does not have an task location room object: {0}", routineTypeName);
            return;
        }

        // get all room objects of that possible types that are on the map
        List<RoomObjectGO> roomObjectsGOs = new List<RoomObjectGO>();
        for (int i = 0; i < roomObjectNames.Count; i++)
        {
            roomObjectsGOs = RoomManager.RoomObjectGOs.Where(
                roomObjectGO => 
                {
                    return roomObjectGO.RoomObject.RoomObjectName == roomObjectNames[i] && // find all room objects of correct type of task
                    roomObjectGO.InteractingCharacter == null;  // only pick room objects that are available
                }
                ).ToList();
        }

        if (roomObjectsGOs.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, roomObjectsGOs.Count);
            TaskLocationRoomObject = roomObjectsGOs[random];
        }
    }

    public static CharacterRoutineTask CreateCharacterRoutineTask(CharacterRoutineTypeName routineTypeName, Character character)
    {
        CharacterRoutineTask characterRoutineTask = new CharacterRoutineTask(routineTypeName, character)
            .WithDuration();
        return characterRoutineTask;
    }

    public static CharacterRoutineTask CreateCharacterRoutineTask(Character character)
    {
        CharacterRoutineTypeName routineTypeName = GetRandomRoutineType(character);
        CharacterRoutineTask characterRoutineTask = new CharacterRoutineTask(routineTypeName, character)
            .WithDuration();
            
        return characterRoutineTask;
    }

    private CharacterRoutineTask WithDuration(int duration = 3000)
    {
        Duration = duration;
        return this;
    }

    public async Task Run()
    {
        if(TaskLocationRoomObject != null)
        {
            bool travellingToLocation = false;
            Vector2 targetLocation = TaskLocationRoomObject.RoomObjectInteractionLocation.transform.position;
            if (Vector2.Distance(Character.transform.position, targetLocation) > CharacterManager.MinDistanceForInteraction)
            {
                Character.PlayerLocomotion.SetLocomotionTarget(targetLocation);
                travellingToLocation = true;
            }

            // go to location
            while (travellingToLocation)
            {
                await Task.Delay(250);

                if (TaskLocationRoomObject == null || TaskLocationRoomObject.InteractingCharacter != null) // EG the room object targetted for routine does not exist anymore or is in use
                {
                    Logger.Warning("The room object targetted for routine does not exist anymore or is already in use.");
                    travellingToLocation = false;
                    Character.PlannedRoutine.InterruptRoutine();

                    Character.SetCharacterActionState(CharacterActionState.Idle);
                    Character.CharacterAnimationHandler.SetLocomotion(false);

                    return;
                }
                if (Character.PlayerLocomotion.Target != targetLocation)
                {
                    Character.PlannedRoutine.InterruptRoutine();
                    travellingToLocation = false;
                    return;
                }
                if (Character.CharacterActionState != CharacterActionState.Moving)
                {
                    travellingToLocation = false;
                }
            }

            TaskLocationRoomObject.SetInteractingCharacter(Character);
        }

        Character.CharacterAnimationHandler.SetLocomotion(false);

        //Execution starts
        Character.SetCharacterActionState(CharacterActionState.RoutineAction);
        CharacterTextDisplayContainer.Instance.EnableCharacterRoutineText(Character, CharacterRoutineTypeName.ToString());
        Logger.Log(Logger.Character, "{0} is now doing {1}", Character.FullName(), CharacterRoutineTypeName);
        await Task.Delay(Duration); // it should be possible to interrupt a routine to start an object interaction or to just walk to another place
        CharacterTextDisplayContainer.Instance.DisableCharacterRoutineText(Character);

        if (TaskLocationRoomObject != null)
        {
            TaskLocationRoomObject.UnsetInteractingCharacter(Character);
        }
        return;
    }

    private static CharacterRoutineTypeName GetRandomRoutineType(Character character)
    {
        List<CharacterRoutineType> availableCharacterRoutineTypes = new List<CharacterRoutineType>();

        // filter out available routines that are not suitable for the character's role
        for (int i = 0; i < RoutineManager.AvailableRoutineTypes.Count; i++)
        {
            CharacterRoutineType characterRoutineType = RoutineManager.AvailableRoutineTypes.ElementAt(i).Value;
            if (characterRoutineType.CharacterRoles.Count == 0 || characterRoutineType.CharacterRoles.Contains(character.Role)) {
                availableCharacterRoutineTypes.Add(characterRoutineType);
            }
        }

        // weight chances for routines based on the time of day
        float[] weightedIndeces = availableCharacterRoutineTypes.Select(routine => {
            float originalWeight = routine.ProbabilityWeight;
            float adjustedWeight = routine.ApplyTimeProbabilityMultiplier(originalWeight);
            return adjustedWeight;
            }).ToArray();
        //for (int p = 0; p < weightedIndeces.Length; p++)
        //{
        //    Logger.Log("adjusted weightedIndex {0} is {1}", p, weightedIndeces[p]);
        //}
        //for (int q = 0; q < availableCharacterRoutineTypes.Count; q++)
        //{
        //    Logger.Log("availableCharacterRoutineTypes name{0} is {1}", q, availableCharacterRoutineTypes[q].Name);
        //}
        int randomWeightIndex = Util.GetRandomWeightedIndex(weightedIndeces);

        CharacterRoutineTypeName randomCharacterRoutineType = availableCharacterRoutineTypes[randomWeightIndex].Name;

        return randomCharacterRoutineType;
    }

    private List<RoomObjectName> GetRoomObjectsForTask(CharacterRoutineTypeName routineTypeName)
    {
        if(!RoutineManager.AvailableRoutineTypes.ContainsKey(routineTypeName)) {
            Logger.Error("Could not find routine {0} among the available routine tasks", routineTypeName);
        }
        CharacterRoutineType characterRoutineType = RoutineManager.AvailableRoutineTypes[routineTypeName];

        return characterRoutineType.RoomObjects;
    }
}
