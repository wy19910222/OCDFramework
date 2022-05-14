/*
 * @Author: wangyun
 * @CreateTime: 2022-05-02 01:13:30 495
 * @LastEditor: wangyun
 * @EditTime: 2022-05-04 01:51:33 841
 */

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace TransformSearch {
	public class ComponentSearch : BaseSearch {
		[MenuItem("Window/Search/ComponentSearch")]
		private static void Init() {
			ComponentSearch window = GetWindow<ComponentSearch>("ComponentSearch");
			window.minSize = new Vector2(200F, 200F);
			window.Show();
		}

		[SerializeField]
		private string m_ComponentName;
		private Type m_ComponentType;
	
		public void OnEnable() {
			m_ComponentName = EditorPrefs.GetString("ComponentSearch.ComponentName");
		}

		protected override bool Match(Transform trans) {
			Component[] comps = trans.GetComponents<Component>();
			foreach (var comp in comps) {
				Type type = comp.GetType();
				if (m_ComponentType != null) {
					if (m_ComponentType.IsAssignableFrom(type)) {
						return true;
					}
				} else if (type.Name == m_ComponentName) {
					return true;
				}
			}
			return false;
		}

		protected override void DrawHeader() {
			GUILayout.BeginHorizontal();
			DrawLuaPath();
			if (GUILayout.Button("搜索", GUILayout.Width(60F))) {
				m_ComponentType = NameToType(m_ComponentName);
				Search();
			}
			GUILayout.EndHorizontal();
		}

		protected void DrawLuaPath() {
			GUILayout.BeginHorizontal();
			string newComponentName = EditorGUILayout.TextField(m_ComponentName);
			if (newComponentName != m_ComponentName) {
				Undo.RecordObject(this, "ComponentName");
				m_ComponentName = newComponentName;
				EditorPrefs.SetString("ComponentSearch.ComponentName", m_ComponentName);
			}
			GUILayout.EndHorizontal();
		}

		protected static Type NameToType(string typeName) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies) {
				Type type = assembly.GetType(typeName);
				if (type != null) {
					return type;
				}
			}
			return null;
		}
	}
}
