/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
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

	[Serializable]
	public class StateRelateProgress {
		public int fromMask = -1;
		public int toMask = -1;
		public ProgressController controller;
		public float targetProgress;
	}

	[Serializable]
	public class StateRelateTrigger {
		public int fromMask = -1;
		public int toMask = -1;
		public BaseTriggerCtrl trigger;
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
		public List<StateRelateProgress> progressRelations = new List<StateRelateProgress>();
		public List<StateRelateTrigger> triggerRelations = new List<StateRelateTrigger>();

		private Action m_OnChange;

		private void Reset() {
			title = "";
			initialIndex = 0;
			states.Clear();
			relations.Clear();
			progressRelations.Clear();
			triggerRelations.Clear();
			m_Index = 0;
			AddState();
			AddState();
		}

		private void Awake() {
			m_PrevIndex = initialIndex;
			m_Index = initialIndex;
			Apply();
			RelationApply();
		}

		public int StateCount => states.Count;

		private int m_Index;
		public int Index {
			get => m_Index;
			set {
				if (SetIndex(value)) {
					m_OnChange?.Invoke();
				}
			}
		}
		public bool SetIndex(int value) {
			value = Mathf.Clamp(value, 0, StateCount - 1);
			if (value != m_Index) {
				m_PrevIndex = m_Index;
				m_Index = value;
				Apply();
				RelationApply();
				return true;
			}
			return false;
		}
		private int m_PrevIndex;
		public int PrevIndex => m_PrevIndex;

		public string State {
			get {
				var state = states[Index];
				return state?.name;
			}
			set {
				if (SetState(value)) {
					m_OnChange?.Invoke();
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
			if (m_Index >= 0 && m_Index < StateCount) {
				var uid = states[m_Index].uid;
				foreach (var item in Targets) {
					item.Capture(uid);
				}
			}
		}

		/**
		 * 应用状态
		 */
		public void Apply() {
			if (m_Index >= 0 && m_Index < StateCount) {
				var uid = states[m_Index].uid;
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
					&& ((1 << m_Index) & relation.toMask) != 0 && ((1 << m_PrevIndex) & relation.fromMask) != 0) {
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
			foreach (var relation in progressRelations) {
				if (relation.controller && ((1 << m_Index) & relation.toMask) != 0 && ((1 << m_PrevIndex) & relation.fromMask) != 0) {
					relation.controller.Progress = relation.targetProgress;
				}
			}
			foreach (var relation in triggerRelations) {
				if (relation.trigger && ((1 << m_Index) & relation.toMask) != 0 && ((1 << m_PrevIndex) & relation.fromMask) != 0) {
					relation.trigger.Trigger();
				}
			}
		}

		public void OnChange(Action action) {
			m_OnChange += action;
		}

		public void OffChange(Action action) {
			m_OnChange -= action;
		}
	}
}