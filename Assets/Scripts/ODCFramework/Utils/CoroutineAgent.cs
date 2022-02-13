/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2022-02-14 03:41:29 144
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CoroutineAgent : SingletonComp<CoroutineAgent> {
}

public static class CoroutineUtil {
	private static readonly ConditionalWeakTable<Coroutine, IEnumerator> s_coroutineMap = new ConditionalWeakTable<Coroutine, IEnumerator>();

	public static Coroutine StartCo(this MonoBehaviour owner, IEnumerator routine) {
		if (routine == null) return null;
		if (!owner) owner = CoroutineAgent.Instance;
		var co = owner.StartCoroutine(routine);
		s_coroutineMap.Add(co, routine);
		return co;
	}
	/**
	 * 停止原协程并从当前迭代开启新的协程
	 */
	public static Coroutine StartCo(this MonoBehaviour owner, Coroutine coroutine) {
		if (s_coroutineMap.TryGetValue(coroutine, out var routine)) {
			if (!owner) owner = CoroutineAgent.Instance;
			owner.StopCoroutine(routine);
			var co = owner.StartCoroutine(routine);
			s_coroutineMap.Add(co, routine);
			return co;
		}
		return null;
	}
	
	public static void StopCo(this MonoBehaviour owner, IEnumerator routine) {
		if (!owner) owner = CoroutineAgent.Instance;
		owner.StopCoroutine(routine);
	}
	public static void StopCo(this MonoBehaviour owner, Coroutine coroutine) {
		if (!owner) owner = CoroutineAgent.Instance;
		owner.StopCoroutine(coroutine);
	}

	public static void StopAllCos(this MonoBehaviour owner) {
		if (!owner) owner = CoroutineAgent.Instance;
		owner.StopAllCoroutines();
	}

	public static Coroutine MoveNext(this MonoBehaviour owner, IEnumerator routine) {
		StopCo(owner, routine);
		return StartCo(owner, routine);
	}

	public static void Flush(IEnumerator routine, int maxSteps = 999) {
		if (routine == null) return;
		
		bool hasNext = true;
		int steps = 0;
		while (hasNext && steps < maxSteps) {
			hasNext = routine.MoveNext();
			++steps;
		}
		if (steps >= maxSteps) {
			Debug.LogErrorFormat("Flush {0} steps!", steps);
		}
	}

	public static void Flush(Coroutine coroutine, int maxSteps = 999) {
		if (s_coroutineMap.TryGetValue(coroutine, out var routine)) {
			Flush(routine, maxSteps);
		}
	}

	public static Coroutine Late(this MonoBehaviour owner, Action callback) {
		return StartCo(owner, DoLate(callback));
	}
	private static IEnumerator DoLate(Action callback) {
		if (callback != null) {
			yield return new WaitForEndOfFrame();
			callback.Invoke();
		}
	}
	
	public static Coroutine Once(this MonoBehaviour owner, float delay, Action callback) {
		return StartCo(owner, IEOnce(delay, callback, false));
	}
	public static Coroutine LateOnce(this MonoBehaviour owner, float delay, Action callback) {
		return StartCo(owner, IEOnce(delay, callback, true));
	}
	private static IEnumerator IEOnce(float delay, Action callback, bool late) {
		if (callback != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(delay);
			callback.Invoke();
		}
	}
	
	public static Coroutine FrameOnce(this MonoBehaviour owner, int delay, Action callback) {
		return StartCo(owner, IEFrameOnce(delay, callback, false));
	}
	public static Coroutine LateFrameOnce(this MonoBehaviour owner, int delay, Action callback) {
		return StartCo(owner, IEFrameOnce(delay, callback, true));
	}
	private static IEnumerator IEFrameOnce(int delay, Action callback, bool late) {
		if (callback != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			for (int i = 0; i < delay; ++i) {
				yield return null;
			}
			callback.Invoke();
		}
	}

	public static Coroutine Wait(this MonoBehaviour owner, Func<bool> loopUntil, Action callback) {
		return Wait(owner, new WaitUntil(loopUntil), callback);
	}
	public static Coroutine LateWait(this MonoBehaviour owner, Func<bool> loopUntil, Action callback) {
		return LateWait(owner, new WaitUntil(loopUntil), callback);
	}
	public static Coroutine Wait(this MonoBehaviour owner, object instruction, Action callback) {
		return StartCo(owner, IEWait(instruction, callback, false));
	}
	public static Coroutine LateWait(this MonoBehaviour owner, object instruction, Action callback) {
		return StartCo(owner, IEWait(instruction, callback, true));
	}
	private static IEnumerator IEWait(object instruction, Action callback, bool late) {
		if (instruction != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			yield return instruction;
			callback?.Invoke();
		}
	}

	public static Coroutine Loop(this MonoBehaviour owner, float interval, Func<bool> loopUntil) {
		return StartCo(owner, IELoop(interval, loopUntil, false));
	}
	public static Coroutine LateLoop(this MonoBehaviour owner, float interval, Func<bool> loopUntil) {
		return StartCo(owner, IELoop(interval, loopUntil, true));
	}
	private static IEnumerator IELoop(float interval, Func<bool> loopUntil, bool late) {
		if (loopUntil != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			var instruction = new WaitForSeconds(interval);
			while (!loopUntil()) {
				yield return instruction;
			}
		}
	}
	
	public static Coroutine FrameLoop(this MonoBehaviour owner, int interval, Func<bool> loopUntil) {
		return StartCo(owner, IEFrameLoop(interval, loopUntil, false));
	}
	public static Coroutine LateFrameLoop(this MonoBehaviour owner, int interval, Func<bool> loopUntil) {
		return StartCo(owner, IEFrameLoop(interval, loopUntil, true));
	}
	private static IEnumerator IEFrameLoop(float interval, Func<bool> loopUntil, bool late) {
		if (loopUntil != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			while (!loopUntil()) {
				for (int i = 0; i < interval; ++i) {
					yield return null;
				}
			}
		}
	}
	
	public static Coroutine EndOfLag(this MonoBehaviour owner, Action callback = null) {
		return StartCo(owner, IEEndOfLag(callback));
	}
	private const int LAG_FRAME_COUNT_MAX = 20;
	private const int LAG_CHECK_FRAME_COUNT = 3;
	private const float LAG_CHECK_THRESHOLD = 0.00001F;
	private static IEnumerator IEEndOfLag(Action callback) {
		int maxFrame = Time.frameCount + Mathf.Max(LAG_FRAME_COUNT_MAX, LAG_CHECK_FRAME_COUNT);
		Queue<float> deltaTimeQueue = new Queue<float>();
		deltaTimeQueue.Enqueue(Time.deltaTime);
		yield return null;
		deltaTimeQueue.Enqueue(Time.deltaTime);
		while (Time.frameCount < maxFrame) {
			yield return null;
			deltaTimeQueue.Enqueue(Time.deltaTime);
			if (deltaTimeQueue.Count > LAG_CHECK_FRAME_COUNT) {
				deltaTimeQueue.Dequeue();
			}
			float variance = GetVariance(deltaTimeQueue);
			if (variance < LAG_CHECK_THRESHOLD) {
				break;
			}
		}
		callback?.Invoke();
	}
	private static float GetVariance(Queue<float> queue) {
		int count = queue.Count;
		float sum = 0;
		foreach (float deltaTime in queue) {
			sum += deltaTime;
		}
		float average = sum / count;
		sum = 0;
		foreach (float deltaTime in queue) {
			sum += Mathf.Pow(deltaTime - average, 2);
		}
		return sum / count;
	}
}