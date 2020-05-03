using UnityEngine;

public class CharacterAnimationHandler : MonoBehaviour
{
    public Animator Animator;

    private bool _inLocomotion = false;
    public bool InLocomotion
    {
        get { return _inLocomotion; }
        set
        {
            _inLocomotion = value;
            Animator.SetBool("Locomotion", _inLocomotion);
        }
    }


    public void Awake()
    {
        if (Animator == null)
            Animator = GetComponent<Animator>();
    }

    public void SetHorizontal(float speed)
    {
        Animator.SetFloat("Horizontal", speed);
    }

    public void SetVertical(float speed)
    {
        Animator.SetFloat("Vertical", speed);
    }

    public void SetLocomotion(bool value)
    {
        InLocomotion = value;
    }

    public void SetLocomotion(bool inLocomotion, Character character)
    {
        InLocomotion = inLocomotion;
        if(inLocomotion)
            character.SetCharacterActionState(CharacterActionState.Moving);
    }
}
