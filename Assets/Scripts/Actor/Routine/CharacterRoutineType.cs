using System.Collections.Generic;

public class CharacterRoutineType
{
    public CharacterRoutineTypeName Name;
    public List<RoomObjectName> RoomObjects; // the room objects with which a character can perform this routine task
    public List<Role> CharacterRoles;
    public float ProbabilityWeight;
    public Dictionary<int, float> TimeProbabilityMultiplier; // int, float = hour, probability multiplier

    private CharacterRoutineType(CharacterRoutineTypeName name)
    {
        Name = name;
        RoomObjects = new List<RoomObjectName>();
        CharacterRoles = new List<Role>();
        ProbabilityWeight = 100;
        TimeProbabilityMultiplier = new Dictionary<int, float>();
    }

    public static CharacterRoutineType Create(CharacterRoutineTypeName name)
    {
        CharacterRoutineType characterRoutineType = new CharacterRoutineType(name);
        switch (name)
        {
            case CharacterRoutineTypeName.Idle1:
                return characterRoutineType.Idle();
            case CharacterRoutineTypeName.MakePhoneCall:
                return characterRoutineType.MakePhoneCall();
            case CharacterRoutineTypeName.Sing:
                return characterRoutineType.Sing();
            case CharacterRoutineTypeName.Sleep:
                return characterRoutineType.Sleep();
            default:
                Logger.Error("Cannot find a creation function for character routine name {0}", name);
                return null;
        }
    }

    public float ApplyTimeProbabilityMultiplier(float originalWeight)
    {
        float timeProbabilityMultiplier = 1;
        bool foundAvailableHour = false;
        for (int i = TimeManager.Instance.CurrentTimeInHours - 1; i >= 0; i--)
        {
            if(TimeProbabilityMultiplier.ContainsKey(i))
            {
                timeProbabilityMultiplier = TimeProbabilityMultiplier[i];
                foundAvailableHour = true;
                break;
            }
        }
        if (!foundAvailableHour)
        {
            for (int j = 24 - 1; j > TimeManager.Instance.CurrentTimeInHours; j--)
            {
                if (TimeProbabilityMultiplier.ContainsKey(j))
                {
                    timeProbabilityMultiplier = TimeProbabilityMultiplier[j];
                    foundAvailableHour = true;
                    break;
                }
            }
        }
        float adjustedWeight = originalWeight * timeProbabilityMultiplier;
        return adjustedWeight;
    }

    private CharacterRoutineType WithRoomObjects(List<RoomObjectName> roomObjects)
    {
        RoomObjects = roomObjects;
        return this;
    }

    private CharacterRoutineType WithProbabilityWeight(float weight)
    {
        ProbabilityWeight = weight;
        return this;
    }

    private CharacterRoutineType WithTimeProbabilityMultiplier(Dictionary<int, float> timeProbabilityMultiplier)
    {
        TimeProbabilityMultiplier = timeProbabilityMultiplier;
        return this;
    }

    private CharacterRoutineType ForCharacterRoles(List<Role> characterRoles)
    {
        CharacterRoles = characterRoles;
        return this;
    }

    private CharacterRoutineType Idle()
    {
        return this;
    }

    private CharacterRoutineType MakePhoneCall()
    {
        Dictionary<int, float> timeProbabilityMultiplier = new Dictionary<int, float>
        {
            { 0, 0.3f },
            { 9, 0.8f },
            { 11, 1 },
            { 22, 0.8f },
        }; 
        
        return WithRoomObjects(new List<RoomObjectName>
        {
            RoomObjectName.Telephone
        }).WithTimeProbabilityMultiplier(timeProbabilityMultiplier);
    }

    private CharacterRoutineType Sing()
    {
        Dictionary<int, float> timeProbabilityMultiplier = new Dictionary<int, float>
        {
            { 0, 0.8f },
            { 4, 0.4f },
            { 15, 1 },
            { 20, 1.2f },
        }; 
        
        List<Role> characterRoles = new List<Role>() { Role.Musician };

        return WithRoomObjects(new List<RoomObjectName>
        {
            RoomObjectName.ControlRoomMicrophone
        })
        .ForCharacterRoles(characterRoles)
        .WithTimeProbabilityMultiplier(timeProbabilityMultiplier);
    }

    private CharacterRoutineType Sleep()
    {
        Dictionary<int, float> timeProbabilityMultiplier = new Dictionary<int, float>
       {
           { 0, 4 },
           { 7, 1 },
           { 9, 0 },
           { 23, .5f },
       };

        return WithTimeProbabilityMultiplier(timeProbabilityMultiplier);
    }
}
