/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace Control {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(StateController), true)]
	public class StateControllerInspector : Editor {
		private static readonly Color BTN_NORMAL_COLOR = Color.white;
		private static readonly Color BTN_CHECKED_COLOR = Color.cyan * 0.8F;
		private static readonly GUILayoutOption BTN_WIDTH_OPTION = GUILayout.Width(80F);
		private static readonly GUILayoutOption BTN_DOUBLE_HEIGHT_OPTION = GUILayout.Height(EditorGUIUtility.singleLineHeight * 2);

		protected StateController m_StateController;
		private bool m_BriefMode = true;
		private bool m_DisableCtrl;
		private bool m_AutoCapture;

		private void OnEnable() {
			m_StateController = target as StateController;
		}

		public override void OnInspectorGUI() {
			DrawTitle();
			if (m_BriefMode) {
				DrawStatesBrief(m_StateController.states, m_StateController.Index);
			} else {
				DrawStates(m_StateController.states, m_StateController.Index);
				DrawRelations(m_StateController.relations);
				DrawProgressRelations(m_StateController.progressRelations);
				DrawTriggerRelations(m_StateController.triggerRelations);
				DrawTargets();
			}
			DrawCapture();

			if (GUI.changed) {
				EditorUtility.SetDirty(m_StateController);
			}
		}
		private void DrawTitle() {
			EditorGUILayout.BeginHorizontal();
			{
				// 标题
				GUILayout.Label("标题:", "BoldLabel", GUILayout.Width(30F));
				string newTitle = GUILayout.TextField(m_StateController.title);
				if (newTitle != m_StateController.title) {
					Undo.RecordObject(m_StateController, "Title");
					m_StateController.title = newTitle;
				}

				// 简洁模式按钮
				Color oldColor = GUI.backgroundColor;
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
				GUILayout.Label("初始状态:", GUILayout.Width(54F));
				string[] options = GetStatePopupOption();
				int newInitialIndex = EditorGUILayout.Popup(m_StateController.initialIndex, options);
				if (newInitialIndex != m_StateController.initialIndex) {
					Undo.RecordObject(m_StateController, "InitialIndex");
					m_StateController.initialIndex = newInitialIndex;
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

		private void DrawStatesBrief(ICollection states, int index) {
			GUILayout.Space(10F);
			
			string[] options = GetStatePopupOption();
			ASCIIEncoding ascii = new ASCIIEncoding();
			float optionLengthMax = 0;
			foreach (var option in options) {
				float optionLength = 0;
				byte[] bytes = ascii.GetBytes(option);
				for (int i = 0, length = bytes.Length; i < length; ++i) {
					optionLength += bytes[i] == 63 ? 2.5F : 1F;
				}
				if (optionLength > optionLengthMax) {
					optionLengthMax = optionLength;
				}
			}
			float itemWidth = Mathf.Max(optionLengthMax * 5F, 50F);
			float width = EditorGUIUtility.currentViewWidth - 20F;
			int columnMax = Mathf.Max(1, (int) (width / itemWidth));
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
						GUI.backgroundColor = selected ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
						if (GUILayout.Toggle(selected, options[i], "button") && !selected) {
							OnStateBtnClick(i);
						}
						GUI.backgroundColor = oldColor;
					}
				}
				EditorGUILayout.EndHorizontal();
			}

			GUILayout.Space(10F);
		}

		private void DrawStates(IList<State> states, int index) {
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
				GUI.backgroundColor = selected ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
				if (GUILayout.Toggle(selected, i + "", "button", GUILayout.Width(30F)) && !selected) {
					OnStateBtnClick(i);
				}
				GUI.backgroundColor = oldColor;

				// 名称
				string newName = GUILayout.TextField(state.name, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4F - 40F));
				if (newName != state.name) {
					Undo.RecordObject(m_StateController, "State.Name");
					state.name = newName;
				}

				// 描述
				string newDesc = GUILayout.TextField(state.desc);
				if (newDesc != state.desc) {
					Undo.RecordObject(m_StateController, "State.Desc");
					state.desc = newDesc;
				}

				bool prevEnabled = GUI.enabled;
				// 上移下移删除按钮
				GUI.enabled = prevEnabled && i > 0;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && i < length - 1;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = prevEnabled && length > 1;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));
				GUI.enabled = prevEnabled;

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_StateController, "State.Remove");
					m_StateController.RemoveState(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_StateController, "State.Up");
					states[i] = states[i - 1];
					states[i - 1] = state;
				} else if (down) {
					Undo.RecordObject(m_StateController, "State.Down");
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
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_StateController, "State.Add");
					m_StateController.AddState();
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawRelations(IList<StateRelate> relations) {
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
					Undo.RecordObject(m_StateController, "StateRelation.From");
					relation.fromMask = newFromMask;
				}
				GUILayout.Label("到", GUILayout.Width(16F));
				int newTomMask = EditorGUILayout.MaskField(relation.toMask, options);
				if (newTomMask != relation.toMask) {
					Undo.RecordObject(m_StateController, "StateRelation.To");
					relation.toMask = newTomMask;
				}

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
						() => Undo.RecordObject(m_StateController, "StateRelation.Controller")
				);
				if (relation.controller) {
					int _stateCount = relation.controller.states.Count;
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
					Undo.RecordObject(m_StateController, "StateRelation.State");
					relation.targetIndex = EditorGUILayout.IntPopup(relation.targetIndex, _options, _values);
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
					Undo.RecordObject(m_StateController, "StateRelation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_StateController, "StateRelation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_StateController, "StateRelation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_StateController, "StateRelation.Add");
					relations.Add(new StateRelate());
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawProgressRelations(IList<StateRelateProgress> relations) {
			bool fold = EditorPrefs.GetBool("DrawProgressRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联进度控制:" : "\u25BC 关联进度控制:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawProgressRelations", newFold);
			}

			if (newFold) {
				return;
			}

			string[] options = GetStatePopupOption();
			for (int i = 0, length = relations.Count; i < length; ++i) {
				StateRelateProgress relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				GUILayout.Label("从", GUILayout.Width(16F));
				int newFromMask = EditorGUILayout.MaskField(relation.fromMask, options);
				if (newFromMask != relation.fromMask) {
					Undo.RecordObject(m_StateController, "ProgressRelation.From");
					relation.fromMask = newFromMask;
				}
				GUILayout.Label("到", GUILayout.Width(16F));
				int newTomMask = EditorGUILayout.MaskField(relation.toMask, options);
				if (newTomMask != relation.toMask) {
					Undo.RecordObject(m_StateController, "ProgressRelation.To");
					relation.toMask = newTomMask;
				}

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
						() => Undo.RecordObject(m_StateController, "ProgressRelation.Controller")
				);
				if (relation.controller) {
					float newProgress = EditorGUILayout.Slider(relation.targetProgress, 0, 1);
					if (Mathf.Abs(newProgress - relation.targetProgress) >= Mathf.Epsilon) {
						Undo.RecordObject(m_StateController, "ProgressRelation.Progress");
						relation.targetProgress = newProgress;
					}
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
					Undo.RecordObject(m_StateController, "ProgressRelation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_StateController, "ProgressRelation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_StateController, "ProgressRelation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_StateController, "ProgressRelation.Add");
					relations.Add(new StateRelateProgress());
				}
				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawTriggerRelations(IList<StateRelateTrigger> relations) {
			bool fold = EditorPrefs.GetBool("DrawTriggerRelations", false);
			bool newFold = GUILayout.Toggle(fold, fold ? "\u25BA 关联触发器:" : "\u25BC 关联触发器:", "BoldLabel");
			if (newFold != fold) {
				EditorPrefs.SetBool("DrawProgressRelations", newFold);
			}

			if (newFold) {
				return;
			}

			string[] options = GetStatePopupOption();
			for (int i = 0, length = relations.Count; i < length; ++i) {
				StateRelateTrigger relation = relations[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				GUILayout.Label("从", GUILayout.Width(16F));
				int newFromMask = EditorGUILayout.MaskField(relation.fromMask, options);
				if (newFromMask != relation.fromMask) {
					Undo.RecordObject(m_StateController, "TriggerRelation.From");
					relation.fromMask = newFromMask;
				}
				GUILayout.Label("到", GUILayout.Width(16F));
				int newTomMask = EditorGUILayout.MaskField(relation.toMask, options);
				if (newTomMask != relation.toMask) {
					Undo.RecordObject(m_StateController, "TriggerRelation.To");
					relation.toMask = newTomMask;
				}

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
						() => Undo.RecordObject(m_StateController, "TriggerRelation.Trigger")
				);
				GUILayout.Label("", GUILayout.Width(76F));

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();

				// 上移下移删除
				if (remove) {
					Undo.RecordObject(m_StateController, "TriggerRelation.Remove");
					relations.RemoveAt(i);
					--length;
					--i;
				} else if (up) {
					Undo.RecordObject(m_StateController, "TriggerRelation.Up");
					relations[i] = relations[i - 1];
					relations[i - 1] = relation;
				} else if (down) {
					Undo.RecordObject(m_StateController, "TriggerRelation.Down");
					relations[i] = relations[i + 1];
					relations[i + 1] = relation;
				}
			}

			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);
				if (GUILayout.Button("+", "button", GUILayout.Width(30F))) {
					Undo.RecordObject(m_StateController, "TriggerRelation.Add");
					relations.Add(new StateRelateTrigger());
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
			List<BaseStateCtrl> _targets = m_StateController.Targets;
			for (int i = 0, length = _targets.Count; i < length; ++i) {
				BaseStateCtrl _target = _targets[i];

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(10F);

				EditorGUILayout.ObjectField(_target, typeof(BaseStateCtrl), true);

				GUILayout.Space(10F);
				EditorGUILayout.EndHorizontal();
			}
			GUI.enabled = prevEnabled;
		}

		private void DrawCapture() {
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("记录状态", BTN_DOUBLE_HEIGHT_OPTION)) {
				m_StateController.Capture();
			}
			GUI.backgroundColor = m_AutoCapture ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
			if (GUILayout.Toggle(m_AutoCapture, "切换状态时\n自动记录", "button", BTN_WIDTH_OPTION, BTN_DOUBLE_HEIGHT_OPTION) != m_AutoCapture) {
				m_AutoCapture = !m_AutoCapture;
			}
			EditorGUILayout.EndHorizontal();
		}

		private string[] GetStatePopupOption() {
			List<State> states = m_StateController.states;
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

		private void OnStateBtnClick(int i) {
			Undo.RecordObject(m_StateController, "State.Select");
			if (m_AutoCapture) {
				m_StateController.Capture();
			}
			if (m_DisableCtrl) {
				FieldInfo field = m_StateController.GetType().GetField("m_Index", BindingFlags.Instance | BindingFlags.NonPublic);
				field?.SetValue(m_StateController, i);
			} else {
				m_StateController.Index = i;
			}
		}
	}
}