
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterRoutineTask
{
    public Character Character;
    public CharacterRoutineType CharacterRoutineType;
    public int Duration;
    public RoomObjectGO TaskLocationRoomObject = null;

    public CharacterRoutineTask(CharacterRoutineType routineType, Character character)
    {
        CharacterRoutineType = routineType;
        Character = character;

        // get room object type for routineType
        List<RoomObjectName> roomObjectNames = GetRoomObjectsForTask(routineType);

        // get all room objects of that type on the map
        List<RoomObjectGO> roomObjectsGOs = new List<RoomObjectGO>();
        for (int i = 0; i < roomObjectNames.Count; i++)
        {
            roomObjectsGOs = RoomManager.RoomObjectGOs.Where(roomObjectGO => roomObjectGO.RoomObject.RoomObjectName == roomObjectNames[i]).ToList();
        }
        Logger.Warning("We found {0} of suitable room objects for {1}", roomObjectsGOs.Count, routineType);

        if (roomObjectsGOs.Count > 0)
        {

            int random = UnityEngine.Random.Range(0, roomObjectsGOs.Count);
            Logger.Log("Random that we found: {0}", random);
            TaskLocationRoomObject = roomObjectsGOs[random];
        }
    }

    public static CharacterRoutineTask CreateCharacterRoutineTask(CharacterRoutineType routineType, Character character)
    {
        CharacterRoutineTask characterRoutineTask = new CharacterRoutineTask(routineType, character)
            .WithDuration();
        return characterRoutineTask;
    }

    public static CharacterRoutineTask CreateCharacterRoutineTask(Character character)
    {
        CharacterRoutineType routineType = GetRandomRoutineType();
        CharacterRoutineTask characterRoutineTask = new CharacterRoutineTask(routineType, character)
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

                if (TaskLocationRoomObject == null)
                {
                    Logger.Warning("The room object targetted for routine does not exist anymore.");
                    travellingToLocation = false;
                    Character.PlannedRoutine.InterruptRoutine();
                    return;
                }
                if (Character.NavActor.Target != targetLocation || Vector2.Distance(Character.transform.position, targetLocation) < CharacterManager.MinDistanceForInteraction)
                {
                    Character.SetCharacterActionState(CharacterActionState.Idle);
                    Character.NavActor.Target = new Vector2(Character.transform.position.x, Character.transform.position.y);
                    travellingToLocation = false;
                }
                if (!Character.NavActor.FollowingPath)
                 {
                    Character.PlannedRoutine.InterruptRoutine();
                    travellingToLocation = false;
                    return;
                }
            }
        }

        //Character.PlayerLocomotion.StopLocomotion();
        Character.CharacterAnimationHandler.SetLocomotion(false);

        Character.SetCharacterActionState(CharacterActionState.RoutineAction);
        Logger.Log(Logger.Character, "{0} is now doing {1}", Character.FullName(), CharacterRoutineType);
        await Task.Delay(Duration);
        return;
    }

    private static CharacterRoutineType GetRandomRoutineType()
    {
        Array values = Enum.GetValues(typeof(CharacterRoutineType));
        int randomValue = Util.InitRandomNumber().Next(values.Length);

        CharacterRoutineType randomCharacterRoutineType = (CharacterRoutineType)values.GetValue(randomValue);

        return randomCharacterRoutineType;
    }

    private List<RoomObjectName> GetRoomObjectsForTask(CharacterRoutineType routineType)
    {
        List<RoomObjectName> roomObjectNames = new List<RoomObjectName>();
        switch (CharacterRoutineType)
        {
            case CharacterRoutineType.Idle:
                break;
            case CharacterRoutineType.Sing:
                roomObjectNames.Add(RoomObjectName.ControlRoomMicrophone);
                break;
            case CharacterRoutineType.MakePhoneCall:
                roomObjectNames.Add(RoomObjectName.Telephone);
                break;
            default:
                Logger.Error("No room objects were assigned for the routine {0}", routineType);
                return null;
        }

        return roomObjectNames;
    }
}
