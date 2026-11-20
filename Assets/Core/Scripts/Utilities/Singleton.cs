using UnityEngine;

// static instance overrides current instance
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance {get; private set;}
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
        return;
    }
}

//singleton destroys new instances, leaving original intact
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}


