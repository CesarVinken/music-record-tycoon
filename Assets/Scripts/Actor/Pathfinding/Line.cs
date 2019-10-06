using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    const float VerticalLineGradient = 1e5f;
    float Gradient;
    float Y_intercept;
    Vector2 PointOnLine_1;
    Vector2 PointOnLine_2;

    float GradientPerpendicular;
    bool ApproachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if(dx == 0)
        {
            GradientPerpendicular = VerticalLineGradient;
        }
        else
        {
            GradientPerpendicular = dy / dx;
        }

        if(GradientPerpendicular == 0)
        {
            Gradient = VerticalLineGradient;
        }
        else
        {
            Gradient = -1 / GradientPerpendicular;

        }

        Y_intercept = pointOnLine.y - Gradient * pointOnLine.x;
        PointOnLine_1 = pointOnLine;
        PointOnLine_2 = pointOnLine + new Vector2(1, Gradient);

        ApproachSide = false;
        ApproachSide = GetSide(pointPerpendicularToLine);

    }

    bool GetSide(Vector2 p)
    {
        return (p.x - PointOnLine_1.x) * (PointOnLine_2.y - PointOnLine_1.y) > (p.y - PointOnLine_1.y) * (PointOnLine_2.x - PointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != ApproachSide;
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDir = new Vector3(1, 0, Gradient).normalized;
        Vector3 lineCentre = new Vector3(PointOnLine_1.x, 0, PointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
    }
}
