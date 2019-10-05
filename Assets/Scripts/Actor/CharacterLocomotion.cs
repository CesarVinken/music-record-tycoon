using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public ObjectDirection CharacterDirection = ObjectDirection.Down;

    public float BaseSpeed = .8f;
    public float Speed;

    private Transform _characterNavTransform;
    private Vector2 _target;

    public void Awake()
    {
        Speed = BaseSpeed;
        _characterNavTransform = PlayerCharacter.Instance.NavTransform;
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
        //Debug.Log("New location target set for player: " + target);
        _target = target;

        PlayerCharacter.Instance.CharacterAnimationHandler.InLocomotion = true;
        PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Moving);
    }

    private void HandleMovement()
    {
        if(transform.position.x != _target.x || transform.position.y != _target.y)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, _target, Speed * Time.deltaTime);

            // Turn the player
            _characterNavTransform.right = -(new Vector2(transform.position.x, transform.position.y) - newPosition);

            if (!IsColliding(newPosition))
            {
                CalculateCharacterDirection();

                transform.position = newPosition;
            }
            if (Vector2.Distance(transform.position, _target) < 0.02)
            {
                StopLocomotion();
            }
        }
    }

    public bool IsColliding(Vector2 newPosition)
    {
        //Debug.DrawLine(transform.position, transform.position + transform.right * 3, Color.red, 1f);

        // do raycasthit to check if there is collision with an object in front of the player
        // in case of an upcoming collision, make the player stop in time so that he can listen to new movement commands.
        RaycastHit2D collisionTest = Physics2D.Raycast(transform.position, _characterNavTransform.right, .8f);

        if (collisionTest)
            return true;

        return false;
    }

    public void StopLocomotion()
    {
        PlayerCharacter.Instance.CharacterAnimationHandler.InLocomotion = false;
        PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Idle);
    }

    public void CalculateCharacterDirection()
    {
        float verticalAngle = Vector3.Angle(_characterNavTransform.up, -transform.right);
        float horizontalAngle = Vector3.Angle(_characterNavTransform.up, -transform.up);  //This expects the playerGO always to be pointed to the right

        if (horizontalAngle < 75)//we are moving left if the angle is less than 90
        {
            PlayerCharacter.Instance.CharacterAnimationHandler.SetHorizontal(1f);
            if (verticalAngle < 75)  //if vertical angle is less than 90, it means we are moving up
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(1f);    //left and up
                CharacterDirection = ObjectDirection.LeftUp;
            }
            else if (verticalAngle > 105)
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(-1f);   //left and down
                CharacterDirection = ObjectDirection.LeftDown;
            }
            else
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(0); //left
                CharacterDirection = ObjectDirection.Left;
            }
        }
        else if (horizontalAngle > 105)
        {
            PlayerCharacter.Instance.CharacterAnimationHandler.SetHorizontal(-1f);
            if (verticalAngle < 75)  //if vertical angle is less than 90, it means we are moving up
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(1f);    //right and up
                CharacterDirection = ObjectDirection.RightUp;
            }
            else if (verticalAngle > 105)
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(-1f);   //right and down
                CharacterDirection = ObjectDirection.RightDown;
            }
            else
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(0); //right
                CharacterDirection = ObjectDirection.Right;
            }
        }
        else //walking up / down
        {
            PlayerCharacter.Instance.CharacterAnimationHandler.SetHorizontal(0f);
            if (verticalAngle < 90)  //if vertical angle is less than 90, it means we are moving up
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(1f);    //straight up
                CharacterDirection = ObjectDirection.Up;
            }
            else
            {
                PlayerCharacter.Instance.CharacterAnimationHandler.SetVertical(-1f); //straight down
                CharacterDirection = ObjectDirection.Down;
            }
        }
    }
}
