/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace StateControl {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(StateController), true)]
	public class StateControllerInspector : Editor {

		protected StateController m_stateController;
		private bool m_disableCtrl;
		private bool m_briefMode = true;

		public void OnEnable() {
			m_stateController = target as StateController;
		}

		public override void OnInspectorGUI() {
			DrawTitle();
			if (m_briefMode) {
				DrawStatesBrief(m_stateController.states, m_stateController.Index);
			} else {
				DrawStates(m_stateController.states, m_stateController.Index);
				DrawRelations(m_stateController.relations, m_stateController.states.Count);
				DrawTargets();
			}

			if (GUILayout.Button("记录状态", "minibutton")) {
				m_stateController.Capture();
			}

			if (GUI.changed) {
				EditorUtility.SetDirty(m_stateController);
			}
		}
		private void DrawTitle() {
			EditorGUILayout.BeginHorizontal();
			{
				// 标题
				GUILayout.Label("标题:", "BoldLabel", GUILayout.Width(30F));
				string newTitle = GUILayout.TextField(m_stateController.title);
				if (newTitle != m_stateController.title) {
					Undo.RecordObject(m_stateController, "Title");
					m_stateController.title = newTitle;
				}

				// 简洁模式按钮
				Color oldColor = GUI.backgroundColor;
				if (m_briefMode) {
					GUI.backgroundColor = new Color(0.8F, 0.8F, 0.8F);
				} else {
					GUI.backgroundColor = new Color(1, 1, 1);
				}
				if (GUILayout.Toggle(m_briefMode, "简略模式", "minibutton", GUILayout.Width(80F)) != m_briefMode) {
					m_briefMode = !m_briefMode;
				}
				GUI.backgroundColor = oldColor;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				// 初始索引
				GUILayout.Label("初始状态:", GUILayout.Width(54F));
				string[] options = GetStatePopupOption();
				int newInitialIndex = EditorGUILayout.Popup(m_stateController.initialIndex, options);
				if (newInitialIndex != m_stateController.initialIndex) {
					Undo.RecordObject(m_stateController, "InitialIndex");
					m_stateController.initialIndex = newInitialIndex;
				}

				// 屏蔽控制按钮
				Color oldColor = GUI.backgroundColor;
				if (m_disableCtrl) {
					GUI.backgroundColor = new Color(0.8F, 0.8F, 0.8F);
				} else {
					GUI.backgroundColor = new Color(1, 1, 1);
				}
				if (GUILayout.Toggle(m_disableCtrl, "屏蔽控制", "minibutton", GUILayout.Width(80F)) != m_disableCtrl) {
					m_disableCtrl = !m_disableCtrl;
				}
				GUI.backgroundColor = oldColor;
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawStatesBrief(List<State> states, int index) {
			GUILayout.Space(10F);
			string[] options = GetStatePopupOption();
			int optionLengthMax = 0;
			foreach (var option in options) {
				int optionLength = option.Length;
				if (optionLength > optionLengthMax) {
					optionLengthMax = optionLength;
				}
			}
			float itemWidth = Mathf.Max(optionLengthMax * 10, 50);
			float width = EditorGUIUtility.currentViewWidth - 20;
			int columnMax = (int) (width / itemWidth) - 1;	// 不知道为什么还是会超出去，所以-1
			int stateCount = states.Count;
			int row = Mathf.CeilToInt((float) stateCount / columnMax);
			int col = Mathf.Min(columnMax, stateCount);
			for (int r = 0; r < row; ++r) {
				EditorGUILayout.BeginHorizontal();
				for (int c = 0; c < col; ++c) {
					int i = r * col + c;
					if (i < stateCount) {
						bool selected = i == index;
						// 序号按钮
						Color oldColor = GUI.backgroundColor;
						if (selected) {
							GUI.backgroundColor = new Color(0.8F, 0.8F, 0.8F);
						} else {
							GUI.backgroundColor = new Color(1, 1, 1);
						}
						if (GUILayout.Toggle(selected, options[i], "minibutton") && !selected) {
							Undo.RecordObject(m_stateController, "State.Select");
							if (m_disableCtrl) {
								FieldInfo field = m_stateController.GetType().GetField("m_index", BindingFlags.Instance | BindingFlags.NonPublic);
								field.SetValue(m_stateController, i);
							} else {
								m_stateController.Index = i;
							}
						}
						GUI.backgroundColor = oldColor;
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			GUILayout.Space(10F);
		}

		private void DrawStates(List<State> states, int index) {
			bool fold = EditorPrefs.GetBool("DrawStates", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 状态列表:" : "\u25BC 状态列表:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawStates", newFold);
			}

			if (newFold) {
				return;
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10F);
			GUILayout.Label("序号:", GUILayout.Width(30F));
			GUILayout.Label("名称:", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4F - 40F));
			GUILayout.Label("描述:");
			GUILayout.Space(10F);
			EditorGUILayout.EndHorizontal();

			for (int i = 0, length = states.Count; i < length; ++i) {
				bool selected = i == index;
				State state = states[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				// 序号按钮
				Color oldColor = GUI.backgroundColor;
				if (selected) {
					GUI.backgroundColor = new Color(0.8F, 0.8F, 0.8F);
				} else {
					GUI.backgroundColor = new Color(1, 1, 1);
				}
				if (GUILayout.Toggle(selected, i + "", "minibutton", GUILayout.Width(30F)) && !selected) {
					Undo.RecordObject(m_stateController, "State.Select");
					if (m_disableCtrl) {
						FieldInfo field = m_stateController.GetType().GetField("m_index", BindingFlags.Instance | BindingFlags.NonPublic);
						field.SetValue(m_stateController, i);
					} else {
						m_stateController.Index = i;
					}
				}
				GUI.backgroundColor = oldColor;

				// 名称
				string newName = GUILayout.TextField(state.name, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4F - 40F));
				if (newName != state.name) {
					Undo.RecordObject(m_stateController, "State.Name");
					state.name = newName;
				}

				// 描述
				string newDesc = GUILayout.TextField(state.desc);
				if (newDesc != state.desc) {
					Undo.RecordObject(m_stateController, "State.Desc");
					state.desc = newDesc;
				}

				// 上移下移删除按钮
				GUI.enabled = i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = length > 1;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));
				GUI.enabled = true;

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_stateController, "State.Remove");
					m_stateController.RemoveState(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_stateController, "State.Up");
					states[i] = states[i - 1];
					states[i - 1] = state;
				} else if (down) {
					Undo.RecordObject(m_stateController, "State.Down");
					states[i] = states[i + 1];
					states[i + 1] = state;
				}

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
			{
				// 新增按钮
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "minibutton", GUILayout.Width(30F))) {
					Undo.RecordObject(m_stateController, "State.Add");
					m_stateController.AddState();
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawRelations(List<StateRelate> relations, int stateCount) {
			bool fold = EditorPrefs.GetBool("DrawRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联控制:" : "\u25BC 关联控制:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawRelations", newFold);
			}

			if (newFold) {
				return;
			}

			string[] options = GetStatePopupOption();
			for (int i = 0, length = relations.Count; i < length; ++i) {
				StateRelate relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				GUILayout.Label("从", GUILayout.Width(16F));
				int newFromMask = EditorGUILayout.MaskField(relation.fromMask, options);
				if (newFromMask != relation.fromMask) {
					relation.fromMask = newFromMask;
				}

				GUILayout.Label("到", GUILayout.Width(16F));
				int newToMask = EditorGUILayout.MaskField(relation.toMask, options);
				if (newToMask != relation.toMask) {
					relation.toMask = newToMask;
				}

				// 上移下移删除按钮
				GUI.enabled = i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = true;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				// 关联状态
				GUILayout.Label("切换状态", GUILayout.Width(60F));
				StateController newController = EditorGUILayout.ObjectField(relation.controller, typeof(StateController), true) as StateController;
				if (newController != relation.controller) {
					relation.controller = newController;
				}
				if (newController) {
					int _stateCount = newController.states.Count;
					string[] _options = new string[_stateCount + 3];
					int[] _values = new int[_stateCount + 3];
					_options[0] = "无";
					_values[0] = StateRelate.TARGET_NONE;
					_options[1] = "相同索引";
					_values[1] = StateRelate.TARGET_SAME_INDEX;
					_options[2] = "相同名称";
					_values[2] = StateRelate.TARGET_SAME_NAME;
					for (int j = 0; j < _stateCount; ++j) {
						_options[j + 3] = j + "";
						_values[j + 3] = j;
					}
					int newTargetIndex = EditorGUILayout.IntPopup(relation.targetIndex, _options, _values);
					if (newTargetIndex != relation.targetIndex) {
						relation.targetIndex = newTargetIndex;
					}
				} else {
					GUI.enabled = false;
					EditorGUILayout.IntPopup(-1, new string[] { "无" }, new int[] { -1 });
					GUI.enabled = true;
				}

				GUILayout.Label("", GUILayout.Width(76F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_stateController, "Target.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_stateController, "Target.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_stateController, "Target.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "minibutton", GUILayout.Width(30F))) {
					Undo.RecordObject(m_stateController, "Target.Add");
					relations.Add(new StateRelate());
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

			GUI.enabled = false;
			List<BaseStateCtrl> targets = m_stateController.Targets;
			for (int i = 0, length = targets.Count; i < length; ++i) {
				BaseStateCtrl target = targets[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				EditorGUILayout.ObjectField(target, typeof(BaseStateCtrl), true);

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
			GUI.enabled = true;
		}

		private string[] GetStatePopupOption() {
			List<State> states = m_stateController.states;
			bool anyNameExist = states.Exists(state => !string.IsNullOrEmpty(state.name));
			int stateCount = states.Count;
			string[] options = new string[stateCount];
			for (int i = 0; i < stateCount; ++i) {
				State state = states[i];
				string ext = anyNameExist ? state.name : state.desc;
				if (!string.IsNullOrEmpty(ext)) {
					ext = ":" + ext;
				}
				options[i] = i + ext;
			}
			return options;
		}
	}
}