﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public struct CharacterStats
{
    public CharacterStats(int age, Gender gender)
    {
        Name = CharacterNameGenerator.Generate(gender);
        Age = age;
        Gender = gender;
        Image = CharacterImageGenerator.Generate(gender);
    }
    public CharacterName Name;
    public int Age;
    public Gender Gender;
    public string Image;
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public GameObject CharacterPrefab;
    public GameObject NavActorPrefab;

    public GameObject SceneObjectsGO;
    public GameObject PathfindingGO;

    public PlayableCharacter SelectedCharacter;
    public List<Character> Characters = new List<Character>();

    private AvatarContainer _avatarContainer;

    public Sprite[] AvatarsMale;
    public Sprite[] AvatarsFemale;

    void Awake()
    {
        Guard.CheckIsNull(CharacterPrefab, "CharacterPrefab");
        Guard.CheckIsNull(NavActorPrefab, "NavActorPrefab");

        Guard.CheckIsNull(SceneObjectsGO, "SceneObjectsGO");
        Guard.CheckIsNull(PathfindingGO, "PathfindingGO");

        Instance = this;

        AvatarsMale = Resources.LoadAll<Sprite>("Icons/Avatars/TestAvatarsMale");
        AvatarsFemale = Resources.LoadAll<Sprite>("Icons/Avatars/TestAvatarsFemale");
    }

    async void Start()
    {
        _avatarContainer = AvatarContainer.Instance;

        await GeneratePlayableCharacter(new CharacterStats(CharacterAgeGenerator.Generate(), CharacterNameGenerator.PickGender()), new Vector2(0, 15));
        await GeneratePlayableCharacter(new CharacterStats(CharacterAgeGenerator.Generate(), CharacterNameGenerator.PickGender()), new Vector2(5, 10));
    }

    public void Update()
    {
        //Logger.Log("GENERATE NAME:::: {0}", CharacterNameGenerator.GetName(CharacterNameGenerator.Generate(CharacterNameGenerator.PickGender())));
    }

    public async Task GeneratePlayableCharacter(CharacterStats characterStats, Vector2 position)
    {
        Logger.Log(Logger.Initialisation, "Create character");

        GameObject characterGO = GameManager.Instance.InstantiatePrefab(CharacterPrefab, SceneObjectsGO.transform, position);
        PlayableCharacter playableCharacter = characterGO.GetComponent<PlayableCharacter>();

        GameObject navActorGO = GameManager.Instance.InstantiatePrefab(NavActorPrefab, PathfindingGO.transform, characterGO.transform.position);
        NavActor navActor = navActorGO.GetComponent<NavActor>();
        navActor.SetCharacter(playableCharacter);

        characterGO.name = CharacterNameGenerator.GetName(characterStats.Name);
        playableCharacter.Setup(characterStats.Name, characterStats.Age, characterStats.Gender, characterStats.Image);

        _avatarContainer.CreateAvatar(playableCharacter);

        SelectCharacter(playableCharacter);

        Characters.Add(playableCharacter);

        await UpdatePathfindingGrid();
        return;
    }

    public void SelectCharacter(PlayableCharacter playableCharacter)
    {
        PlayableCharacter previouslySelectedCharacter = SelectedCharacter;
        SelectedCharacter = playableCharacter;

        _avatarContainer.SelectAvatar(playableCharacter, previouslySelectedCharacter);
    }

    public async Task UpdatePathfindingGrid()
    {
        await Task.Yield();
        GameManager.Instance.PathfindingGrid.CreateGrid();


        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].NavActor.Target.x == Characters[i].transform.position.x &&
                Characters[i].NavActor.Target.y == Characters[i].transform.position.y ||
                Characters[i].NavActor.Target == new Vector2(0, 0))
                continue;

            IEnumerator retryReachLocomotionTarget = WaitAndRetryReachLocomotionTarget(Characters[i]);
            StartCoroutine(retryReachLocomotionTarget);
        }
        Logger.Log("Updated pathfinding grid");
    }

    public IEnumerator WaitAndRetryReachLocomotionTarget(Character character)
    {
        character.NavActor.IsReevaluating = true;
        yield return new WaitForSeconds(0.08f);

        character.PlayerLocomotion.RetryReachLocomotionTarget();
    }

}
