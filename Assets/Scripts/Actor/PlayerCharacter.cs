using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public Animator Animator;

    public CharacterLocomotion PlayerLocomotion;
    public CharacterActionState CharacterActionState;
    void Awake()
    {
        //if (Animator == null)
        //    Animator = GetComponent<Animator>();
        Instance = this;

        PlayerLocomotion = gameObject.AddComponent<CharacterLocomotion>();
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
