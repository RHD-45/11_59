
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(TMP_Text))]
public class TypeWriterText : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private TMP_Text textComponentBlind;
    [SerializeField] private StressBar stressBar;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private GameSaveManager saveManager;
    [SerializeField] private TMP_Text saveStatusText;
    private WindowFrame windowFrame;
    private IDisposable anyKeySubscription;
    private bool saved;
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

    private bool isKeyA = true;
    private int currentPage = 1;
    private int currentCharInPage = 0;

    public int totalChar = 0;

    private float stressPoint = 0.048f;

    public event Action OnBTLChanged;

    private void OnEnable()
    {
        anyKeySubscription = InputSystem.onAnyButtonPress.Call(
            HandleAnyButtonPress
        );
    }

    private void OnDisable()
    {
        anyKeySubscription?.Dispose();
        anyKeySubscription = null;
    }

    void Start()
    {
        selectedFileIndex = UnityEngine.Random.Range(
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

        cursorTimer += Time.deltaTime;

        if (cursorTimer >= cursorBlinkSpeed)
        {
            cursorTimer = 0f;
            cursorVisible = !cursorVisible;

            UpdateTypingProgress();
        }
    }

    private void HandleAnyButtonPress(InputControl control)
    {
        if (!(control.device is Keyboard keyboard))
        {
            return;
        }

        if (!(control is KeyControl))
        {
            return;
        }

        if (GameManager.State != GameState.Playing)
        {
            return;
        }

        if (windowFrame == null || !windowFrame.IsFocused)
        {
            return;
        }

        if (!stressBar.GetCanType())
        {
            return;
        }


        // if (keyboard.shiftKey.isPressed &&           //fix sau
        //     control == keyboard.sKey)
        // {
        //     SaveGame();
        //     return;
        // }

        KeyPressSuccess();
    }

    private void KeyPressSuccess()
    {
        if (saved)
        {
            saved = false;
            saveStatusText.text = "Save*";
            saveStatusText.color = Color.black;
        }

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

        isKeyA = !isKeyA;

        currentCharInPage++;
        totalChar++;

        cursorVisible = true;
        cursorTimer = 0f;

        UpdateTypingProgress();

        progressBar.UpdateProgress(totalChar);
        stressBar.SetStress(stressPoint);

        OnBTLChanged?.Invoke();

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
        saved = true;
        GameSaveData data = new GameSaveData();

        data.typedTextAll = typedTextAll;
        data.generatedText = generatedText;

        data.currentCharInPage = currentCharInPage;
        data.currentPage = currentPage;

        data.totalChar = totalChar;
        data.totalWords = totalWords;

        data.readDataIndex = readData.GetIndex();

        data.isKeyA = isKeyA;
        data.fileIndex = selectedFileIndex;

        data.stressValue = stressBar.GetStress();
        data.progressValue = progressBar.GetProgress();

        saveStatusText.text = "Saved";
        saveStatusText.color = Color.red;

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

        isKeyA = data.isKeyA;

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

    public float GetBTLProgress()
    {
        return progressBar.GetProgress();
    }

    public void RestoreLastCheckpoint()
    {
        if (saveManager != null && saveManager.HasSave())
        {
            LoadGame();
        }
        else
        {
            ResetToBeginning();
        }
    }

    private void ResetToBeginning()
    {
        selectedFileIndex = 0;

        string content = readData.ReadFiles(
            file_content_path[selectedFileIndex]
        );

        typedTextAll = "";
        totalWords = 0;
        totalChar = 0;
        currentPage = 1;
        isKeyA = true;

        stressBar.SetStressValue(0f);
        progressBar.SetMaxWords(content.Length);

        GenerateRandomText();

        OnBTLChanged?.Invoke();
    }
}
