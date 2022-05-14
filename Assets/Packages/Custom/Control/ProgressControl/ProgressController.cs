/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:18 824
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 14:22:18 819
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	[Serializable]
	public class ProgressRelate {
		public float minProgress = 0;
		public float maxProgress = 1;
		public ProgressController controller;
		public float targetMinProgress = 0;
		public float targetMaxProgress = 1;
	}
	
	[Serializable]
	public class ProgressRelateState {
		public const int TARGET_NONE = -1;
		
		public float minProgress = 0;
		public float maxProgress = 1;
		public StateController controller;
		public int targetIndex = TARGET_NONE;
	}
	
	[Serializable]
	public class ProgressRelateTrigger {
		public float minProgress = 0;
		public float maxProgress = 1;
		public BaseTriggerCtrl trigger;
		public bool triggered;
	}

	public class ProgressController : MonoBehaviour {

		public string title;
		public bool lazyInit;
		public float initialProgress;
		public List<ProgressRelate> relations = new List<ProgressRelate>();
		public List<ProgressRelateState> stateRelations = new List<ProgressRelateState>();
		public List<ProgressRelateTrigger> triggerRelations = new List<ProgressRelateTrigger>();

		private bool initialized;
		private Action m_OnChange;

		private void Reset() {
			title = "";
			initialProgress = 0;
			relations.Clear();
			stateRelations.Clear();
			triggerRelations.Clear();
			m_Progress = 0;
		}

		private void Awake() {
			if (!lazyInit) {
				Init();
			}
		}

		private void Init() {
			if (!initialized) {
				initialized = true;
				m_PrevProgress = initialProgress;
				m_Progress = initialProgress;
				Apply();
				RelationApply();
			}
		}

		private float m_Progress;
		public float Progress {
			get => m_Progress;
			set {
				if (lazyInit) {
					Init();
				}
				if (SetProgress(value)) {
					m_OnChange?.Invoke();
				}
			}
		}
		public bool SetProgress(float progress) {
			progress = Mathf.Clamp(progress, 0, 1);
			if (Math.Abs(progress - m_Progress) > Mathf.Epsilon) {
				m_PrevProgress = m_Progress;
				m_Progress = progress;
				Apply();
				RelationApply();
				return true;
			}
			return false;
		}
		private float m_PrevProgress;
		public float PrevProgress => m_PrevProgress;

		public List<BaseProgressCtrl> Targets {
			get {
				var targets = new List<BaseProgressCtrl>();
				var comps = GetComponentsInChildren<BaseProgressCtrl>(true);
				for (int i = 0, length = comps.Length; i < length; i++) {
					var target = comps[i];
					if (target.controller == this) {
						targets.Add(target);
					}
				}
				return targets;
			}
		}

		/**
		 * 记录状态
		 */
		public void Capture() {
			foreach (var item in Targets) {
				item.Capture(m_Progress);
			}
		}

		/**
		 * 应用状态
		 */
		public void Apply() {
			foreach (var item in Targets) {
				item.Apply(m_Progress);
			}
		}

		/**
		 * 关联控制
		 */
		private void RelationApply() {
			foreach (var relation in relations) {
				if (relation.controller && relation.minProgress <= m_Progress && relation.maxProgress >= m_Progress) {
					var rate = (m_Progress - relation.minProgress) / (relation.maxProgress - relation.minProgress);
					relation.controller.Progress = relation.targetMinProgress + rate * (relation.targetMaxProgress - relation.targetMinProgress);
				}
			}
			foreach (var relation in stateRelations) {
				if (relation.controller && relation.minProgress <= m_Progress && relation.maxProgress >= m_Progress) {
					relation.controller.Index = relation.targetIndex;
				}
			}
			foreach (var relation in triggerRelations) {
				if (relation.trigger && relation.minProgress <= m_Progress && relation.maxProgress >= m_Progress) {
					if (!relation.triggered) {
						relation.triggered = true;
						relation.trigger.Trigger();
					}
				} else {
					if (relation.triggered) {
						relation.triggered = false;
					}
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