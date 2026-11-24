using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    // Prefab mẫu dùng để tạo đối tượng trong pool
    [SerializeField] protected T prefab = null;

    // Số lượng đối tượng khởi tạo sẵn khi bắt đầu
    [SerializeField] private int Pool_Size = 10;

    // Container chứa các đối tượng đang nhàn rỗi trong pool
    protected Transform worldPoolContainer;

    // Hàng đợi chứa các đối tượng sẵn sàng để tái sử dụng
    protected Queue<T> pool;

    // Khởi tạo container và sinh sẵn các đối tượng vào hàng đợi
    protected virtual void Awake()
    {
        SetupPoolContainer();

        pool = new Queue<T>();
        if (prefab == null)
        {
            return;
        }

        for (int i = 0; i < Pool_Size; i++)
        {
            T instance = CreateNewInstance();
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    // Tạo container chứa đối tượng pool (lớp con UI có thể override để đặt trong Canvas)
    protected virtual void SetupPoolContainer()
    {
        GameObject objContainer = new GameObject("[WorldPool] " + gameObject.name);
        worldPoolContainer = objContainer.transform;
    }

    // Sinh một đối tượng mới từ prefab
    protected virtual T CreateNewInstance()
    {
        T instance = Instantiate(prefab, worldPoolContainer);
        return instance;
    }

    // Lấy một đối tượng ra khỏi pool để sử dụng (tạo mới nếu pool đang rỗng)
    public virtual T GetObject()
    {
        if (prefab == null)
        {
            Debug.LogWarning("[" + name + "] Chưa gán Prefab cho ObjectPool!");
            return null;
        }

        T instance = pool.Count > 0 ? pool.Dequeue() : CreateNewInstance();
        instance.gameObject.SetActive(true);
        return instance;
    }

    // Trả đối tượng về lại pool và tắt hiển thị
    public virtual void ReturnObject(T instance)
    {
        if (instance == null || !instance.gameObject.activeSelf)
        {
            return;
        }

        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }

    // Xóa container pool khi object quản lý bị hủy
    protected virtual void OnDestroy()
    {
        if (worldPoolContainer != null && worldPoolContainer != transform)
        {
            Destroy(worldPoolContainer.gameObject);
        }
    }
}