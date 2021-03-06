﻿using UnityEngine;

public class WallPiece : MonoBehaviour
{
    public WallPieceType WallPieceType;
    public WallPieceDisplayMode DisplayMode;

    public SpriteRenderer SpriteRenderer;

    public SpriteRenderer ConnectionLeft;
    public SpriteRenderer ConnectionRight;

    public Room Room;

    public void Awake()
    {
        Guard.CheckIsNull(SpriteRenderer, "SpriteRenderer");
    }

    public void SetWallSprite(WallPieceDisplayMode wallPieceDisplayMode)
    {
        if (wallPieceDisplayMode == WallPieceDisplayMode.Visible)
        {
            SpriteRenderer.color = new Color(1, 1, 1, 1);
            if (ConnectionLeft)
                ConnectionLeft.color = new Color(1, 1, 1, 1);
            if (ConnectionRight)
                ConnectionRight.color = new Color(1, 1, 1, 1);
        }
        else
        {
            SpriteRenderer.color = new Color(1, 1, 1, .4f);
            if(ConnectionLeft)
                ConnectionLeft.color = new Color(1, 1, 1, .4f);
            if (ConnectionRight)
                ConnectionRight.color = new Color(1, 1, 1, .4f);
        }
    }
}

public enum WallPieceType
{
    DownRight,
    DownLeft,
    UpRight,
    UpLeft
}

public enum WallPieceDisplayMode
{
    Visible,
    Transparent
}