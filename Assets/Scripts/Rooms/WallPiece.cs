using UnityEngine;

public class WallPiece : MonoBehaviour
{
    public WallPieceType WallPieceType;
    public WallPieceDisplayMode DisplayMode;

    public SpriteRenderer SpriteRenderer;

    public Sprite TallSprite;

    public void Awake()
    {
        Debug.Log("Halo");
        Guard.CheckIsNull(SpriteRenderer, "SpriteRenderer");
        Guard.CheckIsNull(TallSprite, "TallSprite");
    }

    public void SetWallSprite(WallPieceDisplayMode wallPieceDisplayMode)
    {
        if (wallPieceDisplayMode == WallPieceDisplayMode.Visible)
        {
            SpriteRenderer.color = new Color(1,1,1,1);
        }
        else
        {
            SpriteRenderer.color = new Color(1, 1, 1, .4f);
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
    Visible,
    Transparent
}