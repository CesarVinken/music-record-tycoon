﻿using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float PanSpeed = 8f;
    public float PanBorderThickness = 15f; // in pixels

    private Vector2 _panMaxLimit;
    private Vector2 _panMinLimit;

    public Vector2 PanMinLimit
    {
        get
        {
            return _panMinLimit;
        }

        set
        {
            _panMinLimit = value;
        }
    }
    public Vector2 PanMaxLimit
    {
        get
        {
            return _panMaxLimit;
        }

        set
        {
            _panMaxLimit = value;
        }
    }

    private float _panBorderThickness;

    public void Start()
    {
        _panBorderThickness = PanBorderThickness;
        PanMinLimit = new Vector2(100f, 40f);
        PanMaxLimit = new Vector2(40f, 40f);
        PanSpeed = GameManager.Instance.Configuration.PanSpeed;
    }

    void Update()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        position = HandleMobilePanning(position);
        position = HandleComputerPanning(position);

        // binding to the limits of the map
        position.x = Mathf.Clamp(position.x, -PanMinLimit.x, PanMaxLimit.x);
        position.y = Mathf.Clamp(position.y, -PanMinLimit.y, PanMaxLimit.y);

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
        if (Input.GetKey(KeyCode.W) || (Input.mousePosition.y >= Screen.height - PanBorderThickness && Input.mousePosition.y <= Screen.height + PanBorderThickness * 2))
        {
            position.y += PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanDown(Vector2 position)
    {
        if (Input.GetKey(KeyCode.S) || (Input.mousePosition.y <= PanBorderThickness && Input.mousePosition.y >= -PanBorderThickness * 2))
        {
            position.y -= PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanLeft(Vector2 position)
    {
        if (Input.GetKey(KeyCode.D) || (Input.mousePosition.x >= Screen.width - PanBorderThickness && Input.mousePosition.x <= Screen.width + PanBorderThickness * 2))
        {
            position.x += PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanRight(Vector2 position)
    {
        if (Input.GetKey(KeyCode.A) || (Input.mousePosition.x <= PanBorderThickness && Input.mousePosition.x >= -PanBorderThickness * 2))
        {
            position.x -= PanSpeed * Time.deltaTime;
        }

        return position;
    }
}
