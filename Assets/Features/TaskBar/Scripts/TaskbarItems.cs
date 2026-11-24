using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TaskbarIconItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Khung chứa bản thu nhỏ khi Hover")]
    [SerializeField] private RectTransform miniPreviewContainer; // Panel con nằm phía trên icon
    [SerializeField] private float previewScale = 0.25f;         // Tỷ lệ thu nhỏ của cửa sổ

    private WindowFrame _targetFrame;
    private GameObject _previewClone;

    private void Awake()
    {
        if (miniPreviewContainer != null)
            miniPreviewContainer.gameObject.SetActive(false);
    }

    public void Bind(WindowFrame targetFrame)
    {
        _targetFrame = targetFrame;
    }

    // 1. Click vào icon -> Hiện cửa sổ (nếu đang ẩn) và đưa lên lớp trên cùng
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_targetFrame == null) return;

        HidePreview();

        if (!_targetFrame.gameObject.activeSelf)
            _targetFrame.gameObject.SetActive(true);

        _targetFrame.Manager?.ActivateWindow(_targetFrame.Rect);
    }

    // 2. Di chuột vào -> Tạo bản sao thu nhỏ y hệt cửa sổ thật
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_targetFrame == null || miniPreviewContainer == null) return;

        // 1. Giữ miniPreviewContainer ở trạng thái TẮT trong lúc đang nhân bản và dọn script
        miniPreviewContainer.gameObject.SetActive(false);
        CreatePreviewSnapshot();

        // 2. Dọn sạch script xong hết rồi mới bật khung Preview lên
        miniPreviewContainer.gameObject.SetActive(true);
    }

    // 3. Rời chuột ra -> Xóa bản sao và ẩn khung
    public void OnPointerExit(PointerEventData eventData)
    {
        HidePreview();
    }

    private void CreatePreviewSnapshot()
    {
        if (_previewClone != null) Destroy(_previewClone);

        _previewClone = new GameObject("WindowPreviewClone", typeof(RectTransform));
        // Vì miniPreviewContainer đang tắt nên _previewClone cũng ở trạng thái Inactive
        _previewClone.transform.SetParent(miniPreviewContainer, false);

        RectTransform sourceRect = _targetFrame.GetComponent<RectTransform>();
        RectTransform cloneRect = _previewClone.GetComponent<RectTransform>();
        cloneRect.sizeDelta = sourceRect.sizeDelta;
        cloneRect.anchorMin = new Vector2(0.5f, 0.5f);
        cloneRect.anchorMax = new Vector2(0.5f, 0.5f);
        cloneRect.pivot = new Vector2(0.5f, 0.5f);
        cloneRect.anchoredPosition = Vector2.zero;
        cloneRect.localScale = Vector3.one * previewScale;

        if (_targetFrame.TryGetComponent<Image>(out var sourceBg))
        {
            Image cloneBg = _previewClone.AddComponent<Image>();
            cloneBg.sprite = sourceBg.sprite;
            cloneBg.color = sourceBg.color;
            cloneBg.type = sourceBg.type;
        }

        foreach (Transform child in sourceRect)
        {
            // Vì cha (cloneRect) đang ở trong cây Inactive, childClone sinh ra sẽ Inactive hoàn toàn -> Awake() KHÔNG BAO GIỜ chạy
            GameObject childClone = Instantiate(child.gameObject, cloneRect, false);

            // Xóa sạch mọi script logic (WindowClose, WindowDrag...) trước khi cha được bật lên
            foreach (var script in childClone.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (script is UnityEngine.EventSystems.UIBehaviour) continue;
                DestroyImmediate(script);
            }
        }
    }

    private void HidePreview()
    {
        if (_previewClone != null)
        {
            Destroy(_previewClone);
            _previewClone = null;
        }

        if (miniPreviewContainer != null)
            miniPreviewContainer.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_previewClone != null)
            Destroy(_previewClone);
    }
}