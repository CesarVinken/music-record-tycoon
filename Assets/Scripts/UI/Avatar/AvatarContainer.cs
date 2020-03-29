using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarContainer : MonoBehaviour
{
    public static AvatarContainer Instance;

    public GameObject AvatarPrefab;
    public Dictionary<string, GameObject> AvatarGOs = new Dictionary<string, GameObject>(); // Character Id and CharacterAvatar GO
    public Dictionary<string, AvatarTile> Avatars = new Dictionary<string, AvatarTile>(); // Character Id and AvatarTile

    void Awake()
    {
        Guard.CheckIsNull(AvatarPrefab, "AvatarPrefab");

        Instance = this;
    }

    public void CreateAvatar(Character character)
    {
        GameObject avatarGO = Instantiate(AvatarPrefab, transform);
        AvatarGOs.Add(character.Id, avatarGO);

        AvatarTile avatar = avatarGO.GetComponent<AvatarTile>();
        avatar.Setup(character);
        Avatars.Add(character.Id, avatar);
    }

    public void DeleteAvatar(Character character)
    {
        if (!Avatars.ContainsKey(character.Id))
        {
            Logger.Warning("Tried, but could not remove avatar for character {0}", character.Id);
            return;
        }
        if (!AvatarGOs.ContainsKey(character.Id))
        {
            Logger.Warning("Tried, but could not remove avatar go for character {0}", character.Id);
            return;
        }
        GameObject avatarGO = AvatarGOs[character.Id];

        AvatarGOs.Remove(character.Id);
        Avatars.Remove(character.Id);

        Destroy(avatarGO);
    }

    public void SelectAvatar(Character selectedCharacter, Character previousCharacter)
    {
        if (previousCharacter != null)
        {
            if (!Avatars.ContainsKey(previousCharacter.Id))
            {
                Logger.Warning("Tried, but could not find avatar for character {0}", previousCharacter.Id);
            } 
            else
            {
                Avatars[previousCharacter.Id].DisableSelectionMarker();
            }
        }

        if (!Avatars.ContainsKey(selectedCharacter.Id))
        {
            Logger.Warning("Tried, but could not find avatar for character {0}", selectedCharacter.Id);
            return;
        }

        Avatars[selectedCharacter.Id].EnableSelectionMarker();
    }
}
