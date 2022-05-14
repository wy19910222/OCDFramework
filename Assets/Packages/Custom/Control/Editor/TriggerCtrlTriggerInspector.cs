// /*
//  * @Author: wangyun
//  * @CreateTime: 2022-04-20 02:37:58 039
//  * @LastEditor: wangyun
//  * @EditTime: 2022-04-20 02:37:58 053
//  */
//
// using UnityEngine;
// using UnityEditor;
//
// using UObject = UnityEngine.Object;
//
// namespace Control {
// 	[CanEditMultipleObjects]
// 	[CustomEditor(typeof(TriggerCtrlTrigger), true)]
// 	public class TriggerCtrlTriggerInspector : Editor {
// 		public override void OnInspectorGUI() {
// 			base.OnInspectorGUI();
// 			if (GUILayout.Button("触发")) {
// 				(target as TriggerCtrlTrigger)?.Trigger();
// 			}
// 		}
// 	}
// }