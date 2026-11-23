
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ICPuzzleManager : MonoBehaviour
{
    [Header("LED Settings")]
    public Image[] Leds;

    public float glowSize = 3f;
    public float glowMaxAlpha = 0.8f;
    public float pulseSpeed = 2f;

    [Header("Victory Settings")]
    public Image circuitBoard;
    public CanvasGroup completionPanel;

    public int flashCount = 3;
    public float flashDuration = 0.15f;
    public float fadeDuration = 0.6f;

    private bool[] ledStates;
    private Color[] originalColors;
    private Image[] glowImages;

    private Color originalBoardColor;
    private bool isCompleted = false;

    public bool IsCompleted
    {
        get { return isCompleted; }
    }

    void Start()
    {
        ledStates = new bool[Leds.Length];
        originalColors = new Color[Leds.Length];
        glowImages = new Image[Leds.Length];

        for (int i = 0; i < Leds.Length; i++)
        {
            if (Leds[i] == null)
                continue;

            originalColors[i] = Leds[i].color;

            CreateGlow(i);

            Leds[i].enabled = true;
            SetBrightness(i, 0.2f);
        }

        if (circuitBoard != null)
            originalBoardColor = circuitBoard.color;

        if (completionPanel != null)
        {
            completionPanel.alpha = 0;
            completionPanel.interactable = false;
            completionPanel.blocksRaycasts = false;
        }
    }

    void CreateGlow(int index)
    {
        Image led = Leds[index];

        GameObject glowObject =
            new GameObject("Glow_" + led.name);

        glowObject.transform.SetParent(
            led.transform.parent, false);

        RectTransform glowRect =
            glowObject.AddComponent<RectTransform>();

        RectTransform ledRect =
            led.GetComponent<RectTransform>();

        glowRect.anchorMin = ledRect.anchorMin;
        glowRect.anchorMax = ledRect.anchorMax;
        glowRect.pivot = ledRect.pivot;
        glowRect.anchoredPosition = ledRect.anchoredPosition;
        glowRect.localRotation = ledRect.localRotation;
        glowRect.localScale = ledRect.localScale;

        glowRect.sizeDelta =
            ledRect.rect.size * glowSize;

        glowRect.SetSiblingIndex(
            led.transform.GetSiblingIndex());

        Image glow = glowObject.AddComponent<Image>();

        glow.sprite = CreateGlowSprite();

        glow.color = new Color(
            originalColors[index].r,
            originalColors[index].g,
            originalColors[index].b,
            0f
        );

        glow.raycastTarget = false;
        glowImages[index] = glow;
    }

    Sprite CreateGlowSprite()
    {
        int size = 128;

        Texture2D texture = new Texture2D(
            size, size, TextureFormat.RGBA32, false);

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - size / 2f) / (size / 2f);
                float dy = (y - size / 2f) / (size / 2f);

                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                float alpha = Mathf.Pow(
                    Mathf.Clamp01(1f - distance), 2.5f);

                texture.SetPixel(
                    x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100
        );
    }

    void SetBrightness(int index, float brightness)
    {
        Color c = originalColors[index];

        Leds[index].color = new Color(
            c.r * brightness,
            c.g * brightness,
            c.b * brightness,
            c.a
        );
    }

    public void TurnOnLED(int index)
    {
        if (isCompleted)
            return;

        if (index < 0 || index >= Leds.Length)
            return;

        if (ledStates[index])
            return;

        ledStates[index] = true;

        SetBrightness(index, 1.5f);

        glowImages[index].enabled = true;

        StartCoroutine(PulseGlow(index));

        CheckWin();
    }

    IEnumerator PulseGlow(int index)
    {
        Image glow = glowImages[index];

        Color baseColor = originalColors[index];

        while (true)
        {
            float pulse =
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            float alpha = Mathf.Lerp(
                0.25f,
                glowMaxAlpha,
                pulse
            );

            glow.color = new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                alpha
            );

            yield return null;
        }
    }

    void CheckWin()
    {
        for (int i = 0; i < ledStates.Length; i++)
        {
            if (!ledStates[i])
                return;
        }

        if (!isCompleted)
        {
            isCompleted = true;

            Debug.Log("IC PUZZLE COMPLETED!");

            StartCoroutine(PlayVictoryAnimation());
        }
    }

    IEnumerator PlayVictoryAnimation()
    {
        // Flash bo mach
        if (circuitBoard != null)
        {
            for (int i = 0; i < flashCount; i++)
            {
                circuitBoard.color = Color.white;

                yield return new WaitForSeconds(flashDuration);

                circuitBoard.color = originalBoardColor;

                yield return new WaitForSeconds(flashDuration);
            }
        }

        // Hien completion panel
        if (completionPanel != null)
        {
            completionPanel.interactable = true;
            completionPanel.blocksRaycasts = true;

            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;

                completionPanel.alpha =
                    Mathf.Clamp01(timer / fadeDuration);

                yield return null;
            }

            completionPanel.alpha = 1f;
        }

        Debug.Log("VICTORY ANIMATION FINISHED!");
    }
}
