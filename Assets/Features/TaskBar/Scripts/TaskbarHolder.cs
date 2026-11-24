using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskbarHolder : MonoBehaviour
{
    [SerializeField] private List<Transform> taskbars = new List<Transform>();
    [SerializeField] private List<Transform> taskbarsIcons = new List<Transform>();

    [Header("Tham chiếu hệ thống")]
    [SerializeField] private AssignmentSpawner assignmentSpawner;
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private Transform iconPrefabs;
    [SerializeField] private Transform FatherPrefabs;

    [Header("Cấu hình khoảng cách & Animation")]
    [SerializeField] private float sizeFig = 100f;
    [SerializeField] private float padding = 10f;
    [SerializeField] private float animDuration = 0.25f;

    private Dictionary<Transform, Coroutine> _activeTweens = new Dictionary<Transform, Coroutine>();

    private void Start()
    {
        int pairCount = Mathf.Min(taskbars.Count, taskbarsIcons.Count);
        for (int i = 0; i < pairCount; i++)
        {
            Transform winTrans = taskbars[i];
            Transform iconTrans = taskbarsIcons[i];

            if (winTrans != null && iconTrans != null)
            {
                if (winTrans.TryGetComponent<WindowFrame>(out var windowFrame) &&
                    iconTrans.TryGetComponent<TaskbarIconItem>(out var iconItem))
                {
                    iconItem.Bind(windowFrame);
                }
            }
        }

        RecalculateAndAnimate();
    }
    private void OnEnable()
    {
        if (assignmentSpawner != null) assignmentSpawner.OnSpawning += AddTaskbars;
        if (windowManager != null) windowManager.OnWindowClosed += RemoveTaskbar;
    }

    private void OnDisable()
    {
        if (assignmentSpawner != null) assignmentSpawner.OnSpawning -= AddTaskbars;
        if (windowManager != null) windowManager.OnWindowClosed -= RemoveTaskbar;

        StopAllCoroutines();
        _activeTweens.Clear();
    }

    private void AddTaskbars(Transform windowTrans)
    {
        Transform newIcon = Instantiate(iconPrefabs, FatherPrefabs);

        // 1. Lấy Sprite từ Header/Icon của cửa sổ gán vào Image của icon trên Taskbar
        Transform windowIconObj = windowTrans.Find("Header/Icon");
        if (windowIconObj != null && windowIconObj.TryGetComponent<Image>(out var sourceImage))
        {
            if (newIcon.TryGetComponent<Image>(out var targetImage))
                targetImage.sprite = sourceImage.sprite;
        }

        // 2. Kết nối icon với WindowFrame của cửa sổ để xử lý Click & Hover Preview
        if (windowTrans.TryGetComponent<WindowFrame>(out var windowFrame) &&
            newIcon.TryGetComponent<TaskbarIconItem>(out var iconItem))
        {
            iconItem.Bind(windowFrame);
        }

        // 3. Đặt vị trí khởi đầu bên phải cùng và thu nhỏ về 0 để chạy hiệu ứng nảy lên
        float step = sizeFig + padding;
        float startX = (taskbarsIcons.Count * step) / 2f;
        newIcon.localPosition = new Vector3(startX, 0f, 0f);
        newIcon.localScale = Vector3.zero;

        taskbars.Add(windowTrans);
        taskbarsIcons.Add(newIcon);

        // 4. Tính lại vị trí căn giữa cho toàn bộ dãy icon và chạy animation
        RecalculateAndAnimate();
    }

    private void RemoveTaskbar(Transform windowTrans)
    {
        int index = taskbars.IndexOf(windowTrans);
        if (index == -1) return;

        Transform iconToRemove = taskbarsIcons[index];
        taskbars.RemoveAt(index);
        taskbarsIcons.RemoveAt(index);

        // Thu nhỏ icon bị xóa về 0 rồi mới Destroy
        if (iconToRemove != null)
        {
            StartIconTween(iconToRemove, iconToRemove.localPosition, Vector3.zero, destroyOnComplete: true);
        }

        // Các icon còn lại trượt mượt vào giữa lấp chỗ trống
        RecalculateAndAnimate();
    }

    private void RecalculateAndAnimate()
    {
        int count = taskbarsIcons.Count;
        if (count == 0) return;

        float step = sizeFig + padding;
        float firstX = -((count - 1) * step) / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform icon = taskbarsIcons[i];
            Vector3 targetPos = new Vector3(firstX + i * step, 0f, 0f);
            StartIconTween(icon, targetPos, Vector3.one, destroyOnComplete: false);
        }
    }

    private void StartIconTween(Transform target, Vector3 targetPos, Vector3 targetScale, bool destroyOnComplete)
    {
        if (_activeTweens.TryGetValue(target, out Coroutine running) && running != null)
        {
            StopCoroutine(running);
        }

        _activeTweens[target] = StartCoroutine(AnimateIconRoutine(target, targetPos, targetScale, destroyOnComplete));
    }

    private IEnumerator AnimateIconRoutine(Transform target, Vector3 targetPos, Vector3 targetScale, bool destroyOnComplete)
    {
        Vector3 startPos = target.localPosition;
        Vector3 startScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            if (target == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);

            target.localPosition = Vector3.Lerp(startPos, targetPos, t);
            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        if (target != null)
        {
            target.localPosition = targetPos;
            target.localScale = targetScale;
            _activeTweens.Remove(target);

            if (destroyOnComplete)
                Destroy(target.gameObject);
        }
    }
}