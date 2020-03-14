using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public struct CharacterStats
{
    public CharacterStats(string name, int age, Gender gender, string image)
    {
        Name = name;
        Age = age;
        Gender = gender;
        Image = image;
    }
    public string Name;
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

    void Awake()
    {
        Guard.CheckIsNull(CharacterPrefab, "CharacterPrefab");
        Guard.CheckIsNull(NavActorPrefab, "NavActorPrefab");

        Guard.CheckIsNull(SceneObjectsGO, "SceneObjectsGO");
        Guard.CheckIsNull(PathfindingGO, "PathfindingGO");

        Instance = this;
    }

    async void Start()
    {
        _avatarContainer = AvatarContainer.Instance;

        await GeneratePlayableCharacter(new CharacterStats("Bruce", 27, Gender.Male, "imageString"), new Vector2(0, 15));
        await GeneratePlayableCharacter(new CharacterStats("Frank Zappa", 33, Gender.Male, "imageString"), new Vector2(5, 10));
    }

    public async Task GeneratePlayableCharacter(CharacterStats characterStats, Vector2 position)
    {
        Logger.Log(Logger.Initialisation, "Create character");

        GameObject characterGO = Instantiate(CharacterPrefab, SceneObjectsGO.transform);
        PlayableCharacter playableCharacter = characterGO.GetComponent<PlayableCharacter>();
        playableCharacter.transform.position = position;

        GameObject navActorGO = Instantiate(NavActorPrefab, PathfindingGO.transform);
        NavActor navActor = navActorGO.GetComponent<NavActor>();
        navActor.SetCharacter(playableCharacter);
        navActorGO.transform.position = characterGO.transform.position;

        characterGO.name = characterStats.Name;
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
        //await Task.Delay(20);
        await Task.Yield();
        GameManager.Instance.PathfindingGrid.CreateGrid();


        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].NavActor.Target.x == Characters[i].transform.position.x &&
                Characters[i].NavActor.Target.y == Characters[i].transform.position.y ||
                Characters[i].NavActor.Target == new Vector3(0, 0, 0))
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
