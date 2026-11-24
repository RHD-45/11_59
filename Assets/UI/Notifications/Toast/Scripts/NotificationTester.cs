using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NotificationTester : MonoBehaviour
{
    // Demo configurations
    [SerializeField] private bool playDemoOnStart = true;
    [SerializeField] private float demoInterval = 1.2f;

    private void Start()
    {
        if (playDemoOnStart)
        {
            StartCoroutine(RunStartupDemo());
        }
    }

    // Hotkeys 1-5 and 0 for Play Mode testing
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TestSystemNotification();
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            TestPowerCutWarning();
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            TestAssignmentNotification();
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            TestZaloNotification();
        }
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            TestQueueOverflow();
        }
        else if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            TestClearAll();
        }
    }

    private IEnumerator RunStartupDemo()
    {
        yield return new WaitForSeconds(0.5f);
        TestSystemNotification();

        yield return new WaitForSeconds(demoInterval);
        TestAssignmentNotification();

        yield return new WaitForSeconds(demoInterval);
        TestZaloNotification();
    }

    // Test cases
    [ContextMenu("Test 1 - System Notification")]
    public void TestSystemNotification()
    {
        ToastData data = new ToastData(
            ToastType.System,
            "Windows Security",
            "Pin laptop chỉ còn 5%. Nhớ lưu bài tập lớn trước khi sập nguồn!",
            6f,
            true
        );
        EventBroker.onToastRequested?.Invoke(data);
    }

    [ContextMenu("Test 2 - Power Cut Warning")]
    public void TestPowerCutWarning()
    {
        ToastData data = new ToastData(
            ToastType.PowerCutWarning,
            "CẢNH BÁO ĐIỆN ÁP!",
            "Nguồn điện không ổn định! Sắp cúp điện trong 1 giây...",
            2f,
            false
        );
        EventBroker.onToastRequested?.Invoke(data);
    }

    [ContextMenu("Test 3 - New BTN Notification")]
    public void TestAssignmentNotification()
    {
        ToastData data = new ToastData(
            ToastType.Assignment,
            "BKeL - Bài tập mới",
            "Quiz Giải tích 2 vừa mở. Hạn nộp trước 23:59 hôm nay!",
            6f,
            true
        );
        EventBroker.onToastRequested?.Invoke(data);
    }

    [ContextMenu("Test 4 - Zalo Invite")]
    public void TestZaloNotification()
    {
        ToastData data = new ToastData(
            ToastType.Zalo,
            "Zalo - Nhóm Đồ Án",
            "Ê tối nay ra quán làm vài chai không? BTL mai tính!",
            6f,
            true
        );
        EventBroker.onToastRequested?.Invoke(data);
    }

    [ContextMenu("Test 5 - Queue Overflow (6 Toasts)")]
    public void TestQueueOverflow()
    {
        for (int i = 1; i <= 6; i++)
        {
            ToastType toastType = (ToastType)(i % 4);
            ToastData data = new ToastData(
                toastType,
                "Thông báo hàng đợi #" + i,
                "Kiểm tra xếp chồng VerticalLayoutGroup và Queue (" + i + "/6).",
                5f,
                true
            );
            EventBroker.onToastRequested?.Invoke(data);
        }
    }

    [ContextMenu("Test 0 - Clear All Toasts")]
    public void TestClearAll()
    {
        EventBroker.onClearAllToasts?.Invoke();
    }
}
