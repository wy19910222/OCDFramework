/*
 * @Author: wangyun
 * @CreateTime: 2022-05-04 08:12:59 425
 * @LastEditor: wangyun
 * @EditTime: 2022-05-04 08:12:59 431
 */

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace TransformSearch {
	public abstract class BaseSearch : EditorWindow {
		[MenuItem("Window/BaseSearch")]
		private static void Init() {
			BaseSearch window = GetWindow<BaseSearch>();
			window.minSize = new Vector2(200F, 200F);
			window.Show();
		}

		private static readonly Color BTN_NORMAL_COLOR_1 = new Color(1, 1, 1, 0);
		private static readonly Color BTN_NORMAL_COLOR_2 = new Color(1, 1, 1, 0.2F);
		private static readonly Color BTN_CHECKED_COLOR = new Color(0, 1, 1, 0.5F);
		private static readonly Color BTN_NORMAL_ARROW_COLOR = new Color(1, 1, 1, 0.5F);
		private static readonly Color BTN_CHECKED_ARROW_COLOR = Color.white;
		private static readonly Color LABEL_NORMAL_COLOR = Color.white;
		private static readonly Color LABEL_MATCHED_COLOR = Color.green * 0.8F;
		private static readonly GUILayoutOption ARROW_WIDTH = GUILayout.Width(20F);
		private static readonly GUILayoutOption LINE_HEIGHT = GUILayout.Height(EditorGUIUtility.singleLineHeight);
		
		protected readonly Dictionary<Transform, bool> m_TransIsFoldedDict = new Dictionary<Transform, bool>();
		protected readonly HashSet<Transform> m_TransIsMatchSet = new HashSet<Transform>();
		protected readonly List<Transform> m_TransList = new List<Transform>();

		private Vector2 m_ScrollPos = Vector2.zero;
		private int m_LineNumber;

		protected abstract bool Match(Transform trans);

		protected void Search() {
			m_TransIsFoldedDict.Clear();
			m_TransIsMatchSet.Clear();
			m_TransList.Clear();
			
			GetSelections();
			foreach (var trans in m_TransList) {
				CheckTrans(trans);
			}
		}

		private void GetSelections() {
			foreach (var obj in Selection.objects) {
				switch (obj) {
					case GameObject go:
						m_TransList.Add(go.transform);
						break;
					case DefaultAsset defaultAsset: {
						string path = AssetDatabase.GetAssetPath(defaultAsset);
						if (Directory.Exists(path)) {
							string[] filePaths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
							foreach (var filePath in filePaths) {
								Transform trans = AssetDatabase.LoadAssetAtPath<Transform>(filePath);
								m_TransList.Add(trans);
							}
						}
						break;
					}
				}
			}
			// 任意两者有父子关系，则只取父节点，同时也可以去重
			for (int index1 = m_TransList.Count - 1; index1 >= 1; --index1) {
				for (int index2 = index1 - 1; index2 >= 0; --index2) {
					Transform trans1 = m_TransList[index1];
					Transform trans2 = m_TransList[index2];
					if (trans1.IsChildOf(trans2)) {
						m_TransList.RemoveAt(index1);
						break;
					}
					if (trans2.IsChildOf(trans1)) {
						m_TransList.RemoveAt(index2);
						--index1;
					}
				}
			}
		}

		private bool CheckTrans(Transform trans) {
			bool visible = false;
			foreach (Transform child in trans) {
				visible = CheckTrans(child) || visible;
			}
			if (Match(trans)) {
				m_TransIsMatchSet.Add(trans);
				m_TransIsFoldedDict[trans] = false;
				return true;
			}
			if (visible) {
				m_TransIsFoldedDict[trans] = false;
				return true;
			}
			return false;
		}

		protected virtual void OnGUI() {
			DrawHeader();
			DrawList();
		}

		protected virtual void DrawHeader() {
			if (GUILayout.Button("搜索")) {
				Search();
			}
		}

		protected virtual void DrawList() {
			m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);
			m_LineNumber = 0;
			bool anyDisplayed = false;
			foreach (var trans in m_TransList) {
				if (m_TransIsFoldedDict.ContainsKey(trans)) {
					anyDisplayed = true;
					DisplayTransform(trans);
				}
			}
			if (anyDisplayed) {
				DrawBottom();
			}
			GUILayout.EndScrollView();
		}
		protected virtual void DrawBottom() {
			if (GUILayout.Button("全选")) {
				List<Transform> matchedTransforms = new List<Transform>(m_TransIsMatchSet);
				Selection.objects = matchedTransforms.ConvertAll(trans => (Object)trans.gameObject).ToArray();
			}
		}

		private void DisplayTransform(Transform trans, int indent = 0) {
			m_LineNumber++;
			
			List<Transform> children = new List<Transform>();
			foreach (Transform child in trans) {
				if (m_TransIsFoldedDict.ContainsKey(child)) {
					children.Add(child);
				}
			}
			bool isFolded = m_TransIsFoldedDict[trans];
			bool isSelected = false;
			foreach (var obj in Selection.objects) {
				if (obj is GameObject go && go.transform == trans) {
					isSelected = true;
					break;
				}
			}
			bool isMatched = m_TransIsMatchSet.Contains(trans);
			
			GUILayout.Space(-3);
			if (children.Count > 0) {
				GUILayout.BeginHorizontal();
				{
					Color prevContentColor = GUI.contentColor;
					GUI.contentColor = isSelected ? BTN_CHECKED_ARROW_COLOR : BTN_NORMAL_ARROW_COLOR;
					GUILayout.Space(16F * indent + 2F);
					string content = isFolded ? "\u25BA" : "\u25BC";
					if (GUILayout.Toggle(isFolded, content, "Label", ARROW_WIDTH, LINE_HEIGHT) != isFolded) {
						m_TransIsFoldedDict[trans] = !isFolded;
					}
					GUI.contentColor = prevContentColor;
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(-EditorGUIUtility.singleLineHeight - 2);
			}
			
			GUILayout.Space(1);
			GUILayout.BeginHorizontal();
			{
				Color prevBackgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = isSelected ? BTN_CHECKED_COLOR : (m_LineNumber & 1) == 1 ? BTN_NORMAL_COLOR_1 : BTN_NORMAL_COLOR_2;
				if (GUILayout.Toggle(isSelected, "", "ButtonMid", LINE_HEIGHT) != isSelected) {
					bool holdCtrl = (Event.current.modifiers & EventModifiers.Control) != 0;
					bool holdCommand = (Event.current.modifiers & EventModifiers.Command) != 0;
					if (holdCtrl || holdCommand) {
						List<Object> list = new List<Object>(Selection.objects);
						if (isSelected) {
							list.Remove(trans.gameObject);
						} else {
							list.Add(trans.gameObject);
						}
						Selection.objects = list.ToArray();
					} else {
						Selection.objects = new Object[] {trans.gameObject};
					}
				}
				GUI.backgroundColor = prevBackgroundColor;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(-EditorGUIUtility.singleLineHeight - 2);
			
			GUILayout.Space(-1);
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(16F * indent + 18F);
				Color prevContentColor = GUI.contentColor;
				GUI.contentColor = isMatched ? LABEL_MATCHED_COLOR : LABEL_NORMAL_COLOR;
				GUILayout.Label(trans.name, isMatched ? "BoldLabel" : "Label", LINE_HEIGHT);
				GUI.contentColor = prevContentColor;
			}
			GUILayout.EndHorizontal();

			if (!isFolded) {
				foreach (Transform child in children) {
					DisplayTransform(child, indent + 1);
				}
			}
		}
	}
}
