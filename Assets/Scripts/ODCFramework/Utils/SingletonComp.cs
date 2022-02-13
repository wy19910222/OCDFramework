using UnityEngine;
using UnityEngine.Serialization;

public class SingletonComp<T> : MonoBehaviour where T : SingletonComp<T> {
	private static T s_instance;
	public static T Instance {
		get {
			if (!s_instance) {
				var go = new GameObject("Singleton - " + typeof(T).Name);
				go.AddComponent<T>();
			}
			return s_instance;
		}
	}

	[SerializeField]
	protected bool m_dontDestroyOnLoad = true;

	private void Awake() {
		s_instance = this as T;
		if (m_dontDestroyOnLoad) {
			DontDestroyOnLoad(s_instance);
		}
		OnAwake();
	}

	protected virtual void OnAwake() {
	}
}
