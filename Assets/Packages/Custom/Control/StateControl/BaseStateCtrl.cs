/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public abstract class BaseStateCtrl : MonoBehaviour {
		[StateControllerSelect]
		public StateController controller;
		public abstract void Capture(int uid);
		public abstract void Apply(int uid);
	}

	public class BaseStateCtrl<TValue> : BaseStateCtrl {
		[SerializeField]
		private TValue m_DefaultValue;

		[SerializeField, ReadOnly]
		private List<int> m_ValueKeys = new List<int>();
		[SerializeField, ReadOnly]
		private List<TValue> m_ValueValues = new List<TValue>();

		private TValue GetValue(int uid) {
			for (int index = 0, length = m_ValueKeys.Count; index < length; ++index) {
				if (m_ValueKeys[index] == uid) {
					return m_ValueValues[index];
				}
			}
			return m_DefaultValue;
		}

		private void SetValue(int uid, TValue value) {
			for (int index = 0, length = m_ValueKeys.Count; index < length; ++index) {
				if (m_ValueKeys[index] == uid) {
					m_ValueValues[index] = value;
					return;
				}
			}
			m_ValueKeys.Add(uid);
			m_ValueValues.Add(value);
		}

		private void Reset() {
			controller = GetComponentInParent<StateController>();
			m_DefaultValue = TargetValue;
			m_ValueKeys.Clear();
			m_ValueValues.Clear();
		}

		protected virtual TValue TargetValue { get; set; }

		public override void Capture(int uid) {
			SetValue(uid, TargetValue);
#if UNITY_EDITOR
			// 清理用不到的状态
			if (controller) {
				for (int index = m_ValueKeys.Count - 1; index >= 0; --index) {
					var key = m_ValueKeys[index];
					if (!controller.states.Exists(state => state.uid == key)) {
						m_ValueKeys.RemoveAt(index);
						m_ValueValues.RemoveAt(index);
					}
				}
			} else {
				m_ValueKeys.Clear();
				m_ValueValues.Clear();
			}
#endif
		}

		public override void Apply(int uid) {
			TargetValue = GetValue(uid);
		}
	}
}