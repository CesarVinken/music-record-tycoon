using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private Vector2 _target;

    public float BaseSpeed = 8f;
    public float Speed;

    public void Awake()
    {
        Speed = BaseSpeed;
    }

    void Update()
    {
        CheckMouseInput();
        if(PlayerCharacter.Instance.CharacterActionState == CharacterActionState.Moving)
        {
            HandleMovement();
        }
    }

    private void CheckMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetLocomotionTarget(_target);
        }
    }

    private void SetLocomotionTarget(Vector3 target)
    {
        Debug.Log("New location target set for player: " + target);

        // Change animation

        PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Moving);
    }

    private void HandleMovement()
    {
        if(transform.position.x != _target.x || transform.position.y != _target.y)
        {
            transform.position = Vector2.Lerp(transform.position, _target, Speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, _target) < 0.01)
            {
                PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Idle);
            }
        }
    }
}
