/*
 * @Author: wangyun
 * @CreateTime: 2022-04-19 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-04-19 16:21:41 627
 */

using System;
using System.Collections;
using UnityEngine;

namespace Control {
	public abstract class BaseTriggerCtrl : MonoBehaviour {
		public string title;
		public bool autoTrigger;
		public float triggerDelay;
		public bool validInactive;
		
		private Action m_OnTrigger;

		protected void Start() {
			if (autoTrigger) {
				Trigger();
			}
		}

		[ContextMenu("Trigger")]
		public void Trigger() {
			if (enabled && (validInactive || gameObject.activeInHierarchy)) {
#if UNITY_EDITOR
				if (triggerDelay > 0 && Application.isPlaying) {
#else
				if (triggerDelay > 0) {
#endif
					StartCoroutine(IETrigger());
				} else {
					DoTrigger();
					m_OnTrigger?.Invoke();
				}
			}
		}

		private IEnumerator IETrigger() {
			yield return new WaitForSeconds(triggerDelay);
			DoTrigger();
			m_OnTrigger?.Invoke();
		}
		
		protected abstract void DoTrigger();
		
		public void OnTrigger(Action action) {
			m_OnTrigger += action;
		}

		public void OffTrigger(Action action) {
			m_OnTrigger -= action;
		}
	}
}