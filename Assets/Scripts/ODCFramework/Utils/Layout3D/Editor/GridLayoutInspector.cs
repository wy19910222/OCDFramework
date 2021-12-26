/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

namespace Layout3D {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(GridLayout), true)]
	public class GridLayout1Inspector : Editor {

		protected GridLayout m_gridLayout;

		public void OnEnable() {
			m_gridLayout = target as GridLayout;
		}

		public override void OnInspectorGUI() {
			DrawDirection(0);
			DrawSortAndAlign(0);
			DrawDeltaAndLimit(0);
			if (m_gridLayout.direction0Limit > 0) {
				DrawDirection(1);
				DrawSortAndAlign(1);
				DrawDeltaAndLimit(1);
				if (m_gridLayout.direction1Limit > 0) {
					DrawDirection(2);
					DrawSortAndAlign(2);
					DrawDeltaAndLimit(2);
				}
			}
			GUILayout.Space(10F);

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Range:", "BoldLabel", GUILayout.Width(42F));
				GUILayout.Label(new GUIContent("Start:", "Include"), GUILayout.Width(34F));
				int newStart = Mathf.Max(EditorGUILayout.IntField(m_gridLayout.range.x), 0);
				if (newStart != m_gridLayout.range.x) {
					Undo.RecordObject(m_gridLayout, "Range");
					m_gridLayout.range.x = newStart;
				}
				GUILayout.Label(new GUIContent("Length:", "Exclude"), GUILayout.Width(46F));
				int oldEnd = m_gridLayout.range.y;
				if (oldEnd < 0) {
					float newEnd = EditorGUILayout.FloatField(Mathf.Infinity);
					if (newEnd < 0) {
						Undo.RecordObject(m_gridLayout, "Range");
						m_gridLayout.range.y = -1;
					} else if (newEnd != Mathf.Infinity) {
						Undo.RecordObject(m_gridLayout, "Range");
						m_gridLayout.range.y = (int)newEnd;
					}
				} else {
					int newEnd = EditorGUILayout.IntField(oldEnd);
					if (newEnd < 0) {
						Undo.RecordObject(m_gridLayout, "Range");
						m_gridLayout.range.y = -1;
					} else if (newEnd != oldEnd) {
						Undo.RecordObject(m_gridLayout, "Range");
						m_gridLayout.range.y = newEnd;
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Offset:", "BoldLabel", GUILayout.Width(42F));
				Vector3 newOffset = EditorGUILayout.Vector3Field("", m_gridLayout.offset);
				if (newOffset != m_gridLayout.offset) {
					Undo.RecordObject(m_gridLayout, "Offset");
					m_gridLayout.offset = newOffset;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Ignore Inactive:", "BoldLabel", GUILayout.Width(90F));
				bool newIgnoreInactive = EditorGUILayout.Toggle(m_gridLayout.ignoreInactive);
				if (newIgnoreInactive != m_gridLayout.ignoreInactive) {
					Undo.RecordObject(m_gridLayout, "IgnoreInactive");
					m_gridLayout.ignoreInactive = newIgnoreInactive;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("skips"), true);

			if (GUI.changed) {
				EditorUtility.SetDirty(m_gridLayout);
			}
		}

		private void DrawDirection(int i) {
			EditorGUILayout.BeginHorizontal();
			{
				if (i == 0) {
					GUILayout.Label("Direction0:", "BoldLabel", GUILayout.Width(66F));
					Direction oldDirection0 = m_gridLayout.direction0;
					Direction newDirection0 = (Direction)EditorGUILayout.EnumPopup(oldDirection0);
					if (newDirection0 != oldDirection0) {
						Undo.RecordObject(m_gridLayout, "Direction");
						m_gridLayout.direction0 = newDirection0;
						if (newDirection0 == m_gridLayout.direction1) {
							m_gridLayout.direction1 = oldDirection0;
						} else if (newDirection0 == m_gridLayout.direction2) {
							m_gridLayout.direction2 = oldDirection0;
						}
					}
				} else if (i == 1) {
					GUILayout.Label("Direction1:", "BoldLabel", GUILayout.Width(66F));
					List<Direction> list = new List<Direction>(new Direction[] { Direction.X, Direction.Y, Direction.Z });
					list.Remove(m_gridLayout.direction0);
					string[] options = list.ConvertAll(dir => dir + "").ToArray();
					int[] values = list.ConvertAll(dir => (int)dir).ToArray();
					Direction oldDirection1 = m_gridLayout.direction1;
					Direction newDirection1 = (Direction)EditorGUILayout.IntPopup((int)oldDirection1, options, values);
					if (newDirection1 != oldDirection1) {
						Undo.RecordObject(m_gridLayout, "Direction");
						m_gridLayout.direction1 = newDirection1;
						m_gridLayout.direction2 = oldDirection1;
					}
				} else if (i == 2) {
					GUILayout.Label("Direction2:", "BoldLabel", GUILayout.Width(66F));
					string[] options = new string[] { m_gridLayout.direction2 + "" };
					EditorGUILayout.Popup(0, options);
				}

				// 上移下移删除按钮
				Type type = m_gridLayout.GetType();
				FieldInfo limitField = type.GetField("direction" + i + "Limit", BindingFlags.Instance | BindingFlags.Public);
				uint limit = limitField == null ? 0 : (uint)limitField.GetValue(m_gridLayout);
				bool hasPrev = i > 0;
				bool hasNext = limit > 0;
				GUI.enabled = hasPrev;
				bool up = GUILayout.Button("\u25B2", GUILayout.Width(23F));
				GUI.enabled = hasNext;
				bool down = GUILayout.Button("\u25BC", GUILayout.Width(23F));
				GUI.enabled = hasPrev || hasNext;
				bool remove = GUILayout.Button("X", GUILayout.Width(24F));
				GUI.enabled = true;

				if (up) {
					// 上移			  
					SwapValue(i - 1, i);
				} else if (down) {
					// 下移
					SwapValue(i, i + 1);
				} else if (remove) {
					// 删除
					int index = i;
					bool swapSucceed = true;
					while (swapSucceed) {
						swapSucceed = SwapValue(index, ++index);
					}
					FieldInfo prevLimitField = type.GetField("direction" + (index - 2) + "Limit", BindingFlags.Instance | BindingFlags.Public);
					Undo.RecordObject(m_gridLayout, "Remove");
					prevLimitField.SetValue(m_gridLayout, (uint)0);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawSortAndAlign(int i) {
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(10F);

				Type type = m_gridLayout.GetType();

				GUILayout.Label("Sort" + i + ":", GUILayout.Width(38F));
				FieldInfo sortField = type.GetField("direction" + i + "Sort", BindingFlags.Instance | BindingFlags.Public);
				SortType oldSort = (SortType)sortField.GetValue(m_gridLayout);
				SortType newSort = (SortType)EditorGUILayout.EnumPopup(oldSort);
				if (newSort != oldSort) {
					Undo.RecordObject(m_gridLayout, "Sort");
					sortField.SetValue(m_gridLayout, newSort);
				}

				GUILayout.Label("Align" + i + ":", GUILayout.Width(42F));
				FieldInfo alignField = type.GetField("direction" + i + "Align", BindingFlags.Instance | BindingFlags.Public);
				AlignType oldAlign = (AlignType)alignField.GetValue(m_gridLayout);
				AlignType newAlign = (AlignType)EditorGUILayout.EnumPopup(oldAlign);
				if (newAlign != oldAlign) {
					Undo.RecordObject(m_gridLayout, "Align");
					alignField.SetValue(m_gridLayout, newAlign);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawDeltaAndLimit(int i) {
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(10F);

				Type type = m_gridLayout.GetType();

				GUILayout.Label("Delta" + i + ":", GUILayout.Width(44F));
				FieldInfo deltaField = type.GetField("direction" + i + "Delta", BindingFlags.Instance | BindingFlags.Public);
				float oldDelta = (float)deltaField.GetValue(m_gridLayout);
				float newDelta = EditorGUILayout.FloatField(oldDelta);
				if (newDelta != oldDelta) {
					Undo.RecordObject(m_gridLayout, "Delta");
					deltaField.SetValue(m_gridLayout, newDelta);
				}

				GUILayout.Label("CountLimit" + i + ":", GUILayout.Width(76F));
				FieldInfo limitField = type.GetField("direction" + i + "Limit", BindingFlags.Instance | BindingFlags.Public);
				if (limitField != null) {
					object limit = limitField.GetValue(m_gridLayout);
					uint oldLimit = (uint)limit;
					uint newLimit = (uint)Mathf.Max(EditorGUILayout.IntField((int)oldLimit), 0);
					if (newLimit != oldLimit) {
						Undo.RecordObject(m_gridLayout, "Limit");
						limitField.SetValue(m_gridLayout, newLimit);
					}
				} else {
					GUI.enabled = false;
					EditorGUILayout.IntField(0);
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private bool SwapValue(int prevI, int nextI) {
			Type type = m_gridLayout.GetType();

			FieldInfo prevDirectionField = type.GetField("direction" + prevI, BindingFlags.Instance | BindingFlags.Public);
			FieldInfo prevSortField = type.GetField("direction" + prevI + "Sort", BindingFlags.Instance | BindingFlags.Public);
			FieldInfo prevAlignField = type.GetField("direction" + prevI + "Align", BindingFlags.Instance | BindingFlags.Public);
			FieldInfo prevDeltaField = type.GetField("direction" + prevI + "Delta", BindingFlags.Instance | BindingFlags.Public);
			FieldInfo prevLimitField = type.GetField("direction" + prevI + "Limit", BindingFlags.Instance | BindingFlags.Public);
			if (prevLimitField == null) {
				// 没有next，交换失败
				return false;
			}

			FieldInfo nextDirectionField = type.GetField("direction" + nextI, BindingFlags.Instance | BindingFlags.Public);
			FieldInfo nextSortField = type.GetField("direction" + nextI + "Sort", BindingFlags.Instance | BindingFlags.Public);
			FieldInfo nextAlignField = type.GetField("direction" + nextI + "Align", BindingFlags.Instance | BindingFlags.Public);
			FieldInfo nextDeltaField = type.GetField("direction" + nextI + "Delta", BindingFlags.Instance | BindingFlags.Public);
			FieldInfo nextLimitField = type.GetField("direction" + nextI + "Limit", BindingFlags.Instance | BindingFlags.Public);

			Direction prevDirection = (Direction)prevDirectionField.GetValue(m_gridLayout);
			SortType prevSort = (SortType)prevSortField.GetValue(m_gridLayout);
			AlignType prevAlign = (AlignType)prevAlignField.GetValue(m_gridLayout);
			float prevDelta = (float)prevDeltaField.GetValue(m_gridLayout);
			uint prevLimit = (uint)prevLimitField.GetValue(m_gridLayout);

			Direction nextDirection = (Direction)nextDirectionField.GetValue(m_gridLayout);
			SortType nextSort = (SortType)nextSortField.GetValue(m_gridLayout);
			AlignType nextAlign = (AlignType)nextAlignField.GetValue(m_gridLayout);
			float nextDelta = (float)nextDeltaField.GetValue(m_gridLayout);
			uint nextLimit = nextLimitField == null ? 0 : (uint)nextLimitField.GetValue(m_gridLayout);

			Undo.RecordObject(m_gridLayout, "UpDown");
			prevDirectionField.SetValue(m_gridLayout, nextDirection);
			nextDirectionField.SetValue(m_gridLayout, prevDirection);
			prevSortField.SetValue(m_gridLayout, nextSort);
			nextSortField.SetValue(m_gridLayout, prevSort);
			prevAlignField.SetValue(m_gridLayout, nextAlign);
			nextAlignField.SetValue(m_gridLayout, prevAlign);
			prevDeltaField.SetValue(m_gridLayout, nextDelta);
			nextDeltaField.SetValue(m_gridLayout, prevDelta);
			if (nextLimit > 0) {
				prevLimitField.SetValue(m_gridLayout, nextLimit);
				nextLimitField.SetValue(m_gridLayout, prevLimit);
			}
			// 交换成功
			return true;
		}
	}
}