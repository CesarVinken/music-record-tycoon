using System.Collections.Generic;

public class RoutineManager
{
    //public static List<CharacterRoutineType> RoutineTypes;
    public static List<CharacterRoutineType> AvailableRoutineTypes = new List<CharacterRoutineType>
    {
        // List all the Routines that do not need an object to interact with
        CharacterRoutineType.Create(CharacterRoutineTypeName.Idle1)
    };

    public static void EnableRoutineType(CharacterRoutineType routineType)
    {
        Logger.Log("Enabled routine {0}", routineType.Name);
        AvailableRoutineTypes.Add(routineType);
    }

    public static void DisableRoutineType(CharacterRoutineType routineType)
    {
        Logger.Log("Disabled routine {0}", routineType.Name);
        AvailableRoutineTypes.Remove(routineType);
    }
}
