﻿using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterLocomotion : MonoBehaviour
{
    public ObjectDirection CharacterDirection = ObjectDirection.Down;

    public float BaseSpeed = 8f;
    public float Speed;

    private Transform _characterNavTransform;
    private CharacterAnimationHandler _characterAnimationHandler;
    private Vector2 m_FingerDownPosition;

    public void Awake()
    {
        Speed = BaseSpeed;
    }

    public void Start()
    {
        _characterNavTransform = PlayerCharacter.Instance.NavTransform;
        _characterAnimationHandler = PlayerCharacter.Instance.CharacterAnimationHandler;
    }
    void Update()
    {
        if (GameManager.MainMenuOpen)
            return;

        CheckMouseInput();
        if(PlayerCharacter.Instance.CharacterActionState == CharacterActionState.Moving)
        {
            HandleMovement();
        }
    }


    private void CheckMouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
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
        if (BuilderManager.BuildMenuActivated)
            BuilderManager.Instance.DeactivateBuildMenuMode();

        Logger.Warning(Logger.Locomotion, "New location target set for player: {0}", newTarget);

        _characterAnimationHandler.InLocomotion = true;
        PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Moving);

        PlayerCharacter.Instance.PlayerNav.Target = new Vector3(newTarget.x, newTarget.y, PlayerCharacter.Instance.transform.position.z);
    }

    private void HandleMovement()
    {
        if (!PlayerCharacter.Instance.PlayerNav.FollowingPath)
        {
            _characterAnimationHandler.InLocomotion = false;
            return;
        }
        else
        {
            _characterAnimationHandler.InLocomotion = true;
        }
        CalculateCharacterDirection();
        if (!PlayerCharacter.Instance.PlayerNav.FollowingPath)
        {
            _characterAnimationHandler.InLocomotion = false;
        }
    }

    public void StopLocomotion()
    {
        _characterAnimationHandler.InLocomotion = false;
        PlayerCharacter.Instance.SetCharacterActionState(CharacterActionState.Idle);
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
