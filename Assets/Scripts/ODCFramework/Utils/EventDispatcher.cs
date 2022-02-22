/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2022-02-13 02:01:26 789
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class EventListener {
	// 事件处理器
	public MulticastDelegate EventHandler { get; }

	// 事件处理器自带参数
	public object[] Args { get; }

	// 是否是一次性监听
	public bool IsOnce { get; }

	// 是否启用
	public bool Enabled { get; set; }

	public EventListener(MulticastDelegate eventHandler, object[] args, bool isOnce = false) {
		EventHandler = eventHandler;
		Args = args;
		IsOnce = isOnce;
	}
}

// 事件管理器
public static class EventDispatcher {
	private static readonly Dictionary<string, Dictionary<object, List<EventListener>>> m_dicEventListeners =
			new Dictionary<string, Dictionary<object, List<EventListener>>>();

	/**
	 * @Desc: 事件触发
	 * @param {eventName 事件名称}
	 * @param {args 可变参数数组}
	 * @return {void}
	 */
	public static void Emit(string eventName, params object[] args) {
		EmitEvent(eventName, args, true);
	}

	/**
	 * @Desc: 事件静默触发（不打印日志）
	 * @param {eventName 事件名称}
	 * @param {args 可变参数数组}
	 * @return {void}
	 */
	public static void EmitSilently(string eventName, params object[] args) {
		EmitEvent(eventName, args, false);
	}

	/**
	 * @Desc: 增加一个监听
	 * @param {eventName 事件名称}
	 * @param {owner 用于统一移除监听的观察者}
	 * @param {callback 事件处理器}
	 * @param {args 事件处理器自带参数}
	 * @param {insertFirst 是否插到最前面}
	 * @return {void}
	 */
	public static void On(string eventName, object owner, Action callback) {
		Add(eventName, owner, callback, null, false);
	}

	public static void On<T1>(string eventName, object owner, Action<T1> callback, object[] args = null) {
		Add(eventName, owner, callback, args, false);
	}

	public static void On<T1, T2>(string eventName, object owner, Action<T1, T2> callback, object[] args = null) {
		Add(eventName, owner, callback, args, false);
	}

	public static void On<T1, T2, T3>(string eventName, object owner, Action<T1, T2, T3> callback, object[] args = null) {
		Add(eventName, owner, callback, args, false);
	}

	public static void On<T1, T2, T3, T4>(string eventName, object owner, Action<T1, T2, T3, T4> callback,
		object[] args = null) {
		Add(eventName, owner, callback, args, false);
	}

	public static void On<T1, T2, T3, T4, T5>(string eventName, object owner, Action<T1, T2, T3, T4, T5> callback,
		object[] args = null) {
		Add(eventName, owner, callback, args, false);
	}

	/**
	 * @Desc: 增加一个单次监听
	 * @param {eventName 事件名称}
	 * @param {owner 用于统一移除监听的观察者}
	 * @param {callback 事件处理器}
	 * @return {void}
	 */
	public static void Once(string eventName, object owner, Action callback) {
		Add(eventName, owner, callback, null, true);
	}

	public static void Once<T1>(string eventName, object owner, Action<T1> callback, object[] args = null) {
		Add(eventName, owner, callback, args, true);
	}

	public static void Once<T1, T2>(string eventName, object owner, Action<T1, T2> callback, object[] args = null) {
		Add(eventName, owner, callback, args, true);
	}

	public static void Once<T1, T2, T3>(string eventName, object owner, Action<T1, T2, T3> callback, object[] args = null) {
		Add(eventName, owner, callback, args, true);
	}

	public static void Once<T1, T2, T3, T4>(string eventName, object owner, Action<T1, T2, T3, T4> callback,
		object[] args = null) {
		Add(eventName, owner, callback, args, true);
	}

	public static void Once<T1, T2, T3, T4, T5>(string eventName, object owner, Action<T1, T2, T3, T4, T5> callback,
		object[] args = null) {
		Add(eventName, owner, callback, args, true);
	}

	/**
	 * @Desc: 移除某个/某些监听器
	 * @param {eventName 事件名称}
	 * @param {owner 用于统一移除监听的观察者}
	 * @param {callback 事件处理器}
	 * @param {onceOnly 是否只移除单次监听}
	 * @return {void}
	 */
	public static void Off(string eventName, object owner, Action callback, bool onceOnly = false) {
		Remove(eventName, owner, callback, onceOnly);
	}

	public static void Off<T1>(string eventName, object owner, Action<T1> callback, bool onceOnly = false) {
		Remove(eventName, owner, callback, onceOnly);
	}

	public static void Off<T1, T2>(string eventName, object owner, Action<T1, T2> callback, bool onceOnly = false) {
		Remove(eventName, owner, callback, onceOnly);
	}

