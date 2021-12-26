/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CoroutineUtil {
	private static readonly ConditionalWeakTable<Coroutine, IEnumerator> s_coroutineMap = new ConditionalWeakTable<Coroutine, IEnumerator>();

	public static Coroutine StartCo(IEnumerator routine, MonoBehaviour owner) {
		if (routine == null) return null;
		if (!owner) return null;
		
		var co = owner.StartCoroutine(routine);
		s_coroutineMap.Add(co, routine);
		return co;
	}
	/**
	 * 停止原协程并从当前迭代开启新的协程
	 */
	public static Coroutine StartCo(Coroutine coroutine, MonoBehaviour owner) {
		if (!owner) return null;
		
		if (s_coroutineMap.TryGetValue(coroutine, out var routine)) {
			owner.StopCoroutine(routine);
			var co = owner.StartCoroutine(routine);
			s_coroutineMap.Add(co, routine);
			return co;
		}
		return null;
	}
	
	
	public static void StopCo(IEnumerator routine, MonoBehaviour owner) {
		if (!owner) return;
		
		owner.StopCoroutine(routine);
	}
	public static void StopCo(Coroutine coroutine, MonoBehaviour owner) {
		if (!owner) return;
		
		owner.StopCoroutine(coroutine);
	}

	public static void StopAllCos(MonoBehaviour owner) {
		if (!owner) return;
		
		owner.StopAllCoroutines();
	}

	public static Coroutine MoveNext(IEnumerator routine, MonoBehaviour owner) {
		StopCo(routine, owner);
		return StartCo(routine, owner);
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

	public static Coroutine Late(Action callback, MonoBehaviour owner) {
		return StartCo(DoLate(callback), owner);
	}
	private static IEnumerator DoLate(Action callback) {
		if (callback != null) {
			yield return new WaitForEndOfFrame();
			callback.Invoke();
		}
	}
	
	public static Coroutine Once(float delay, Action callback, MonoBehaviour owner) {
		return StartCo(DoOnce(delay, callback, false), owner);
	}
	public static Coroutine LateOnce(float delay, Action callback, MonoBehaviour owner) {
		return StartCo(DoOnce(delay, callback, true), owner);
	}
	private static IEnumerator DoOnce(float delay, Action callback, bool late) {
		if (callback != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(delay);
			callback.Invoke();
		}
	}
	
	public static Coroutine FrameOnce(int delay, Action callback, MonoBehaviour owner) {
		return StartCo(DoFrameOnce(delay, callback, false), owner);
	}
	public static Coroutine LateFrameOnce(int delay, Action callback, MonoBehaviour owner) {
		return StartCo(DoFrameOnce(delay, callback, true), owner);
	}
	private static IEnumerator DoFrameOnce(int delay, Action callback, bool late) {
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

	public static Coroutine Wait(Func<bool> loopUntil, Action callback, MonoBehaviour owner) {
		return StartCo(DoWait(loopUntil, callback, false), owner);
	}
	public static Coroutine LateWait(Func<bool> loopUntil, Action callback, MonoBehaviour owner) {
		return StartCo(DoWait(loopUntil, callback, true), owner);
	}
	private static IEnumerator DoWait(Func<bool> loopUntil, Action callback, bool late) {
		if (loopUntil != null) {
			if (late) {
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitUntil(loopUntil);
			callback?.Invoke();
		}
	}

	public static Coroutine Loop(float interval, Func<bool> loopUntil, MonoBehaviour owner) {
		return StartCo(DoLoop(interval, loopUntil, false), owner);
	}
	public static Coroutine LateLoop(float interval, Func<bool> loopUntil, MonoBehaviour owner) {
		return StartCo(DoLoop(interval, loopUntil, true), owner);
	}
	private static IEnumerator DoLoop(float interval, Func<bool> loopUntil, bool late) {
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
	
	public static Coroutine FrameLoop(int interval, Func<bool> loopUntil, MonoBehaviour owner) {
		return StartCo(DoFrameLoop(interval, loopUntil, false), owner);
	}
	public static Coroutine LateFrameLoop(int interval, Func<bool> loopUntil, MonoBehaviour owner) {
		return StartCo(DoFrameLoop(interval, loopUntil, true), owner);
	}
	private static IEnumerator DoFrameLoop(float interval, Func<bool> loopUntil, bool late) {
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
	
	public static Coroutine EndOfLag(Action callback, MonoBehaviour owner) {
		return StartCo(DoEndOfLag(callback, owner), owner);
	}
	private static IEnumerator DoEndOfLag(Action callback, MonoBehaviour owner) {
		if (callback != null) {
			yield return WaitForEndOfLag(owner);
			callback.Invoke();
		}
	}
	public static Coroutine WaitForEndOfLag(MonoBehaviour owner) {
		return StartCo(DoWaitForEndOfLag(), owner);
	}
	
	private const int LAG_FRAME_COUNT_MAX = 20;
	private const int LAG_CHECK_FRAME_COUNT = 3;
	private const float LAG_CHECK_THRESHOLD = 0.00001F;
	private static IEnumerator DoWaitForEndOfLag() {
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