
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WireDraw : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    public RectTransform wiresParent;
    public RectTransform target;
    public ICPuzzleManager puzzleManager;
    public int ledIndex;

    public float lineWidth = 6f;

    private WireCollision collisionChecker;

    private List<GameObject> wireParts =
        new List<GameObject>();

    private List<Vector2> points =
        new List<Vector2>();

    private bool isDrawing = false;
    private bool isConnected = false;
    private bool isInvalid = false;

    private Vector2 lastPoint;

    void Start()
    {
        collisionChecker =
            wiresParent.GetComponent<WireCollision>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {   if (isConnected ||
    (puzzleManager != null && puzzleManager.IsCompleted))
    return;
        if (isConnected)
            return;

        isDrawing = true;
        isInvalid = false;

        points.Clear();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wiresParent,
            eventData.position,
            eventData.pressEventCamera,
            out lastPoint
        );

        points.Add(lastPoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDrawing)
            return;

        Vector2 currentPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wiresParent,
            eventData.position,
            eventData.pressEventCamera,
            out currentPoint
        );

        if (Vector2.Distance(lastPoint, currentPoint) < 4f)
            return;

        CheckNewSegment(lastPoint, currentPoint);

        DrawLine(lastPoint, currentPoint);

        lastPoint = currentPoint;
        points.Add(currentPoint);
    }

    void CheckNewSegment(Vector2 start, Vector2 end)
    {
        if (collisionChecker == null)
            return;

        if (collisionChecker.CheckCollision(
            start, end, lineWidth + 2f))
        {
            isInvalid = true;

            Debug.Log("Wire collision!");
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject part = new GameObject("WirePart");

        part.transform.SetParent(wiresParent, false);

        Image image = part.AddComponent<Image>();

        image.color = GetComponent<Image>().color;
        image.raycastTarget = false;

        RectTransform rect =
            part.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);

        Vector2 direction = end - start;

        rect.anchoredPosition = start;

        rect.sizeDelta = new Vector2(
            direction.magnitude,
            lineWidth
        );

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        rect.localRotation =
            Quaternion.Euler(0, 0, angle);

        wireParts.Add(part);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDrawing)
            return;

        Vector2 endPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wiresParent,
            eventData.position,
            eventData.pressEventCamera,
            out endPoint
        );

        if (Vector2.Distance(lastPoint, endPoint) > 0.1f)
        {
            CheckNewSegment(lastPoint, endPoint);
            DrawLine(lastPoint, endPoint);
            points.Add(endPoint);
        }

        isDrawing = false;

        bool correctTarget =
            target != null &&
            RectTransformUtility.RectangleContainsScreenPoint(
                target,
                eventData.position,
                eventData.pressEventCamera
            );

        if (correctTarget && !isInvalid)
{
    isConnected = true;

    collisionChecker.SaveWire(points);

    // Bat LED tuong ung
    if (puzzleManager != null)
    {
        puzzleManager.TurnOnLED(ledIndex);
    }

    Debug.Log("Correct! Wire saved.");
}
        else
        {
            ResetWire();

            if (isInvalid)
                Debug.Log("Wire touched another wire!");
            else
                Debug.Log("Wrong target!");
        }
    }

    void ResetWire()
    {
        foreach (GameObject part in wireParts)
        {
            Destroy(part);
        }

        wireParts.Clear();
        points.Clear();
    }
}
