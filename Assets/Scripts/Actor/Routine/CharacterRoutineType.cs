using System.Collections.Generic;

public class CharacterRoutineType
{
    public CharacterRoutineTypeName Name;
    public List<RoomObjectName> RoomObjects; // the room objects with which a character can perform this routine task
    public List<Role> CharacterRoles;

    private CharacterRoutineType(CharacterRoutineTypeName name)
    {
        Name = name;
        RoomObjects = new List<RoomObjectName>();
        CharacterRoles = new List<Role>();
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
            default:
                Logger.Error("Cannot find a creation function for character routine name {0}", name);
                return null;
        }
    }

    private CharacterRoutineType WithRoomObjects(List<RoomObjectName> roomObjects)
    {
        RoomObjects = roomObjects;
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
        return WithRoomObjects(new List<RoomObjectName>
        {
            RoomObjectName.Telephone
        });
    }

    private CharacterRoutineType Sing()
    {
        List<Role> characterRoles = new List<Role>() { Role.Musician };
        return WithRoomObjects(new List<RoomObjectName>
        {
            RoomObjectName.ControlRoomMicrophone
        })
        .ForCharacterRoles(characterRoles);
    }
}
