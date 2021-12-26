/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System.Collections.Generic;
using UnityEngine;

namespace StateControl {
	public abstract class BaseStateCtrl : MonoBehaviour {
		[StateControllerSelect]
		public StateController controller;
		public abstract void Capture(int uid);
		public abstract void Apply(int uid);
	}

	public class BaseStateCtrl<TValue> : BaseStateCtrl {
		[SerializeField]
		private TValue m_defaultValue;

		[SerializeField]
		private SerializableDictionary<int, TValue> m_values = new SerializableDictionary<int, TValue>();

		private TValue GetValue(int uid) {
			return m_values.ContainsKey(uid) ? m_values[uid] : m_defaultValue;
		}

		private void SetValue(int uid, TValue value) {
			m_values[uid] = value;
		}

		private void Reset() {
			controller = GetComponentInParent<StateController>();
			m_defaultValue = TargetValue;
			m_values.Clear();
		}

		protected virtual TValue TargetValue { get; set; }

		public override void Capture(int uid) {
			SetValue(uid, TargetValue);
#if UNITY_EDITOR
			// 清理用不到的状态
			if (controller) {
				var deleteUIDs = new List<int>();
				foreach (var pair in m_values) {
					if (!controller.states.Exists(state => state.uid == pair.Key)) {
						deleteUIDs.Add(pair.Key);
					}
				}
				foreach (var deleteUid in deleteUIDs) {
					m_values.Remove(deleteUid);
				}
			} else {
				m_values.Clear();
			}
#endif
		}

		public override void Apply(int uid) {
			TargetValue = GetValue(uid);
		}
	}
}