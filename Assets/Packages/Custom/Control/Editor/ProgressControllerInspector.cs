/*
 * @Author: wangyun
 * @CreateTime: 2022-04-19 01:21:57 858
 * @LastEditor: wangyun
 * @EditTime: 2022-04-19 01:21:57 858
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace Control {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ProgressController), true)]
	public class ProgressControllerInspector : Editor {
		private static readonly Color BTN_NORMAL_COLOR = Color.white;
		private static readonly Color BTN_CHECKED_COLOR = Color.cyan * 0.8F;
		private static readonly GUILayoutOption BTN_WIDTH_OPTION = GUILayout.Width(80F);
		private static readonly GUILayoutOption BTN_DOUBLE_HEIGHT_OPTION = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);

		protected ProgressController m_ProgressController;
		private bool m_BriefMode = true;
		private bool m_DisableCtrl;

		private void OnEnable() {
			m_ProgressController = target as ProgressController;
		}

		public override void OnInspectorGUI() {
			DrawTitle();
			DrawProgress();
			if (!m_BriefMode) {
				DrawRelations(m_ProgressController.relations);
				DrawStateRelations(m_ProgressController.stateRelations);
				DrawTriggerRelations(m_ProgressController.triggerRelations);
				DrawTargets();
			}
			DrawCapture();

			if (GUI.changed) {
				EditorUtility.SetDirty(m_ProgressController);
			}
		}
		private void DrawTitle() {
			EditorGUILayout.BeginHorizontal();
			{
				// 标题
				GUILayout.Label("标题:", "BoldLabel", GUILayout.Width(30F));
				string newTitle = GUILayout.TextField(m_ProgressController.title);
				if (newTitle != m_ProgressController.title) {
					Undo.RecordObject(m_ProgressController, "Title");
					m_ProgressController.title = newTitle;
				}

				Color oldColor = GUI.backgroundColor;
				// 延迟初始化按钮
				GUI.backgroundColor = m_ProgressController.lazyInit ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
				GUIContent lazyInitContent = new GUIContent("延迟初始化", "第一次赋值时才初始化");
				if (GUILayout.Toggle(m_ProgressController.lazyInit, lazyInitContent, "button", BTN_WIDTH_OPTION) != m_ProgressController.lazyInit) {
					m_ProgressController.lazyInit = !m_ProgressController.lazyInit;
				}
				
				// 简洁模式按钮
				GUI.backgroundColor = m_BriefMode ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
				if (GUILayout.Toggle(m_BriefMode, "简略模式", "button", BTN_WIDTH_OPTION) != m_BriefMode) {
					m_BriefMode = !m_BriefMode;
				}
				GUI.backgroundColor = oldColor;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				// 初始索引
				GUILayout.Label("初始进度:", GUILayout.Width(54F));
				float newInitialProgress = EditorGUILayout.Slider(m_ProgressController.initialProgress, 0, 1);
				if (Mathf.Abs(newInitialProgress - m_ProgressController.initialProgress) > Mathf.Epsilon) {
					Undo.RecordObject(m_ProgressController, "InitialProgress");
					m_ProgressController.initialProgress = newInitialProgress;
				}

				// 屏蔽控制按钮
				Color oldColor = GUI.backgroundColor;
				GUI.backgroundColor = m_DisableCtrl ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
				if (GUILayout.Toggle(m_DisableCtrl, "屏蔽控制", "button", BTN_WIDTH_OPTION) != m_DisableCtrl) {
					m_DisableCtrl = !m_DisableCtrl;
				}
				GUI.backgroundColor = oldColor;
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawProgress() {
			GUILayout.Space(10F);
			float newProgress = EditorGUILayout.Slider(m_ProgressController.Progress, 0, 1);
			if (Mathf.Abs(newProgress - m_ProgressController.Progress) > Mathf.Epsilon) {
				OnProgressChange(newProgress);
			}
			GUILayout.Space(10F);
		}

		private void DrawRelations(IList<ProgressRelate> relations) {
			bool fold = EditorPrefs.GetBool("DrawRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联控制:" : "\u25BC 关联控制:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawRelations", newFold);
			}

			if (newFold) {
				return;
			}

			for (int i = 0, length = relations.Count; i < length; ++i) {
				ProgressRelate relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				GUILayout.Label("当", GUILayout.Width(16F));
				DrawMinMaxSlider(
						ref relation.minProgress, ref relation.maxProgress,
						() => Undo.RecordObject(m_ProgressController, "ProgressRelation.Progress")
				);

				bool prevEnabled = GUI.enabled;
				// 上移下移删除按钮
				GUI.enabled = prevEnabled && i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = prevEnabled;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				// 关联进度
				GUILayout.Label("改变进度", GUILayout.Width(52F));
				relation.controller = CustomDrawer.DrawComponentSelector(
						relation.controller,
						controller => controller.title == "" ? "匿名" : controller.title,
						() => Undo.RecordObject(m_ProgressController, "ProgressRelation.Controller")
				);
				if (relation.controller) {
					DrawMinMaxSlider(
							ref relation.targetMinProgress, ref relation.targetMaxProgress,
							() => Undo.RecordObject(m_ProgressController, "ProgressRelation.Target")
					);
				} else {
					GUI.enabled = false;
					EditorGUILayout.Slider(0, 0, 1);
					GUI.enabled = prevEnabled;
				}

				GUILayout.Label("", GUILayout.Width(76F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_ProgressController, "ProgressRelation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_ProgressController, "ProgressRelation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_ProgressController, "ProgressRelation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_ProgressController, "ProgressRelation.Add");
					relations.Add(new ProgressRelate());
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawStateRelations(IList<ProgressRelateState> relations) {
			bool fold = EditorPrefs.GetBool("DrawStateRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联状态控制:" : "\u25BC 关联状态控制:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawStateRelations", newFold);
			}

			if (newFold) {
				return;
			}

			for (int i = 0, length = relations.Count; i < length; ++i) {
				ProgressRelateState relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				GUILayout.Label("当", GUILayout.Width(16F));
				DrawMinMaxSlider(
						ref relation.minProgress, ref relation.maxProgress,
						() => Undo.RecordObject(m_ProgressController, "StateRelation.Progress")
				);

				bool prevEnabled = GUI.enabled;
				// 上移下移删除按钮
				GUI.enabled = prevEnabled && i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = prevEnabled;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				// 关联状态
				GUILayout.Label("切换状态", GUILayout.Width(52F));
				relation.controller = CustomDrawer.DrawComponentSelector(
						relation.controller,
						controller => controller.title == "" ? "匿名" : controller.title,
						() => Undo.RecordObject(m_ProgressController, "StateRelation.Controller")
				);
				if (relation.controller) {
					int _stateCount = relation.controller.states.Count;
					string[] _options = new string[_stateCount + 1];
					int[] _values = new int[_stateCount + 1];
					_options[0] = "无";
					_values[0] = ProgressRelateState.TARGET_NONE;
					for (int j = 0; j < _stateCount; ++j) {
						_options[j + 1] = j + "";
						_values[j + 1] = j;
					}
					int newTargetIndex = EditorGUILayout.IntPopup(relation.targetIndex, _options, _values);
					if (newTargetIndex != relation.targetIndex) {
						Undo.RecordObject(m_ProgressController, "StateRelation.Target");
						relation.targetIndex = newTargetIndex;
					}
				} else {
					GUI.enabled = false;
					EditorGUILayout.IntPopup(-1, new [] { "无" }, new [] { -1 });
					GUI.enabled = prevEnabled;
				}

				GUILayout.Label("", GUILayout.Width(76F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_ProgressController, "StateRelation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_ProgressController, "StateRelation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_ProgressController, "StateRelation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_ProgressController, "StateRelation.Add");
					relations.Add(new ProgressRelateState());
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawTriggerRelations(IList<ProgressRelateTrigger> relations) {
			bool fold = EditorPrefs.GetBool("DrawTriggerRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联触发器:" : "\u25BC 关联触发器:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawStateRelations", newFold);
			}

			if (newFold) {
				return;
			}

			for (int i = 0, length = relations.Count; i < length; ++i) {
				ProgressRelateTrigger relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				GUILayout.Label("当", GUILayout.Width(16F));
				DrawMinMaxSlider(
						ref relation.minProgress, ref relation.maxProgress,
						() => Undo.RecordObject(m_ProgressController, "TriggerRelation.Progress")
				);

				bool prevEnabled = GUI.enabled;
				// 上移下移删除按钮
				GUI.enabled = prevEnabled && i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = prevEnabled;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				// 触发
				GUILayout.Label("触发", GUILayout.Width(28F));
				relation.trigger = CustomDrawer.DrawComponentSelector(
						relation.trigger,
						trigger => trigger.GetType().Name + " - " + (trigger.title == "" ? "匿名" : trigger.title),
						() => Undo.RecordObject(m_ProgressController, "TriggerRelation.Trigger")
				);
				GUILayout.Label("", GUILayout.Width(76F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_ProgressController, "TriggerRelation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_ProgressController, "TriggerRelation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_ProgressController, "TriggerRelation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_ProgressController, "TriggerRelation.Add");
					relations.Add(new ProgressRelateTrigger());
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawTargets() {
			bool fold = EditorPrefs.GetBool("DrawTargets", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 控制对象:" : "\u25BC 控制对象:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawTargets", newFold);
			}

			if (newFold) {
				return;
			}

			bool prevEnabled = GUI.enabled;
			GUI.enabled = false;
			List<BaseProgressCtrl> _targets = m_ProgressController.Targets;
			for (int i = 0, length = _targets.Count; i < length; ++i) {
				BaseProgressCtrl _target = _targets[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				EditorGUILayout.ObjectField(_target, typeof(BaseProgressCtrl), true);

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
			GUI.enabled = prevEnabled;
		}

		private void DrawCapture() {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("记录状态", BTN_DOUBLE_HEIGHT_OPTION)) {
				m_ProgressController.Capture();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawMinMaxSlider(ref float minValue, ref float maxValue, Action beforeChange = null) {
			float newMinValue = minValue;
			float newMaxValue = maxValue;
			
			newMinValue = EditorGUILayout.FloatField(minValue, GUILayout.Width(60F));
			if (Mathf.Abs(newMinValue - minValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				minValue = newMinValue;
			}
			EditorGUILayout.MinMaxSlider(ref newMinValue, ref newMaxValue, 0, 1);
			if (Mathf.Abs(newMinValue - minValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				minValue = newMinValue;
			}
			if (Mathf.Abs(newMaxValue - maxValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				maxValue = newMaxValue;
			}
			newMaxValue = EditorGUILayout.FloatField(maxValue, GUILayout.Width(60F));
			if (Mathf.Abs(newMaxValue - maxValue) >= Mathf.Epsilon) {
				beforeChange?.Invoke();
				maxValue = newMaxValue;
			}
		}

		private void OnProgressChange(float progress) {
			Undo.RecordObject(m_ProgressController, "Progress.Change");
			if (m_DisableCtrl) {
				FieldInfo field = m_ProgressController.GetType().GetField("m_Progress", BindingFlags.Instance | BindingFlags.NonPublic);
				field?.SetValue(m_ProgressController, progress);
			} else {
				m_ProgressController.Progress = progress;
			}
		}
	}
}