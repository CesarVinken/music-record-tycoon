using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public Text Text;

    private Animator _animator;

    public 

    void Awake()
    {
        Guard.CheckIsNull(Text, "Text Component");
        _animator = GetComponent<Animator>();
    }

    public void Setup(NotificationType notificationType, string notificationText)
    {
        SetNotificationType(notificationType);
        SetText(notificationText);
    }

    private void SetNotificationType(NotificationType notificationType)
    {
        switch (notificationType)
        {
            case NotificationType.FromBelow:
                _animator.SetInteger("NotificationType", 1);
                break;
            case NotificationType.FromPointer:
                _animator.SetInteger("NotificationType", 1);
                transform.position = Input.mousePosition;
                break;
            default:
                Logger.Error("Unknown notification type {$0}", notificationType);
                break;
        }
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
