using UnityEngine;

public class WallPiece : MonoBehaviour
{
    public WallPieceType WallPieceType;
    public WallPieceDisplayMode DisplayMode;

    public SpriteRenderer SpriteRenderer;

    public Sprite TallSprite;
    public Sprite ShortSprite;

    public void Awake()
    {
        Debug.Log("Halo");
        Guard.CheckIsNull(SpriteRenderer, "SpriteRenderer");
        Guard.CheckIsNull(TallSprite, "TallSprite");
        Guard.CheckIsNull(ShortSprite, "ShortSprite");
    }

    public void SetWallSprite(WallPieceDisplayMode wallPieceDisplayMode)
    {
        if (wallPieceDisplayMode == WallPieceDisplayMode.Tall)
        {
            SpriteRenderer.sprite = TallSprite;
        }
        else
        {
            SpriteRenderer.sprite = ShortSprite;
        }
    }
}

public enum WallPieceType
{
    DownRight,
    DownLeft,
    UpRight,
    UpLeft,
    CornerDown,
    CornerUp,
    CornerLeft,
    CornerRight
}

public enum WallPieceDisplayMode
{
    Tall,
    Short
}