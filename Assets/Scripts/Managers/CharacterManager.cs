﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
    
public struct CharacterStats
{
    public CharacterStats(Role role, int age, Gender gender)
    {
        Role = role;
        Name = CharacterNameGenerator.Generate(gender);
        Age = age;
        Gender = gender;
        Image = CharacterImageGenerator.Generate(gender);
    }
    public Role Role;
    public CharacterName Name;
    public int Age;
    public Gender Gender;
    public string Image;
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public GameObject CharacterPrefab;

    public GameObject SceneObjectsGO;

    public Character SelectedCharacter;
    public List<Character> Characters = new List<Character>();

    private AvatarContainer _avatarContainer;

    public Sprite[] AvatarsMale;
    public Sprite[] AvatarsFemale;

    public static float MinDistanceForInteraction = 0.4f; // how much distance between character and room object to 'reach' target location;

    void Awake()
    {
        Guard.CheckIsNull(CharacterPrefab, "CharacterPrefab");
        Guard.CheckIsNull(SceneObjectsGO, "SceneObjectsGO");

        Instance = this;

        AvatarsMale = Resources.LoadAll<Sprite>("Icons/Avatars/TestAvatarsMale");
        AvatarsFemale = Resources.LoadAll<Sprite>("Icons/Avatars/TestAvatarsFemale");
    }

    async void Start()
    {
        _avatarContainer = AvatarContainer.Instance;

        await GenerateCharacter(
            new CharacterStats(
                CharacterRoleGenerator.Generate(),
                CharacterAgeGenerator.Generate(),
                CharacterNameGenerator.PickGender()),
            new Vector2(0, 15));
        await GenerateCharacter(
            new CharacterStats(
                CharacterRoleGenerator.Generate(),
                CharacterAgeGenerator.Generate(),
                CharacterNameGenerator.PickGender()),
            new Vector2(5, 10));
    }

    public async Task GenerateCharacter(CharacterStats characterStats, Vector2 position)
    {
        GameObject characterGO = GameManager.Instance.InstantiatePrefab(CharacterPrefab, SceneObjectsGO.transform, position);

        Character character = SetupCharacter(characterGO, characterStats);
        characterGO.name = CharacterNameGenerator.GetName(characterStats.Name);

        await UpdatePathfindingGrid();
        return;
    }

    public Character SetupCharacter(GameObject characterGO, CharacterStats characterStats)
    {
        Character character = AddCharacterRole(characterGO, characterStats);
        character.Setup(characterStats.Name, characterStats.Age, characterStats.Gender, characterStats.Image);

        Characters.Add(character);
        return character;
    }

    public Character AddCharacterRole(GameObject characterGO, CharacterStats characterStats)
    {
        switch (characterStats.Role)
        {
            case Role.Musician:
                return characterGO.AddComponent<Musician>();
            case Role.Engineer:
                return characterGO.AddComponent<Engineer>();
            default:
                Logger.Error("Character Role type {0} not yet implemented", characterStats.Role);
                return null;
        }

    }

    public void SelectCharacter(Character character)
    {
        SelectedCharacter = character;

        _avatarContainer.CreateAvatar(character);
    }

    public void DeselectCharacter()
    {
        SelectedCharacter = null;
        _avatarContainer.DeleteCurrentAvatar();
    }

    public void DeleteCharacter(Character character)
    {
        Logger.Log(Logger.Character, "Deleted character {0}", character.FullName());
        Characters.Remove(character);
        Destroy(character.gameObject);
        Destroy(character);
    }

    public async Task UpdatePathfindingGrid()
    {
        await Task.Yield();
        GameManager.Instance.AstarPath.Scan();

        Logger.Log("Updated pathfinding grid");
    }

}
