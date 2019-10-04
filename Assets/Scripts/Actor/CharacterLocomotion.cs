using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private Vector2 _target;

    public float BaseSpeed = 4f;
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
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetLocomotionTarget(target);
        }
    }

    public void SetLocomotionTarget(Vector3 target)
    {
        Debug.Log("New location target set for player: " + target);
        _target = target;

        PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Moving);
    }

    private void HandleMovement()
    {
        if(transform.position.x != _target.x || transform.position.y != _target.y)
        {
            Vector2 newPosition = Vector2.Lerp(transform.position, _target, Speed * Time.deltaTime);
            if (!IsColliding(newPosition))
            {
                transform.position = newPosition;
            }
            if (Vector2.Distance(transform.position, _target) < 0.01)
            {
                PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Idle);
            }
        }
    }

    public bool IsColliding(Vector2 newPosition)
    {
        // make the player face the direction he is moving towards
        transform.right = -(new Vector2(transform.position.x, transform.position.y) - newPosition);

        //Debug.DrawLine(transform.position, transform.position + transform.right * 3, Color.red, 1f);

        // do raycasthit to check if there is collision with an object in front of the player
        // in case of an upcoming collision, make the player stop in time so that he can listen to new movement commands.
        RaycastHit2D collisionTest = Physics2D.Raycast(transform.position, transform.right, .8f);

        if(collisionTest)
        {
            Debug.LogWarning("colision test true!");
            Debug.Log(collisionTest.rigidbody);
            return true;
        }
        Debug.Log("colision test false!");
        return false;
    }
}
