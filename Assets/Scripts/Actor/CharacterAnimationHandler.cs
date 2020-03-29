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
        Logger.Log("animator handler awoken");

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

    public void SetLocomotion(bool value, Character character)
    {
        InLocomotion = value;
        if(value)
            character.SetCharacterActionState(CharacterActionState.Moving);
    }
}
