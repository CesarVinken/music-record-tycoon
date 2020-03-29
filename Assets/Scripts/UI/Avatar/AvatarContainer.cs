using UnityEngine;

public class AvatarContainer : MonoBehaviour
{
    public static AvatarContainer Instance;

    public GameObject AvatarPrefab;

    public GameObject CurrentAvatarGO;
    public AvatarTile CurrentAvatarTile;

    void Awake()
    {
        Guard.CheckIsNull(AvatarPrefab, "AvatarPrefab");

        Instance = this;
    }

    public void CreateAvatar(Character character)
    {
        DeleteCurrentAvatar();
        GameObject avatarGO = Instantiate(AvatarPrefab, transform);
        CurrentAvatarGO = avatarGO;

        AvatarTile avatar = avatarGO.GetComponent<AvatarTile>();
        avatar.Setup(character);
        CurrentAvatarTile = avatar;
    }

    public void DeleteCurrentAvatar()
    {
        if(CurrentAvatarGO)
            Destroy(CurrentAvatarGO);

        if(CurrentAvatarTile)
            Destroy(CurrentAvatarTile);
    }
}
