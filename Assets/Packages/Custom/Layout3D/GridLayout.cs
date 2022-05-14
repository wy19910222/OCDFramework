/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System.Collections.Generic;
using UnityEngine;

namespace Layout3D {
	public class GridLayout : MonoBehaviour {
		public Direction direction0 = Direction.X;
		public SortType direction0Sort = SortType.POSITIVE;
		public AlignType direction0Align = AlignType.POSITIVE;
		public float direction0Delta;
		public uint direction0Limit;

		public Direction direction1 = Direction.Y;
		public SortType direction1Sort = SortType.POSITIVE;
		public AlignType direction1Align = AlignType.POSITIVE;
		public float direction1Delta;
		public uint direction1Limit;

		public Direction direction2 = Direction.Z;
		public SortType direction2Sort = SortType.POSITIVE;
		public AlignType direction2Align = AlignType.POSITIVE;
		public float direction2Delta;

		public Vector2Int range = Vector2Int.down;
		public Vector3 offset = Vector3.zero;
		public List<int> skips = new List<int>();
		public bool ignoreInactive = true;

		[ContextMenu("RepositionChild")]
		public void RepositionChild() {
			RepositionChild(transform.childCount - 1);
		}
		public void RepositionChild(int childIndex) {
			if (childIndex >= range.x && (range.y < 0 && childIndex < transform.childCount || range.y >= 0 && childIndex < range.x + range.y)) {
				// 取出指定范围内所有子物体
				List<Transform> children = GetChildren();
				int totalCount = children.Count;
				if (totalCount == 0) {
					return;
				}

				// 插空
				ApplySkips(children);
				totalCount = children.Count;

				// 确定行列层数
				int[] counts = GetCounts(totalCount);

				// 计算行列层索引
				Transform child = transform.GetChild(childIndex);
				int index = children.IndexOf(child);
				int temp1 = index / counts[0];
				int i = temp1 / counts[1];
				int j = temp1 - i * counts[1];
				int k = index - temp1 * counts[0];

				// 计算位置
				int dir0 = (int)direction0;
				int dir1 = (int)direction1;
				int dir2 = (int)direction2;
				Vector3 pos = Vector3.zero;
				float _k = direction0Sort == SortType.POSITIVE ? k : counts[0] - 1 - k;
				_k -= (counts[0] - 1) * (int)direction0Align * 0.5F;
				pos[dir0] = _k * direction0Delta;
				float _j = direction1Sort == SortType.POSITIVE ? j : counts[1] - 1 - j;
				_j -= (counts[1] - 1) * (int)direction1Align * 0.5F;
				pos[dir1] = _j * direction1Delta;
				float _i = direction2Sort == SortType.POSITIVE ? i : counts[2] - 1 - i;
				_i -= (counts[2] - 1) * (int)direction2Align * 0.5F;
				pos[dir2] = _i * direction2Delta;

				// 赋值位置
				child.localPosition = pos + offset;
			}
		}

		[ContextMenu("Reposition")]
		public void Reposition() {
			// 取出指定范围内所有子物体
			List<Transform> children = GetChildren();
			int totalCount = children.Count;
			if (totalCount == 0) {
				return;
			}

			// 插空
			ApplySkips(children);
			totalCount = children.Count;

			// 确定行列层数
			int[] counts = GetCounts(totalCount);

			// 遍历行列层
			int dir0 = (int)direction0;
			int dir1 = (int)direction1;
			int dir2 = (int)direction2;
			for (int i = 0; i < counts[2]; ++i) {
				for (int j = 0; j < counts[1]; ++j) {
					for (int k = 0; k < counts[0]; ++k) {
						int index = (i * counts[1] + j) * counts[0] + k;
						if (index < totalCount) {
							Transform child = children[index];
							if (child) {
								// 计算位置
								Vector3 pos = Vector3.zero;
								float _k = direction0Sort == SortType.POSITIVE ? k : counts[0] - 1 - k;
								_k -= (counts[0] - 1) * (int)direction0Align * 0.5F;
								pos[dir0] = _k * direction0Delta;
								float _j = direction1Sort == SortType.POSITIVE ? j : counts[1] - 1 - j;
								_j -= (counts[1] - 1) * (int)direction1Align * 0.5F;
								pos[dir1] = _j * direction1Delta;
								float _i = direction2Sort == SortType.POSITIVE ? i : counts[2] - 1 - i;
								_i -= (counts[2] - 1) * (int)direction2Align * 0.5F;
								pos[dir2] = _i * direction2Delta;

								// 赋值位置
								child.localPosition = pos + offset;
							}
						}
					}
				}
			}
		}

		// 以指定范围取出子节点
		private List<Transform> GetChildren() {
			List<Transform> children = new List<Transform>();
			int childrenCount = transform.childCount;
			int start = range.x;
			int end = range.y < 0 ? childrenCount : Mathf.Min(childrenCount, range.x + range.y);
			for (int i = start; i < end; ++i) {
				Transform child = transform.GetChild(i);
				if (!ignoreInactive || child.gameObject.activeSelf) {
					children.Add(child);
				}
			}
			return children;
		}

		// 插空
		private void ApplySkips(List<Transform> children) {
			List<int> posSkips = new List<int>();
			List<int> negSkips = new List<int>();
			foreach (var skip in skips) {
				if (skip >= 0) {
					posSkips.Add(skip);
				} else {
					negSkips.Add(-skip - 1);
				}
			}
			posSkips.Sort();
			negSkips.Sort();

			int totalCount = children.Count;
			foreach (var skipIndex in posSkips) {
				if (skipIndex < totalCount) {
					// 已经插空了就不需要重复查了，重复插反而把之前的空位挤走了
					if (children[skipIndex]) {
						children.Insert(skipIndex, null);
						totalCount++;
					}
				}
			}
			foreach (var skip in negSkips) {
				int skipIndex = totalCount - 1 - skip;
				if (skipIndex >= 0) {
					// 已经插空了就不需要重复查了，重复插反而把之前的空位挤走了
					if (children[skipIndex]) {
						children.Insert(skipIndex + 1, null);
						totalCount++;
					}
				} else if (skipIndex == -1) {
					// 从后往前插到最前面，相当于在最前面新增一个空位
					children.Insert(0, null);
					totalCount++;
				}
			}
		}

		// 确定行列层数
		private int[] GetCounts(int totalCount) {
			int[] counts = new int[3];
			if (direction0Limit == 0 || totalCount <= direction0Limit) {
				counts[0] = totalCount;
				counts[1] = 1;
				counts[2] = 1;
			} else if (direction1Limit == 0 || (totalCount <= direction0Limit * direction1Limit)) {
				counts[0] = (int)direction0Limit;
				counts[1] = Mathf.CeilToInt((float)totalCount / direction0Limit);
				counts[2] = 1;
			} else {
				counts[0] = (int)direction0Limit;
				counts[1] = (int)direction1Limit;
				counts[2] = Mathf.CeilToInt((float)totalCount / direction0Limit / direction1Limit);
			}
			return counts;
		}
	}
}

