using UnityEngine;
using System.Collections;

/// <summary>
/// Ubh singleton mono behavior.
/// </summary>
public class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour {
    static T _Instance;

    /// <summary>
    /// Get singleton instance.
    /// </summary>
    public static T Instance {
        get {
            if (_Instance == null) {
                _Instance = FindObjectOfType<T>();

                if (_Instance == null) {
                    _Instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return _Instance;
        }
    }

    protected virtual void Awake() {
        if (this != Instance) {
            GameObject obj = this.gameObject;
            Destroy(this);
            Destroy(obj);
            Debug.LogWarning("has instance destory " + typeof(T).Name);
            return;
        }
    }
}
