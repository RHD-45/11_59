
using UnityEngine;
using System.Collections.Generic;

public class WireCollision : MonoBehaviour
{
    private List<List<Vector2>> savedWires =
        new List<List<Vector2>>();

    public bool CheckCollision(
        Vector2 start,
        Vector2 end,
        float minDistance)
    {
        foreach (List<Vector2> wire in savedWires)
        {
            for (int i = 0; i < wire.Count - 1; i++)
            {
                float distance = GetDistance(
                    start,
                    end,
                    wire[i],
                    wire[i + 1]
                );

                if (distance <= minDistance)
                    return true;
            }
        }

        return false;
    }

    public void SaveWire(List<Vector2> points)
    {
        savedWires.Add(new List<Vector2>(points));
    }

    float GetDistance(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        Vector2 d)
    {
        if (IsIntersect(a, b, c, d))
            return 0;

        float d1 = PointDistance(a, c, d);
        float d2 = PointDistance(b, c, d);
        float d3 = PointDistance(c, a, b);
        float d4 = PointDistance(d, a, b);

        return Mathf.Min(d1, d2, d3, d4);
    }

    float PointDistance(
        Vector2 point,
        Vector2 a,
        Vector2 b)
    {
        Vector2 direction = b - a;

        float length = direction.sqrMagnitude;

        if (length == 0)
            return Vector2.Distance(point, a);

        float t = Vector2.Dot(point - a, direction) / length;

        t = Mathf.Clamp01(t);

        Vector2 closest = a + t * direction;

        return Vector2.Distance(point, closest);
    }

    bool IsIntersect(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        Vector2 d)
    {
        float o1 = Cross(b - a, c - a);
        float o2 = Cross(b - a, d - a);
        float o3 = Cross(d - c, a - c);
        float o4 = Cross(d - c, b - c);

        return o1 * o2 <= 0 && o3 * o4 <= 0;
    }

    float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}
