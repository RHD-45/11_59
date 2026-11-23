
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TMP_Text))]
public class TypeWriterText : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private TMP_Text textComponentBlind;
    [SerializeField] private StressBar stressBar;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameSaveManager saveManager;
    private WindowFrame windowFrame;

    private ReadData readData = new ReadData();

    private string typedTextAll = "";
    private int totalWords = 0;
    private int selectedFileIndex = 0;

    private string[] file_content_path =
    {
        "/Features/Word (BTL)/Data/text_01.txt"
    };

    [Header("Cursor Settings")]
    private const string CURSOR_CHAR = "|";
    private bool cursorVisible = true;
    private float cursorTimer = 0f;
    private float cursorBlinkSpeed = 0.5f;

    [Header("Visual Settings")]
    private Color32 typedColor = new Color32(0, 0, 0, 255);
    private Color32 hiddenColor = new Color32(0, 0, 0, 100);

    private string generatedText = "";

    private int currentPage = 1;
    private int currentCharInPage = 0;

    public int totalChar = 0;

    private float stressPoint = 0.048f;

    void Start()
    {
        selectedFileIndex = Random.Range(
            0,
            file_content_path.Length
        );

        string content = readData.ReadFiles(
            file_content_path[selectedFileIndex]
        );

        progressBar.SetMaxWords(content.Length);

        GenerateRandomText();

        windowFrame = GetComponentInParent<WindowFrame>();
    }

    void Update()
    {
        keyPress();

        cursorTimer += Time.deltaTime;

        if (cursorTimer >= cursorBlinkSpeed)
        {
            cursorTimer = 0f;
            cursorVisible = !cursorVisible;

            UpdateTypingProgress();
        }
    }

    private void keyPress()
    {
        if (!windowFrame.IsFocused) return;
        if (GameManager.State != GameState.Playing) return;

        if (!stressBar.GetCanType())
            return;

        if (Keyboard.current != null)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                KeyPressSuccess();
            }
        }
    }

    private void KeyPressSuccess()
    {
        if (currentCharInPage >= generatedText.Length)
            return;

        char typedChar = generatedText[currentCharInPage];

        typedTextAll += typedChar;

        if (!char.IsWhiteSpace(typedChar))
        {
            if (typedTextAll.Length == 1 ||
                char.IsWhiteSpace(typedTextAll[typedTextAll.Length - 2]))
            {
                totalWords++;
            }
        }

        currentCharInPage++;
        totalChar++;

        cursorVisible = true;
        cursorTimer = 0f;

        UpdateTypingProgress();

        progressBar.UpdateProgress(totalChar);
        stressBar.SetStress(stressPoint);

        if (currentCharInPage >= generatedText.Length)
        {
            UpdateNewPage();
        }
    }

    private void UpdateNewPage()
    {
        if (!readData.HasNextPage())
        {
            Debug.Log("Đã hoàn thành toàn bộ văn bản!");
            return;
        }

        currentPage++;

        GenerateRandomText();
    }

    private void UpdateTypingProgress()
    {
        if (string.IsNullOrEmpty(generatedText))
            return;

        currentCharInPage = Mathf.Clamp(
            currentCharInPage,
            0,
            generatedText.Length
        );

        string typedText = generatedText.Substring(
            0,
            currentCharInPage
        );

        string untypedText = generatedText.Substring(
            currentCharInPage
        );

        string visibleColor =
            ColorUtility.ToHtmlStringRGBA(typedColor);

        Color32 transparentColor = new Color32(
            typedColor.r,
            typedColor.g,
            typedColor.b,
            0
        );

        string hiddenColor =
            ColorUtility.ToHtmlStringRGBA(transparentColor);

        string cursorColor = cursorVisible
            ? visibleColor
            : hiddenColor;

        textComponent.text =
            $"<color=#{visibleColor}>{typedText}</color>" +
            $"<color=#{cursorColor}>{CURSOR_CHAR}</color>" +
            $"<color=#{hiddenColor}>{untypedText}</color>";

        textComponent.richText = true;
        textComponent.ForceMeshUpdate();
    }

    private void GenerateRandomText()
    {
        generatedText = readData.GetNextPage(textComponentBlind);

        textComponentBlind.text = generatedText;
        textComponentBlind.color = hiddenColor;

        textComponent.text = "";
        textComponent.color = typedColor;

        textComponentBlind.ForceMeshUpdate();
        textComponent.ForceMeshUpdate();

        currentCharInPage = 0;

        cursorVisible = true;
        cursorTimer = 0f;

        UpdateTypingProgress();
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();

        data.typedTextAll = typedTextAll;
        data.generatedText = generatedText;

        data.currentCharInPage = currentCharInPage;
        data.currentPage = currentPage;

        data.totalChar = totalChar;
        data.totalWords = totalWords;

        data.readDataIndex = readData.GetIndex();

        data.fileIndex = selectedFileIndex;

        data.stressValue = stressBar.GetStress();
        data.progressValue = progressBar.GetProgress();

        saveManager.Save(data);

    }

    public void LoadGame()
    {
        GameSaveData data = saveManager.Load();

        if (data == null)
        {
            Debug.Log("Không tìm thấy dữ liệu Save!");
            return;
        }

        selectedFileIndex = Mathf.Clamp(
            data.fileIndex,
            0,
            file_content_path.Length - 1
        );

        string content = readData.ReadFiles(
            file_content_path[selectedFileIndex]
        );

        progressBar.SetMaxWords(content.Length);

        typedTextAll = data.typedTextAll ?? "";
        generatedText = data.generatedText ?? "";

        currentCharInPage = Mathf.Clamp(
            data.currentCharInPage,
            0,
            generatedText.Length
        );

        currentPage = data.currentPage;

        totalChar = data.totalChar;
        totalWords = data.totalWords;


        readData.SetIndex(data.readDataIndex);

        stressBar.SetStressValue(data.stressValue);
        progressBar.SetProgress(data.progressValue);

        textComponentBlind.text = generatedText;
        textComponentBlind.color = hiddenColor;

        cursorVisible = true;
        cursorTimer = 0f;

        UpdateTypingProgress();

        Debug.Log("Đã khôi phục checkpoint!");
    }
}
