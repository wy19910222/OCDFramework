/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateControl {

	[Serializable]
	public class State {
		public int uid;
		public string name;
		public string desc;

		public State(int uid, string name = "") {
			this.uid = uid;
			this.name = name;
		}
	}

	[Serializable]
	public class StateRelate {
		public const int TARGET_NONE = -1;
		public const int TARGET_SAME_INDEX = -2;
		public const int TARGET_SAME_NAME = -3;

		public int fromMask = -1;
		public int toMask = -1;
		public StateController controller;
		public int targetIndex = TARGET_NONE;
	}

	public class StateController : MonoBehaviour {
		private int AutoIncID {
			get {
				var id = 0;
				foreach (var state in states) {
					if (state.uid > id) {
						id = state.uid;
					}
				}
				return id + 1;
			}
		}

		public string title;
		public int initialIndex;
		public List<State> states = new List<State>();
		public List<StateRelate> relations = new List<StateRelate>();

		private Action m_onChange;

		void Reset() {
			title = "";
			initialIndex = 0;
			states.Clear();
			relations.Clear();
			m_index = 0;
			AddState();
			AddState();
		}

		private void Awake() {
			m_prevIndex = initialIndex;
			m_index = initialIndex;
			Apply();
			RelationApply();
		}

		public int StateCount => states.Count;

		private int m_index;
		public int Index {
			get => m_index;
			set {
				if (SetIndex(value)) {
					m_onChange?.Invoke();
				}
			}
		}
		public bool SetIndex(int value) {
			value = Mathf.Clamp(value, 0, StateCount - 1);
			if (value != m_index) {
				m_prevIndex = m_index;
				m_index = value;
				Apply();
				RelationApply();
				return true;
			}
			return false;
		}
		private int m_prevIndex;
		public int PrevIndex => m_prevIndex;

		public string State {
			get {
				var state = states[Index];
				return state?.name;
			}
			set {
				if (SetState(value)) {
					m_onChange?.Invoke();
				}
			}
		}
		public string PrevState {
			get {
				var state = states[PrevIndex];
				return state?.name;
			}
		}
		public bool SetState(string value) {
			return SetIndex(states.FindIndex(state => state.name == value));
		}

		public List<BaseStateCtrl> Targets {
			get {
				var targets = new List<BaseStateCtrl>();
				var comps = GetComponentsInChildren<BaseStateCtrl>(true);
				for (int i = 0, length = comps.Length; i < length; i++) {
					var target = comps[i];
					if (target.controller == this) {
						targets.Add(target);
					}
				}
				return targets;
			}
		}

		public void AddState(string stateName = "") {
			states.Add(new State(AutoIncID, stateName));
		}

		public void RemoveState(int index) {
			states.RemoveAt(index);
		}

		/**
		 * 记录状态
		 */
		public void Capture() {
			if (m_index >= 0 && m_index < StateCount) {
				var uid = states[m_index].uid;
				foreach (var item in Targets) {
					item.Capture(uid);
				}
			}
		}

		/**
		 * 应用状态
		 */
		public void Apply() {
			if (m_index >= 0 && m_index < StateCount) {
				var uid = states[m_index].uid;
				foreach (var item in Targets) {
					item.Apply(uid);
				}
			}
		}

		/**
		 * 关联控制
		 */
		private void RelationApply() {
			foreach (var relation in relations) {
				if (relation.controller && relation.targetIndex != StateRelate.TARGET_NONE
					&& ((1 << m_index) & relation.toMask) != 0 && ((1 << m_prevIndex) & relation.fromMask) != 0) {
					switch (relation.targetIndex) {
						case StateRelate.TARGET_SAME_INDEX:
							relation.controller.Index = Index;
							break;
						case StateRelate.TARGET_SAME_NAME:
							relation.controller.State = State;
							break;
						default:
							relation.controller.Index = relation.targetIndex;
							break;
					}
				}
			}
		}

		public void OnChange(Action action) {
			m_onChange += action;
		}

		public void OffChange(Action action) {
			m_onChange -= action;
		}
	}
}