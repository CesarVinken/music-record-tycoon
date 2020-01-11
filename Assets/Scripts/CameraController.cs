using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float PanSpeed;
    public float PanBorderThickness = 15f; // in pixels

    public static Dictionary<Direction, float> PanLimits = new Dictionary<Direction, float>
        {
            { Direction.Up, 40f },
            { Direction.Right, 40f },
            { Direction.Down, -40f },
            { Direction.Left, -40f },
        };

    private float _panBorderThickness;
    private float _touchesPreviousPositionDifference;
    private float _touchesCurrentPositionDifference;

    private float _zoomModifier;
    private float _minZoomLevel = 35f;
    private float _maxZoomLevel = 80f;

    private Vector2 _firstTouchPreviousPosition;
    private Vector2 _secondTouchPreviousPosition;

    private float _zoomModifierSpeed;

    private Camera _camera;

    public void Awake()
    {
        _camera = GetComponent<Camera>();
        if (!_camera)
            Logger.Error(Logger.Initialisation, "Could not find main camera");
    }

    public void Start()
    {
        _panBorderThickness = PanBorderThickness;

        PanSpeed = GameManager.Instance.Configuration.PanSpeed;
        _zoomModifierSpeed = GameManager.Instance.Configuration.ZoomModifierSpeed;
    }

    void Update()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        HandleMobileZooming();
        HandleComputerZooming();

        position = HandleMobilePanning(position);
        position = HandleComputerPanning(position);

        // binding to the limits of the map
        position.x = Mathf.Clamp(position.x, PanLimits[Direction.Left], PanLimits[Direction.Right]);
        position.y = Mathf.Clamp(position.y, PanLimits[Direction.Down], PanLimits[Direction.Up]);

        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public Vector2 HandleMobilePanning(Vector2 position)
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            position.x += -touchDeltaPosition.x * PanSpeed * Time.deltaTime;
            position.y += -touchDeltaPosition.y * PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public void HandleMobileZooming()
    {
        if (Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            _firstTouchPreviousPosition = firstTouch.position - firstTouch.deltaPosition;
            _secondTouchPreviousPosition = secondTouch.position - secondTouch.deltaPosition;

            _touchesPreviousPositionDifference = (_firstTouchPreviousPosition - _secondTouchPreviousPosition).magnitude;
            _touchesCurrentPositionDifference = (firstTouch.position - secondTouch.position).magnitude;

            _zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * _zoomModifierSpeed;

            if (_touchesPreviousPositionDifference > _touchesCurrentPositionDifference)
            {
                _camera.orthographicSize += _zoomModifier;
            }
            if (_touchesPreviousPositionDifference < _touchesCurrentPositionDifference)
            {
                _camera.orthographicSize -= _zoomModifier;
            }

            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _minZoomLevel, _maxZoomLevel);

            if (MainCanvas.Instance.PointerImage.sprite != null)
            {
                MainCanvas.Instance.SetPointerImageSize(MainCanvas.Instance.PointerImage);
            }
        }
    }

    public void HandleComputerZooming()
    {
        float fieldOfView = _camera.orthographicSize;
        fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * _zoomModifierSpeed;
        _camera.orthographicSize = Mathf.Clamp(fieldOfView, _minZoomLevel, _maxZoomLevel);

        if (MainCanvas.Instance.PointerImage.sprite != null && !BuildMenuContainer.Instance.IsOpen)
        {
            MainCanvas.Instance.SetPointerImageSize(MainCanvas.Instance.PointerImage);
        }
    }

    public Vector2 HandleComputerPanning(Vector2 position)
    {
        position = HandlePanUp(position);
        position = HandlePanDown(position);
        position = HandlePanLeft(position);
        position = HandlePanRight(position);

        return position;
    }

    public Vector2 HandlePanUp(Vector2 position)
    {
        if (Input.GetKey(KeyCode.W) || (Input.mousePosition.y >= Screen.height - _panBorderThickness && Input.mousePosition.y <= Screen.height + _panBorderThickness * 2))
        {
            position.y += PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanDown(Vector2 position)
    {
        if (Input.GetKey(KeyCode.S) || (Input.mousePosition.y <= _panBorderThickness && Input.mousePosition.y >= -_panBorderThickness * 2))
        {
            position.y -= PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanLeft(Vector2 position)
    {
        if (Input.GetKey(KeyCode.D) || (Input.mousePosition.x >= Screen.width - _panBorderThickness && Input.mousePosition.x <= Screen.width + _panBorderThickness * 2))
        {
            position.x += PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanRight(Vector2 position)
    {
        if (Input.GetKey(KeyCode.A) || (Input.mousePosition.x <= _panBorderThickness && Input.mousePosition.x >= -_panBorderThickness * 2))
        {
            position.x -= PanSpeed * Time.deltaTime;
        }

        return position;
    }
}
