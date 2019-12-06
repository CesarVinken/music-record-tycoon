using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public Transform NavTransform;
    public NavActor PlayerNav;

    public CharacterActionState CharacterActionState;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterAnimationHandler CharacterAnimationHandler;

    public Room CurrentRoom;
    public string Id;

    void Awake()
    {
        Instance = this;
        Id = "Player"; // TODO: Make generic!

        if (NavTransform == null)
            Debug.LogError("Could not find NavTransform on character");
        if (PlayerNav == null)
            Debug.LogError("Could not find PlayerNav on character");

        PlayerLocomotion = gameObject.AddComponent<CharacterLocomotion>();
        CharacterAnimationHandler = gameObject.AddComponent<CharacterAnimationHandler>();
        SetCharacterActionState(CharacterActionState.Idle);
    }


    void Update()
    {
        
    }

    public void SetCharacterActionState(CharacterActionState newState)
    {
        Debug.Log($"CharacterActionState set to {newState}");
        CharacterActionState = newState;
    }
}
