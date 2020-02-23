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
        GeneratePlayableCharacter();
    }

    public void GeneratePlayableCharacter()
    {
        Logger.Log(Logger.Initialisation, "Create character");

        GameObject characterGO = Instantiate(CharacterPrefab, SceneObjectsGO.transform);
        PlayableCharacter playableCharacter = characterGO.GetComponent<PlayableCharacter>();
        SetSelectedCharacter(playableCharacter);

        GameObject navActorGO = Instantiate(NavActorPrefab, PathfindingGO.transform);
        NavActor navActor = navActorGO.GetComponent<NavActor>();
        navActor.SetCharacter(playableCharacter);

        string name = "Bruce";
        characterGO.name = name;
        playableCharacter.Setup(name, 27, Gender.Male, "imageString");
        playableCharacter.Select();
    }

    public void SetSelectedCharacter(PlayableCharacter playableCharacter)
    {
        SelectedCharacter = playableCharacter;
    }

    public void UpdatePathfindingGrid()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            IEnumerator updateGrid = WaitAndUpdatePathfindingGrid(Characters[i]);
            StartCoroutine(updateGrid);
        }
    }

    public IEnumerator WaitAndUpdatePathfindingGrid(Character character)
    {
        yield return new WaitForSeconds(0.01f);
        GameManager.Instance.PathfindingGrid.CreateGrid();  // May have to change to partly recreating the grid.

        character.NavActor.IsReevaluating = true;
        yield return new WaitForSeconds(0.08f);

        // TODO: update routes for all moving characters on the map

        character.PlayerLocomotion.RetryReachLocomotionTarget();
    }
}
