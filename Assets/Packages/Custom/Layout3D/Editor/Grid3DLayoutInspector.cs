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
	[CustomEditor(typeof(Grid3DLayout), true)]
	public class Grid3DLayoutInspector : Editor {

		protected Grid3DLayout grid3DLayout;

		public void OnEnable() {
			grid3DLayout = target as Grid3DLayout;
		}

		public override void OnInspectorGUI() {
			DrawDirection(0);
			DrawSortAndAlign(0);
			DrawDeltaAndLimit(0);
			if (grid3DLayout.direction0Limit > 0) {
				DrawDirection(1);
				DrawSortAndAlign(1);
				DrawDeltaAndLimit(1);
				if (grid3DLayout.direction1Limit > 0) {
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
				int newStart = Mathf.Max(EditorGUILayout.IntField(grid3DLayout.range.x), 0);
				if (newStart != grid3DLayout.range.x) {
					Undo.RecordObject(grid3DLayout, "Range");
					grid3DLayout.range.x = newStart;
				}
				GUILayout.Label(new GUIContent("Length:", "Exclude"), GUILayout.Width(46F));
				int oldEnd = grid3DLayout.range.y;
				if (oldEnd < 0) {
					float newEnd = EditorGUILayout.FloatField(Mathf.Infinity);
					if (newEnd < 0) {
						Undo.RecordObject(grid3DLayout, "Range");
						grid3DLayout.range.y = -1;
					} else if (!float.IsPositiveInfinity(newEnd)) {
						Undo.RecordObject(grid3DLayout, "Range");
						grid3DLayout.range.y = (int)newEnd;
					}
				} else {
					int newEnd = EditorGUILayout.IntField(oldEnd);
					if (newEnd < 0) {
						Undo.RecordObject(grid3DLayout, "Range");
						grid3DLayout.range.y = -1;
					} else if (newEnd != oldEnd) {
						Undo.RecordObject(grid3DLayout, "Range");
						grid3DLayout.range.y = newEnd;
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Offset:", "BoldLabel", GUILayout.Width(42F));
				Vector3 newOffset = EditorGUILayout.Vector3Field("", grid3DLayout.offset);
				if (newOffset != grid3DLayout.offset) {
					Undo.RecordObject(grid3DLayout, "Offset");
					grid3DLayout.offset = newOffset;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Ignore Inactive:", "BoldLabel", GUILayout.Width(90F));
				bool newIgnoreInactive = EditorGUILayout.Toggle(grid3DLayout.ignoreInactive);
				if (newIgnoreInactive != grid3DLayout.ignoreInactive) {
					Undo.RecordObject(grid3DLayout, "IgnoreInactive");
					grid3DLayout.ignoreInactive = newIgnoreInactive;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("skips"), true);

			if (GUI.changed) {
				EditorUtility.SetDirty(grid3DLayout);
			}
		}

		private void DrawDirection(int i) {
			EditorGUILayout.BeginHorizontal();
			{
				if (i == 0) {
					GUILayout.Label("Direction0:", "BoldLabel", GUILayout.Width(66F));
					Direction oldDirection0 = grid3DLayout.direction0;
					Direction newDirection0 = (Direction)EditorGUILayout.EnumPopup(oldDirection0);
					if (newDirection0 != oldDirection0) {
						Undo.RecordObject(grid3DLayout, "Direction");
						grid3DLayout.direction0 = newDirection0;
						if (newDirection0 == grid3DLayout.direction1) {
							grid3DLayout.direction1 = oldDirection0;
						} else if (newDirection0 == grid3DLayout.direction2) {
							grid3DLayout.direction2 = oldDirection0;
						}
					}
				} else if (i == 1) {
					GUILayout.Label("Direction1:", "BoldLabel", GUILayout.Width(66F));
					List<Direction> list = new List<Direction>(new[] { Direction.X, Direction.Y, Direction.Z });
					list.Remove(grid3DLayout.direction0);
					string[] options = list.ConvertAll(dir => dir + "").ToArray();
					int[] values = list.ConvertAll(dir => (int)dir).ToArray();
					Direction oldDirection1 = grid3DLayout.direction1;
					Direction newDirection1 = (Direction)EditorGUILayout.IntPopup((int)oldDirection1, options, values);
					if (newDirection1 != oldDirection1) {
						Undo.RecordObject(grid3DLayout, "Direction");
						grid3DLayout.direction1 = newDirection1;
						grid3DLayout.direction2 = oldDirection1;
					}
				} else if (i == 2) {
					GUILayout.Label("Direction2:", "BoldLabel", GUILayout.Width(66F));
					string[] options = new string[] { grid3DLayout.direction2 + "" };
					EditorGUILayout.Popup(0, options);
				}

				// 上移下移删除按钮
				Type type = grid3DLayout.GetType();
				FieldInfo limitField = type.GetField("direction" + i + "Limit", BindingFlags.Instance | BindingFlags.Public);
				uint limit = limitField == null ? 0 : (uint)limitField.GetValue(grid3DLayout);
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
					Undo.RecordObject(grid3DLayout, "Remove");
					prevLimitField?.SetValue(grid3DLayout, (uint)0);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawSortAndAlign(int i) {
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(10F);

				Type type = grid3DLayout.GetType();

				GUILayout.Label("Sort" + i + ":", GUILayout.Width(38F));
				FieldInfo sortField = type.GetField("direction" + i + "Sort", BindingFlags.Instance | BindingFlags.Public);
				SortType oldSort = (SortType)sortField.GetValue(grid3DLayout);
				SortType newSort = (SortType)EditorGUILayout.EnumPopup(oldSort);
				if (newSort != oldSort) {
					Undo.RecordObject(grid3DLayout, "Sort");
					sortField.SetValue(grid3DLayout, newSort);
				}

				GUILayout.Label("Align" + i + ":", GUILayout.Width(42F));
				FieldInfo alignField = type.GetField("direction" + i + "Align", BindingFlags.Instance | BindingFlags.Public);
				AlignType oldAlign = (AlignType)alignField.GetValue(grid3DLayout);
				AlignType newAlign = (AlignType)EditorGUILayout.EnumPopup(oldAlign);
				if (newAlign != oldAlign) {
					Undo.RecordObject(grid3DLayout, "Align");
					alignField.SetValue(grid3DLayout, newAlign);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawDeltaAndLimit(int i) {
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space(10F);

				Type type = grid3DLayout.GetType();

				GUILayout.Label("Delta" + i + ":", GUILayout.Width(44F));
				FieldInfo deltaField = type.GetField("direction" + i + "Delta", BindingFlags.Instance | BindingFlags.Public);
				float oldDelta = (float)deltaField.GetValue(grid3DLayout);
				float newDelta = EditorGUILayout.FloatField(oldDelta);
				if (Mathf.Abs(newDelta - oldDelta) > Mathf.Epsilon) {
					Undo.RecordObject(grid3DLayout, "Delta");
					deltaField.SetValue(grid3DLayout, newDelta);
				}

				GUILayout.Label("CountLimit" + i + ":", GUILayout.Width(76F));
				FieldInfo limitField = type.GetField("direction" + i + "Limit", BindingFlags.Instance | BindingFlags.Public);
				if (limitField != null) {
					object limit = limitField.GetValue(grid3DLayout);
					uint oldLimit = (uint)limit;
					uint newLimit = (uint)Mathf.Max(EditorGUILayout.IntField((int)oldLimit), 0);
					if (newLimit != oldLimit) {
						Undo.RecordObject(grid3DLayout, "Limit");
						limitField.SetValue(grid3DLayout, newLimit);
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
			Type type = grid3DLayout.GetType();

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

			Direction prevDirection = (Direction)prevDirectionField.GetValue(grid3DLayout);
			SortType prevSort = (SortType)prevSortField.GetValue(grid3DLayout);
			AlignType prevAlign = (AlignType)prevAlignField.GetValue(grid3DLayout);
			float prevDelta = (float)prevDeltaField.GetValue(grid3DLayout);
			uint prevLimit = (uint)prevLimitField.GetValue(grid3DLayout);

			Direction nextDirection = (Direction)nextDirectionField.GetValue(grid3DLayout);
			SortType nextSort = (SortType)nextSortField.GetValue(grid3DLayout);
			AlignType nextAlign = (AlignType)nextAlignField.GetValue(grid3DLayout);
			float nextDelta = (float)nextDeltaField.GetValue(grid3DLayout);
			uint nextLimit = nextLimitField == null ? 0 : (uint)nextLimitField.GetValue(grid3DLayout);

			Undo.RecordObject(grid3DLayout, "UpDown");
			prevDirectionField.SetValue(grid3DLayout, nextDirection);
			nextDirectionField.SetValue(grid3DLayout, prevDirection);
			prevSortField.SetValue(grid3DLayout, nextSort);
			nextSortField.SetValue(grid3DLayout, prevSort);
			prevAlignField.SetValue(grid3DLayout, nextAlign);
			nextAlignField.SetValue(grid3DLayout, prevAlign);
			prevDeltaField.SetValue(grid3DLayout, nextDelta);
			nextDeltaField.SetValue(grid3DLayout, prevDelta);
			if (nextLimit > 0) {
				prevLimitField.SetValue(grid3DLayout, nextLimit);
				nextLimitField?.SetValue(grid3DLayout, prevLimit);
			}
			// 交换成功
			return true;
		}
	}
}