	public static void Off<T1, T2, T3>(string eventName, object owner, Action<T1, T2, T3> callback, bool onceOnly = false) {
		Remove(eventName, owner, callback, onceOnly);
	}

	public static void Off<T1, T2, T3, T4>(string eventName, object owner, Action<T1, T2, T3, T4> callback,
		bool onceOnly = false) {
		Remove(eventName, owner, callback, onceOnly);
	}

	public static void Off<T1, T2, T3, T4, T5>(string eventName, object owner, Action<T1, T2, T3, T4, T5> callback,
		bool onceOnly = false) {
		Remove(eventName, owner, callback, onceOnly);
	}

	/**
	 * @Desc: 移除某个观察者的所有监听器
	 * @param {owner 用于统一移除监听的观察者}
	 * @return {void}
	 */
	public static void OffAll(object owner) {
		RemoveAll(owner);
	}

	/**
	 * @Desc: 事件触发
	 * @param {eventName 事件名称}
	 * @param {args 可变参数数组}
	 * @param {logEvent 是否打印事件}
	 * @return {void}
	 */
	private static void EmitEvent(string eventName, object[] args, bool logEvent) {
		if (logEvent) {
			// 打印事件名和参数列表
			Debug.Log("FireEvent " + eventName + (args.Length > 0 ? ": " + args.Join() : ""));
		}

		// 如果没有对该事件的监听，则直接返回
		if (!m_dicEventListeners.ContainsKey(eventName)) {
			return;
		}

		List<EventListener> list = new List<EventListener>();
		Dictionary<object, List<EventListener>> dicListeners = m_dicEventListeners[eventName];
		// 用于移除的列表，临时存放空了的子列表对应的键
		List<object> deleteOwners = new List<object>();
		foreach (KeyValuePair<object, List<EventListener>> pair in dicListeners) {
			object owner = pair.Key;
			List<EventListener> listeners = pair.Value;
			// 收集所有要触发的监听
			list.AddRange(listeners);
			// 移除单次监听
			for (int index = listeners.Count - 1; index >= 0; --index) {
				if (listeners[index].IsOnce) {
					listeners.RemoveAt(index);
				}
			}

			// 如果已经空了，就加入到移除列表
			if (listeners.Count <= 0) {
				deleteOwners.Add(owner);
			}
		}

		// 移除空了的子列表
		foreach (object deleteOwner in deleteOwners) {
			dicListeners.Remove(deleteOwner);
		}

		// 如果已经空了，就移除空了的子字典
		if (dicListeners.Count <= 0) {
			m_dicEventListeners.Remove(eventName);
		}

		// 触发监听
		foreach (EventListener listener in list) {
			List<object> argList = new List<object>();
			if (listener.Args != null) {
				argList.AddRange(listener.Args);
			}

			if (args != null) {
				argList.AddRange(args);
			}

			try {
				MethodInfo method = listener.EventHandler.Method;
				ParameterInfo[] paramInfos = method.GetParameters();
				int paramLength = paramInfos.Length;
				int argCount = argList.Count;
				if (argCount > paramLength) {
					argList = argList.GetRange(0, paramLength);
				} else if (argCount < paramLength) {
					for (int i = argCount; i < paramLength; ++i) {
						Type type = paramInfos[i].ParameterType;
						argList.Add(type.IsValueType ? Activator.CreateInstance(type) : null);
					}
				}

				listener.EventHandler.DynamicInvoke(argList.ToArray());
			} catch (Exception e) {
				Debug.LogErrorFormat("FireEvent {0}{1} error with exception: {2}", eventName,
					args?.Length > 0 ? ": " + args.Join() : "", e);
			}
		}
	}

	/**
	 * @Desc: 加入新的监听器
	 * @param {eventName 事件名称}
	 * @param {owner 用于统一移除监听的观察者}
	 * @param {callback 事件处理器}
	 * @param {args 处理器自带参数}
	 * @param {isOnce 是否只监听一次}
	 * @param {insertFirst 是否优先级插到最高}
	 * @return {void}
	 */
	private static void Add(string eventName, object owner, MulticastDelegate callback, object[] args, bool isOnce) {
		if (!IsValidParams(eventName, owner, callback)) {
			return;
		}

		// 查找是否已存在监听
		EventListener listener = Find(eventName, owner, callback);
		if (listener != null) {
			Debug.LogError("Listener is already exist! " + eventName);
		} else {
			// 新建一个监听并加入到列表
			listener = new EventListener(callback, args, isOnce);
			List<EventListener> listeners = m_dicEventListeners[eventName][owner];
			listeners.Add(listener);
		}
	}

