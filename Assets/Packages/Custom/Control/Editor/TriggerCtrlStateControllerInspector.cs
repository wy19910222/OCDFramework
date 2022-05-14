/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 20:41:25 998
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 20:41:25 998
 */

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace Control {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TriggerCtrlStateController), true)]
	public class TriggerCtrlStateControllerInspector : Editor {
		private static readonly Color BTN_NORMAL_COLOR = Color.white;
		private static readonly Color BTN_CHECKED_COLOR = Color.cyan * 0.8F;
		
		protected TriggerCtrlStateController m_Trigger;

		private void OnEnable() {
			m_Trigger = target as TriggerCtrlStateController;
		}
		
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			StateController controller = m_Trigger.controller;
			if (controller) {
				DrawStates(controller);
			}
			if (GUI.changed) {
				EditorUtility.SetDirty(target);
			}
		}

		private void DrawStates(StateController controller) {
			GUILayout.Space(10F);
			
			string[] options = GetStatePopupOption(controller);
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
			int stateCount = options.Length;
			float itemWidth = Mathf.Max(optionLengthMax * 5F, 50F);
			float width = EditorGUIUtility.currentViewWidth - 20F;
			int columnMax = Mathf.Max(1, (int) (width / itemWidth));
			int row = Mathf.CeilToInt((float) stateCount / columnMax);
			int col = Mathf.Min(columnMax, stateCount);
			for (int r = 0; r < row; ++r) {
				EditorGUILayout.BeginHorizontal();
				for (int c = 0; c < col; ++c) {
					int i = r * col + c;
					if (i < stateCount) {
						bool selected = i == m_Trigger.index;
						// 序号按钮
						Color oldColor = GUI.backgroundColor;
						GUI.backgroundColor = selected ? BTN_CHECKED_COLOR : BTN_NORMAL_COLOR;
						if (GUILayout.Toggle(selected, options[i], "button") && !selected) {
							Undo.RecordObject(target, "Index");
							m_Trigger.index = i;
						}
						GUI.backgroundColor = oldColor;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			
			GUILayout.Space(10F);
		}
		
		private string[] GetStatePopupOption(StateController controller) {
			List<State> states = controller.states;
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