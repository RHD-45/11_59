using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TMP_Text))]
public class TypeWriterText : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private StressBar stressBar;

    [Header("Random Settings")]
    [SerializeField] private int text_character_max = 465;
    [SerializeField] private int text_character_min = 430;
    
    private const string CHAR_POOL = "■";


    [Header("Visual Settings")]
     private Color32  typedColor = new Color32(120, 120, 120, 255); 
    // Màu chữ đang gõ / đã gõ đúng
     private Color32   hiddenColor = new Color32(255, 255, 255, 255);

    private string generatedText;
    private bool isKeyA = true; 
    private int currentPage = 1;
    private int currentCharInPage = 0;
    public int totalChar = 0;
    private float stressPoint = 0.018f;
    void Start()
    {
        textComponent.ForceMeshUpdate();
        GenerateRandomText();
        UpdateTypingProgress(0);
    }

    void Update()
    {
        keyPress();
    }

    private void keyPress()
    {
        if(Keyboard.current != null)
        {
            if(isKeyA && Keyboard.current.aKey.wasPressedThisFrame || !isKeyA && Keyboard.current.dKey.wasPressedThisFrame)
            {
                KeyPressSuccess();
            }
        }

        if(currentCharInPage >= generatedText.Length)
        {
            UpdateNewPage();
        }
    }

    private void KeyPressSuccess()
    {
        isKeyA = !isKeyA;
        UpdateTypingProgress(currentCharInPage++);
        totalChar++;
        stressBar.SetStress(stressPoint);
    }

    private void UpdateNewPage()
    {
        currentPage++;
        currentCharInPage = 0;
        GenerateRandomText();
    }

    private void UpdateTypingProgress(int currentTypedIndex)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

            Color32 targetColor = (i <= currentTypedIndex) ? typedColor : hiddenColor;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j] = targetColor;
            }
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void GenerateRandomText()
    {
        int randomLength = Random.Range(text_character_min,text_character_max);
        int visibleChar = 0;
        float k = 1.1f;

        char[] stringChars = new char[randomLength];

        for (int i = 0 ;i < randomLength; i++){
            // double propb = (Mathf.Exp(k*visibleChar/7f) -1)/ (Mathf.Exp(k) -1) ;
            double propb = 1f/(1 + Mathf.Exp(-k*(visibleChar - 3.5f)));
            if(i > 0 && i < randomLength -1 && stringChars[i-1] != ' ' && Random.Range(0f,1f) < propb)
            {
                stringChars[i] = ' ';
                visibleChar = 0;
                continue;
            }
            stringChars[i] = CHAR_POOL[0];
            visibleChar++;
        }

        generatedText = new string(stringChars);
        textComponent.text = generatedText;
        UpdateTypingProgress(generatedText.Length);
    }
}

