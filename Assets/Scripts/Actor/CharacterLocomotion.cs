﻿using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public ObjectDirection CharacterDirection = ObjectDirection.Down;
    public Character Character;
    public float BaseSpeed = 8f;
    public float Speed;

    private Transform _characterNavTransform;
    private CharacterAnimationHandler _characterAnimationHandler;
    private Vector2 m_FingerDownPosition;

    public void Awake()
    {
        if (Character == null)
            Logger.Log(Logger.Initialisation, "Could not find Actor component on character");

        Speed = BaseSpeed;
    }

    public void Start()
    {
        _characterNavTransform = Character.NavActor.transform;
        _characterAnimationHandler = Character.CharacterAnimationHandler;
    }
    void Update()
    {
        if (GameManager.MainMenuOpen)
            return;

        if (Console.Instance && Console.Instance.ConsoleState == ConsoleState.Large)
            return;

        CheckPointerInput();
        if(Character.CharacterActionState == CharacterActionState.Moving)
        {
            HandleMovement();
        }
    }


    private void CheckPointerInput()
    {
        if (!(Character is IPlayable))
            return;

        if (Character.Id != CharacterManager.Instance.SelectedCharacter.Id)
            return;

        if (PointerHelper.IsPointerOverGameObject())
            return;

        if (BuilderManager.InBuildMode)
            return;

        if (OnScreenTextContainer.Instance.ObjectInteractionTextContainer)
            return;

        if (GameManager.Instance.CurrentPlatform == Platform.PC)
        {
            if (Input.GetMouseButtonDown(0))
            {                
                Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetLocomotionTarget(target);
            }
        }
        else if (Input.touchCount > 0) { 
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                m_FingerDownPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                Vector2 releaseDistance = m_FingerDownPosition - touch.position;

                if((releaseDistance.x < 12 && releaseDistance.x > -12) && (releaseDistance.y < 12 && releaseDistance.y > -12))  // tapping start position is roughly the same as the release position
                {
                    Vector2 target = Camera.main.ScreenToWorldPoint(touch.position);
                    SetLocomotionTarget(target);
                }
                else
                {
                    Logger.Log("{0} and {1}", m_FingerDownPosition, touch.position);
                }
            }
        }
    }

    public void SetLocomotionTarget(Vector3 newTarget)
    {
        Logger.Warning(Logger.Locomotion, "New location target set for player: {0}", newTarget);

        if(Character.CharacterActionState != CharacterActionState.Action)
        {
            _characterAnimationHandler.SetLocomotion(true, Character);
        }

        Character.NavActor.Target = new Vector3(newTarget.x, newTarget.y, transform.position.z);
    }

    public void RetryReachLocomotionTarget()
    {
        Logger.Log("Retry locomotion target");

        Vector3 target = new Vector3(Character.NavActor.Target.x, Character.NavActor.Target.y, transform.position.z);
        Character.NavActor.Target = target;
    }

    private void HandleMovement()
    {
        if (!Character.NavActor.FollowingPath && _characterAnimationHandler.InLocomotion)
        {
            StopLocomotion();
            return;
        }
        else if(Character.NavActor.FollowingPath)
        {
            _characterAnimationHandler.SetLocomotion(true, Character);
        }
        CalculateCharacterDirection();
        if (!Character.NavActor.FollowingPath && _characterAnimationHandler.InLocomotion)
        {
            StopLocomotion();
        }
    }

    public void StopLocomotion(CharacterActionState nextState = CharacterActionState.Idle)
    {
        _characterAnimationHandler.SetLocomotion(false);
        Character.SetCharacterActionState(nextState);
    }

    public void CalculateCharacterDirection()
    {
        float verticalAngle = Vector3.Angle(_characterNavTransform.forward, transform.up);
        float horizontalAngle = Vector3.Angle(_characterNavTransform.forward, -transform.right);  //This expects the playerGO always to be pointed to the right

        if (horizontalAngle < 75)//we are moving left if the angle is less than 90
        {
            _characterAnimationHandler.SetHorizontal(1f);
            if (verticalAngle < 75)  //if vertical angle is less than 90, it means we are moving up
            {
                _characterAnimationHandler.SetVertical(1f);    //left and up
                CharacterDirection = ObjectDirection.LeftUp;
            }
            else if (verticalAngle > 105)
            {
                _characterAnimationHandler.SetVertical(-1f);   //left and down
                CharacterDirection = ObjectDirection.LeftDown;
            }
            else
            {
                _characterAnimationHandler.SetVertical(0); //left
                CharacterDirection = ObjectDirection.Left;
            }
        }
        else if (horizontalAngle > 105)
        {
            _characterAnimationHandler.SetHorizontal(-1f);
            if (verticalAngle < 75)  //if vertical angle is less than 90, it means we are moving up
            {
                _characterAnimationHandler.SetVertical(1f);    //right and up
                CharacterDirection = ObjectDirection.RightUp;
            }
            else if (verticalAngle > 105)
            {
                _characterAnimationHandler.SetVertical(-1f);   //right and down
                CharacterDirection = ObjectDirection.RightDown;
            }
            else
            {
                _characterAnimationHandler.SetVertical(0); //right
                CharacterDirection = ObjectDirection.Right;
            }
        }
        else //walking up / down
        {
            _characterAnimationHandler.SetHorizontal(0f);
            if (verticalAngle < 90)  //if vertical angle is less than 90, it means we are moving up
            {
                _characterAnimationHandler.SetVertical(1f);    //straight up
                CharacterDirection = ObjectDirection.Up;
            }
            else
            {
                _characterAnimationHandler.SetVertical(-1f); //straight down
                CharacterDirection = ObjectDirection.Down;
            }
        }
    }

    //Position is set through the pathfinding system and synchronised with the character's navActor
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
