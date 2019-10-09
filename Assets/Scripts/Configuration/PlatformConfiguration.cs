public interface IPlatformConfiguration
{
    float PanSpeed
    {
        get;
        set;
    }

}

public class AndroidConfiguration : IPlatformConfiguration
{
    private float _panSpeed;

    public AndroidConfiguration()
    {
        PanSpeed = 0.8f;
    }

    public float PanSpeed
    {
        get
        {
            return _panSpeed;
        }

        set
        {
            _panSpeed = value;
        }
    }
}

public class PCConfiguration : IPlatformConfiguration
{
    private float _panSpeed;


    public PCConfiguration()
    {
        PanSpeed = 8f;
    }

    public float PanSpeed
    {
        get
        {
            return _panSpeed;
        }

        set
        {
            _panSpeed = value;
        }
    }
}