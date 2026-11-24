using UnityEngine;

public class SystemToastPool : ObjectPool<SystemToast>
{
    protected override void SetupPoolContainer()
    {
        GameObject poolHolder = new GameObject("[Pool] SystemToast");
        RectTransform rect = poolHolder.AddComponent<RectTransform>();
        rect.SetParent(transform, false);
        worldPoolContainer = rect;
    }

    protected override SystemToast CreateNewInstance()
    {
        SystemToast instance = Instantiate(prefab, worldPoolContainer, false);
        return instance;
    }

    public override void ReturnObject(SystemToast instance)
    {
        if (instance == null)
        {
            return;
        }

        instance.ResetForPool();
        instance.transform.SetParent(worldPoolContainer, false);
        base.ReturnObject(instance);
    }
}
