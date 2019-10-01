using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float PanSpeed = 4f;
    public float PanBorderThickness = 10f; // in pixels
    public Vector2 PanMinLimit = new Vector2(8f, 12f);
    public Vector2 PanMaxLimit = new Vector2(10f, 10f);

    void Update()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        position = HandleMobilePanning(position);
        position = HandleComputerPanning(position);

        // binding to the limits of the map
        position.x = Mathf.Clamp(position.x, -PanMinLimit.x, PanMinLimit.x);
        position.y = Mathf.Clamp(position.y, -PanMaxLimit.y, PanMaxLimit.y);

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
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - PanBorderThickness)
        {
            position.y += PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanDown(Vector2 position)
    {
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= PanBorderThickness)
        {
            position.y -= PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanLeft(Vector2 position)
    {
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - PanBorderThickness)
        {
            position.x += PanSpeed * Time.deltaTime;
        }

        return position;
    }

    public Vector2 HandlePanRight(Vector2 position)
    {
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= PanBorderThickness)
        {
            position.x -= PanSpeed * Time.deltaTime;
        }

        return position;
    }
}
