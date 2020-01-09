public interface IPlatformConfiguration
{
    float PanSpeed
    {
        get;
        set;
    }
    float ZoomModifierSpeed
    {
        get;
        set;
    }

}

public class AndroidConfiguration : IPlatformConfiguration
{
    private float _panSpeed;
    private float _zoomModifierSpeed;

    public AndroidConfiguration()
    {
        PanSpeed = 1.3f;
        ZoomModifierSpeed = 0.032f;
    }

    public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
    public float ZoomModifierSpeed { get { return _zoomModifierSpeed; } set { _zoomModifierSpeed = value; } }
}

public class PCConfiguration : IPlatformConfiguration
{
    private float _panSpeed;
    private float _zoomModifierSpeed;

    public PCConfiguration()
    {
        PanSpeed = 10f;
        ZoomModifierSpeed = 14f;
    }

    public float PanSpeed { get { return _panSpeed; } set { _panSpeed = value; } }
    public float ZoomModifierSpeed { get { return _zoomModifierSpeed; } set { _zoomModifierSpeed = value; } }
}