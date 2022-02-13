/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2022-02-04 23:55:18 002
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRoot : MonoBehaviour { }

public class Root<E> : BaseRoot where E : MonoBehaviour {
	private readonly Dictionary<Type, E> m_dic = new Dictionary<Type, E>();

	public T Add<T>() where T : E {
		var go = new GameObject();
		go.transform.SetParent(transform);
		T t = go.AddComponent<T>();
		m_dic[typeof(T)] = t;
		return t;
	}

	public T get<T>() where T : E {
		return m_dic[typeof(T)] as T;
	}
}
