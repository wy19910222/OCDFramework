using System.Collections.Generic;
using UnityEngine;
/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

namespace Layout3D {
	public class LinearLayout : MonoBehaviour {
		public Direction direction = Direction.X;
		public SortType sort = SortType.POSITIVE;
		public AlignType align = AlignType.POSITIVE;
		public float gap;
		public bool ignoreInactive = true;

		[ContextMenu("Reposition")]
		public void Reposition() {
			List<Transform> children = new List<Transform>();
			foreach (Transform child in transform) {
				if (!ignoreInactive || child.gameObject.activeSelf) {
					children.Add(child);
				}
			}
			int totalCount = children.Count;
			if (totalCount == 0) {
				return;
			}

			int dir = (int)direction;
			float cur = 0;
			for (int i = 0; i < totalCount; ++i) {
				int index = sort == SortType.POSITIVE ? i : totalCount - 1 - i;
				Transform child = children[index];
				Vector2 minMax = getMinMax(child, dir);
				Vector3 pos = Vector3.zero;
				pos[dir] = cur - minMax[0];
				child.localPosition = pos;

				cur += minMax[1] - minMax[0] + gap;
			}
			float totalSize = cur - gap;
			float offset = -totalSize * (int)align * 0.5F;
			for (int i = 0; i < totalCount; ++i) {
				Transform child = children[i];
				Vector3 pos = child.localPosition;
				pos[dir] += offset;
				child.localPosition = pos;
			}
		}

		private Vector2 getMinMax(Transform trans, int dir) {
			Vector2 minMax = Vector2.zero;
			Transform myTrans = transform;
			Vector3 pivotPoint = myTrans.InverseTransformPoint(trans.position);
			MeshRenderer[] renderers = trans.GetComponentsInChildren<MeshRenderer>(!ignoreInactive);
			foreach (var renderer in renderers) {
				Bounds bounds = renderer.bounds;
				Vector3 minPoint = myTrans.InverseTransformPoint(bounds.min);
				Vector3 maxPoint = myTrans.InverseTransformPoint(bounds.max);
				float min = Mathf.Min(minPoint[dir], maxPoint[dir]) - pivotPoint[dir];
				float max = Mathf.Max(minPoint[dir], maxPoint[dir]) - pivotPoint[dir];
				if (minMax[0] > min) {
					minMax[0] = min;
				}
				if (minMax[1] < max) {
					minMax[1] = max;
				}
			}
			return minMax;
		}
	}
}
