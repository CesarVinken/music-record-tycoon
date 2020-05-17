using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;
    public static List<Room> Rooms;
    public static List<RoomObjectGO> RoomObjectGOs;

    void Awake()
    {
        Instance = this;

        Rooms = new List<Room>();
        RoomObjectGOs = new List<RoomObjectGO>();
    }

    public void AddRoom(Room Room)
    {
        Rooms.Add(Room);

        for (int i = 0; i < Room.RoomObjects.Count; i++)
        {
            RoomObjectGO roomObjectGO = Room.RoomObjects[i];
            roomObjectGO.Register();
        }
    }

    public void RemoveRoom(Room Room)
    {
        Rooms.Remove(Room);

        for (int i = Room.RoomObjects.Count - 1; i >= 0; i--)
        {
            RoomObjectGO roomObjectGO = Room.RoomObjects[i];
            roomObjectGO.Deregister();
        }
    }

    public void DeleteAllRooms()
    {
        for (int i = Rooms.Count - 1; i >= 0; i--)
        {
            BuilderManager.Instance.DeleteRoom(Rooms[i]);
        }
    }

    public void RegisterRoomObjectGO(RoomObjectGO roomObjectGO)
    {
        bool exists = RoomObjectGOs.Any(go => go.RoomObjectBlueprint.RoomObjectName == roomObjectGO.RoomObjectBlueprint.RoomObjectName);
        //Logger.Log("Registered a {0} object in {1}", roomObjectGO.RoomObject.RoomObjectName, roomObjectGO.ParentRoom.RoomBlueprint.RoomName);
        RoomObjectGOs.Add(roomObjectGO);

        // if first of its kind, check if we should register a new routine
        if(!exists)
        {
            for (int i = 0; i < roomObjectGO.RoomObjectBlueprint.CharacterRoutines.Length; i++)
            {
                CharacterRoutineType routineType = roomObjectGO.RoomObjectBlueprint.CharacterRoutines[i];
                Logger.Warning("routine type in object {0} is {1}. ", roomObjectGO.RoomObjectBlueprint.Name, routineType.Name);
                if (!RoutineManager.AvailableRoutineTypes.ContainsKey(routineType.Name))
                {
                    Logger.Log("{0} is not yet in the available routine types. We already had: ", routineType.Name);
                    for (int j = 0; j < RoutineManager.AvailableRoutineTypes.Count; j++)
                    {
                        Logger.Log("{0}", RoutineManager.AvailableRoutineTypes.ElementAt(j).Key);
                    }
                    RoutineManager.EnableRoutineType(routineType);
                }
            }
            
        }
    }

    public void DeregisterRoomObjectGO(RoomObjectGO roomObjectGO)
    {
        RoomObjectGOs.Remove(roomObjectGO);

        // if room object last of its kind, check if we should deregister a routine
        bool exists = RoomObjectGOs.Any(go => go.RoomObjectBlueprint.RoomObjectName == roomObjectGO.RoomObjectBlueprint.RoomObjectName);

        List<CharacterRoutineType> noLongerAvailableRoutines = new List<CharacterRoutineType>();
        if (!exists)
        {
            for (int i = 0; i < RoutineManager.AvailableRoutineTypes.Count; i++)
            {
                CharacterRoutineType characterRoutineType = RoutineManager.AvailableRoutineTypes.ElementAt(i).Value;
                bool containsRoomObject = characterRoutineType.RoomObjects.Contains(roomObjectGO.RoomObjectBlueprint.RoomObjectName);
                bool isStillAvailable = false;

                if (containsRoomObject)
                {
                    for (int j = 0; j < characterRoutineType.RoomObjects.Count; j++)
                    {
                        RoomObjectName roomObjectInRoutine = characterRoutineType.RoomObjects[j];
                        if (RoomObjectGOs.Any(go => go.RoomObjectBlueprint.RoomObjectName == roomObjectInRoutine))
                        {
                            isStillAvailable = true;
                            break;
                        }
                    }
                }
                if(isStillAvailable)
                {
                    noLongerAvailableRoutines.Add(characterRoutineType);
                }
            }

            for (int k = 0; k < noLongerAvailableRoutines.Count; k++)
            {
                RoutineManager.DisableRoutineType(noLongerAvailableRoutines[k]);
            }
        }
    }
}
