
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
                if (IsIntersect(
                    start,
                    end,
                    wire[i],
                    wire[i + 1]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void SaveWire(List<Vector2> points)
    {
        savedWires.Add(new List<Vector2>(points));
    }

    bool IsIntersect(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        Vector2 d)
    {
        const float epsilon = 0.0001f;

        float o1 = Cross(b - a, c - a);
        float o2 = Cross(b - a, d - a);
        float o3 = Cross(d - c, a - c);
        float o4 = Cross(d - c, b - c);

        // Only count proper intersections.
        // Nearby and parallel wires are allowed.
        return
            ((o1 > epsilon && o2 < -epsilon) ||
             (o1 < -epsilon && o2 > epsilon)) &&
            ((o3 > epsilon && o4 < -epsilon) ||
             (o3 < -epsilon && o4 > epsilon));
    }

    float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }
}
