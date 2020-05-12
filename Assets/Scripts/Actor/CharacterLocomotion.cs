using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;

public class CharacterLocomotion : MonoBehaviour
{
    public ObjectDirection CharacterDirection = ObjectDirection.Down;
    public Character Character;
    public float BaseSpeed = 8f;
    public float Speed;

    private CharacterAnimationHandler _characterAnimationHandler;
    private Vector2 m_FingerDownPosition;
    private bool _listenForInput;
    public AIDestinationSetter DestinationSetter;
    public CharacterPath CharacterPath;
    private GameObject _targetObject;
    public Vector2 Target { get { return new Vector2(_targetObject.transform.position.x, _targetObject.transform.position.y); } }

    public void Awake()
    {
        Character = GetComponent<Character>();

        if (Character == null)
            Logger.Log(Logger.Initialisation, "Could not find Actor component on character");

        if (DestinationSetter == null)
            Logger.Log(Logger.Initialisation, "Could not find AIDestinationSetter component on CharacterLocomotion");

        if (CharacterPath == null)
            Logger.Log(Logger.Initialisation, "Could not find CharacterPath component on CharacterLocomotion");

        Speed = BaseSpeed;
    }

    public void Start()
    {
        _characterAnimationHandler = Character.CharacterAnimationHandler;
        _listenForInput = true;
    }
    void Update()
    {
        if (GameManager.MainMenuOpen)
            return;

        if (Console.Instance && Console.Instance.ConsoleState == ConsoleState.Large)
            return;

        CheckPointerInput();
        if (_targetObject != null)
        {
            HandleMovement();
        }
    }

    private void CheckPointerInput()
    {
        if (!_listenForInput)
            return;

        if (CharacterManager.Instance.SelectedCharacter == null)
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
        if(_targetObject == null)
        {
            GameObject targetGO = new GameObject();
            targetGO.transform.SetParent(GameManager.Instance.AstarGO.transform);
            _targetObject = targetGO;
        }

        _targetObject.transform.position = newTarget;

        DestinationSetter.target = _targetObject.transform;
        _characterAnimationHandler.SetLocomotion(true);
        Character.SetCharacterActionState(CharacterActionState.Moving);
    }

    //public void RetryReachLocomotionTarget()
    //{
    //    Logger.Log("Retry locomotion target");

    //    //Vector3 target = new Vector3(Character.NavActor.Target.x, Character.NavActor.Target.y, transform.position.z);
    //    //Character.NavActor.Target = target;
    //}

    public void ReachLocomotionTarget()
    {
        DestinationSetter.target = null;

        _characterAnimationHandler.SetLocomotion(false);
        Character.SetCharacterActionState(CharacterActionState.Idle);
    }

    private void HandleMovement()
    {
        transform.position = CharacterPath.position;
        CalculateCharacterDirection();
    }

    public void CalculateCharacterDirection()
    {
        float rotationAngle = CharacterPath.transform.rotation.eulerAngles.z;

        if(rotationAngle < 22.5f)
        {
            _characterAnimationHandler.SetHorizontal(0f);
            _characterAnimationHandler.SetVertical(1f);
            CharacterDirection = ObjectDirection.Up;
        }
        else if (rotationAngle < 67.5f)
        {
            _characterAnimationHandler.SetHorizontal(1f);
            _characterAnimationHandler.SetVertical(1f);
            CharacterDirection = ObjectDirection.LeftUp;
        }
        else if (rotationAngle < 112.5f)
        {
            _characterAnimationHandler.SetHorizontal(1f);
            _characterAnimationHandler.SetVertical(0);
            CharacterDirection = ObjectDirection.Left;
        }
        else if (rotationAngle < 157.5f)
        {
            _characterAnimationHandler.SetHorizontal(1f);
            _characterAnimationHandler.SetVertical(-1f);
            CharacterDirection = ObjectDirection.LeftDown;
        }
        else if (rotationAngle < 202.5f)
        {
            _characterAnimationHandler.SetHorizontal(0f);
            _characterAnimationHandler.SetVertical(-1f);
            CharacterDirection = ObjectDirection.Down;
        }
        else if (rotationAngle < 247.5f)
        {
            _characterAnimationHandler.SetHorizontal(-1f);
            _characterAnimationHandler.SetVertical(-1f);
            CharacterDirection = ObjectDirection.RightDown;
        }
        else if (rotationAngle < 292.5)
        {
            _characterAnimationHandler.SetHorizontal(-1f);
            _characterAnimationHandler.SetVertical(0f);
            CharacterDirection = ObjectDirection.Right;
        }
        else if (rotationAngle < 337.5)
        {
            _characterAnimationHandler.SetHorizontal(-1f);
            _characterAnimationHandler.SetVertical(1f);
            CharacterDirection = ObjectDirection.RightUp;
        }
        else
        {
            _characterAnimationHandler.SetHorizontal(0f);
            _characterAnimationHandler.SetVertical(1f);
            CharacterDirection = ObjectDirection.Up;
        }
    }

    public async void SetInputListeningFreeze(int timeInMiliseconds)
    {
        _listenForInput = false;
        await Task.Delay(timeInMiliseconds);
        _listenForInput = true;
    }
}
