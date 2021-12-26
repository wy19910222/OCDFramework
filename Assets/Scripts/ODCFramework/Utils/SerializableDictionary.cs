/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
	
	[SerializeField]
	private List<TKey> m_keys = new List<TKey>();
	[SerializeField]
	private List<TValue> m_values = new List<TValue>();

	public void OnBeforeSerialize() {
		m_keys.Clear();
		m_values.Clear();
		foreach (var item in this) {
			m_keys.Add(item.Key);
			m_values.Add(item.Value);
		}
	}

	public void OnAfterDeserialize() {
		Clear();
		for (int i = 0, count = Mathf.Min(m_keys.Count, m_values.Count); i < count; ++i) {
			TKey key = m_keys[i];
			if (!ContainsKey(key)) {
				Add(key, m_values[i]);
			}
		}
	}
}
