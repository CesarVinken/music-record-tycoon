using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public Transform NavTransform;

    public CharacterActionState CharacterActionState;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterAnimationHandler CharacterAnimationHandler;
    void Awake()
    {
        //if (Animator == null)
        //    Animator = GetComponent<Animator>();
        Instance = this;

        if (NavTransform == null)
            Debug.LogError("Could not find NavTransform on character");

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
