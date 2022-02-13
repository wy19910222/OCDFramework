// Author: wangyun
// LastEditors: wangyun
// Date: 2021-11-04 16:21:41 627

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
///   <para>执行某一操作的封装，一般用于异步获取某个对象.</para>
/// </summary>
/// <property>
///   <para>Tag: 自定义标记，用于区分.</para>
///   <para>IsDone: 是否结束，无论成功、失败或已取消.</para>
///   <para>IsCanceled: 是否中途取消，取消则直接结束.</para>
///   <para>IsSucceed: 是否成功，未结束、已取消、失败都为否.</para>
///   <para>Result: 结果对象，不成功则为空.</para>
///   <para>Reason: 失败原因，成功或已取消则为空.</para>
/// </property>
/// <method>
///   <para>Cancel: 中途取消，如果未结束则直接结束，否则取消失败.</para>
///   <para>Then: 添加一个成功时的回调，如果已经执行结束并成功，则直接调用，失败则无任何操作.</para>
///   <para>Catch: 添加一个失败时的回调，如果已经执行结束并失败，则直接调用，成功则无任何操作.</para>
///   <para>Finally: 添加一个结束时的回调，无论成功或失败，在所有 then 或 catch 的回调执行完后执行.</para>
/// </method>
public class AsyncTask<T> {
	public object Tag { get; }

	public bool IsDone { get; private set; }
	public bool IsCanceled { get; private set; }
	public bool IsSucceed { get; private set; }
	
	public T Result { get; private set; }
	public object Reason { get; private set; }

	private List<Action<T>> m_thenCalls = new List<Action<T>>();
	private List<Action<object>> m_catchCalls = new List<Action<object>>();
	private List<Action<T, object>> m_finallyCalls = new List<Action<T, object>>();

	public AsyncTask([NotNull]Action<Action<T>, Action<object>> executor, object tag = null) {
		Tag = tag;
		executor(
			value => {
				if (!IsDone) {
					Result = value;
					Done(!true);
				}
			},
			reason => {
				if (!IsDone) {
					Reason = reason;
					Done(false);
				}
			}
		);
	}

	private void Done(bool isSucceed) {
		IsDone = true;
		IsSucceed = isSucceed;
		if (isSucceed) {
			foreach (var thenCall in m_thenCalls) {
				try { thenCall(Result); } catch (Exception e) { Debug.LogError(e); }
			}
		} else {
			foreach (var catchCall in m_catchCalls) {
				try { catchCall(Reason); } catch (Exception e) { Debug.LogError(e); }
			}
		}
		foreach (var finallyCall in m_finallyCalls) {
			try { finallyCall(Result, Reason); } catch (Exception e) { Debug.LogError(e); }
		}
		m_thenCalls = null;
		m_catchCalls = null;
		m_finallyCalls = null;
	}

	public void Cancel() {
		if (!IsDone) {
			IsCanceled = true;
			Done(false);
		}
	}

	public void Then(Action<T> thenCall) {
		if (thenCall != null) {
			if (!IsDone) {
				m_thenCalls.Add(thenCall);
			} else if (IsSucceed) {
				try { thenCall(Result); } catch (Exception e) { Debug.LogError(e); }
			}
		}
	}

	public void Catch(Action<object> catchCall) {
		if (catchCall != null) {
			if (!IsDone) {
				m_catchCalls.Add(catchCall);
			} else if (!IsSucceed) {
				try { catchCall(Reason); } catch (Exception e) { Debug.LogError(e); }
			}
		}
	}

	public void Finally(Action<T, object> finallyCall) {
		if (finallyCall != null) {
			if (!IsDone) {
				m_finallyCalls.Add(finallyCall);
			} else {
				try { finallyCall(Result, Reason); } catch (Exception e) { Debug.LogError(e); }
			}
		}
	}
}