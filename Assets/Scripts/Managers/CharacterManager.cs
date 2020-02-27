using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        Guard.CheckIsNull(CharacterPrefab, "CharacterPrefab");
        Guard.CheckIsNull(NavActorPrefab, "NavActorPrefab");

        Guard.CheckIsNull(SceneObjectsGO, "SceneObjectsGO");
        Guard.CheckIsNull(PathfindingGO, "PathfindingGO");

        Instance = this;
    }

    void Start()
    {
        _avatarContainer = AvatarContainer.Instance;
        GeneratePlayableCharacter("Bruce", 27, Gender.Male, "imageString");
        GeneratePlayableCharacter("Frank Zappa", 33, Gender.Male, "imageString");
    }

    public void GeneratePlayableCharacter(string name, int age, Gender gender, string image)
    {
        Logger.Log(Logger.Initialisation, "Create character");

        GameObject characterGO = Instantiate(CharacterPrefab, SceneObjectsGO.transform);
        PlayableCharacter playableCharacter = characterGO.GetComponent<PlayableCharacter>();

        GameObject navActorGO = Instantiate(NavActorPrefab, PathfindingGO.transform);
        NavActor navActor = navActorGO.GetComponent<NavActor>();
        navActor.SetCharacter(playableCharacter);

        characterGO.name = name;
        playableCharacter.Setup(name, age, gender, image);

        _avatarContainer.CreateAvatar(playableCharacter);

        SelectCharacter(playableCharacter);

        Characters.Add(playableCharacter);

        UpdatePathfindingGrid();
    }

    public void SelectCharacter(PlayableCharacter playableCharacter)
    {
        PlayableCharacter previouslySelectedCharacter = SelectedCharacter;
        SelectedCharacter = playableCharacter;

        _avatarContainer.SelectAvatar(playableCharacter, previouslySelectedCharacter);
    }

    public void UpdatePathfindingGrid()
    {
        IEnumerator updateGrid = WaitAndUpdatePathfindingGrid();
        StartCoroutine(updateGrid);
        for (int i = 0; i < Characters.Count; i++)
        {
            IEnumerator retryReachLocomotionTarget = WaitAndRetryReachLocomotionTarget(Characters[i]);
            StartCoroutine(retryReachLocomotionTarget);
        }
    }

    public IEnumerator WaitAndUpdatePathfindingGrid()
    {
        yield return new WaitForSeconds(0.01f);

        Logger.Log("Update pathfinding grid");
        GameManager.Instance.PathfindingGrid.CreateGrid();
    }

    public IEnumerator WaitAndRetryReachLocomotionTarget(Character character)
    {
        character.NavActor.IsReevaluating = true;
        yield return new WaitForSeconds(0.08f);

        character.PlayerLocomotion.RetryReachLocomotionTarget();
    }

}