	/**
	 * @Desc: 移除某个/某些监听器
	 * @param {eventName 事件名称}
	 * @param {owner 用于统一移除监听的观察者}
	 * @param {callback 事件处理器}
	 * @param {onceOnly 是否只移除单次监听}
	 * @return {void}
	 */
	private static void Remove(string eventName, object owner, MulticastDelegate callback, bool onceOnly = false) {
		RemoveBy((_eventName, _owner, _listener) => {
			if (eventName != null && eventName != _eventName) {
				return false;
			}

			if (owner != null && owner != _owner) {
				return false;
			}

			if (callback != null && callback != _listener.EventHandler) {
				return false;
			}

			if (onceOnly && !_listener.IsOnce) {
				return false;
			}

			return true;
		});
	}

	/**
	 * @Desc: 移除某个观察者的所有监听器
	 * @param {owner 用于统一移除监听的观察者}
	 * @return {void}
	 */
	private static void RemoveAll(object owner) {
		List<string> deleteEventNames = new List<string>();
		foreach (KeyValuePair<string, Dictionary<object, List<EventListener>>> pair1 in m_dicEventListeners) {
			string eventName = pair1.Key;
			Dictionary<object, List<EventListener>> dicListeners = pair1.Value;
			dicListeners.Remove(owner);
			if (dicListeners.Count <= 0) {
				deleteEventNames.Add(eventName);
			}
		}

		foreach (string deleteEventName in deleteEventNames) {
			m_dicEventListeners.Remove(deleteEventName);
		}
	}

	/**
	 * @Desc: 移除某个监听
	 * @param {predicate 移除条件}
	 * @return {void}
	 */
	private static void RemoveBy(Func<string, object, EventListener, bool> predicate) {
		if (predicate == null) {
			return;
		}

		// 用于移除的列表，临时存放空了的子字典对应的键
		List<string> deleteEventNames = new List<string>();
		// 遍历总字典
		foreach (KeyValuePair<string, Dictionary<object, List<EventListener>>> pair1 in m_dicEventListeners) {
			string eventName = pair1.Key;
			Dictionary<object, List<EventListener>> dicListeners = pair1.Value;
			// 用于移除的列表，临时存放空了的子列表对应的键
			List<object> deleteOwners = new List<object>();
			foreach (KeyValuePair<object, List<EventListener>> pair2 in dicListeners) {
				object owner = pair2.Key;
				List<EventListener> listeners = pair2.Value;
				// 从后往前遍历移除
				for (int index = listeners.Count - 1; index >= 0; --index) {
					EventListener listener = listeners[index];
					if (predicate(eventName, owner, listener)) {
						listeners.RemoveAt(index);
					}
				}

				// 如果已经空了，就加入到移除列表
				if (listeners.Count <= 0) {
					deleteOwners.Add(owner);
				}
			}

			// 移除空了的子列表
			foreach (object deleteOwner in deleteOwners) {
				dicListeners.Remove(deleteOwner);
			}

			// 如果已经空了，就加入到移除列表
			if (dicListeners.Count <= 0) {
				deleteEventNames.Add(eventName);
			}
		}

		// 移除空了的子字典
		foreach (string deleteEventName in deleteEventNames) {
			m_dicEventListeners.Remove(deleteEventName);
		}
	}

	/**
	 * @Desc: 查找监听
	 * @param {eventName 事件名称}
	 * @param {owner 用于统一移除监听的观察者}
	 * @param {callback 事件处理器}
	 * @return {EventListener}
	 */
	private static EventListener Find(string eventName, object owner, MulticastDelegate callback) {
		Dictionary<object, List<EventListener>> dicListeners;
		// 如果没有eventName对应的字典，则新建一个
		if (m_dicEventListeners.ContainsKey(eventName)) {
			dicListeners = m_dicEventListeners[eventName];
		} else {
			dicListeners = new Dictionary<object, List<EventListener>>();
			m_dicEventListeners.Add(eventName, dicListeners);
		}

		List<EventListener> listeners;
		// 如果没有owner对应的列表，则新建一个
		if (dicListeners.ContainsKey(owner)) {
			listeners = dicListeners[owner];
		} else {
			listeners = new List<EventListener>();
			dicListeners.Add(owner, listeners);
		}

		// 遍历列表，查找监听
		foreach (EventListener listener in listeners) {
			if (listener.EventHandler == callback) {
				return listener;
			}
		}

		return null;
	}

	private static bool IsValidParams(string eventName, object owner, MulticastDelegate callback) {
		if (eventName == null) {
			Debug.LogError("Event name is null!");
			return false;
		}

		if (owner == null) {
			Debug.LogError("Owner is null!");
			return false;
		}

		if (callback == null) {
			Debug.LogError("Callback is null!");
			return false;
		}

		return true;
	}
}