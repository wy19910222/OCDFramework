/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using UnityEngine;

namespace StateControl {
	[RequireComponent(typeof(MeshFilter))]
	public class StateCtrlMesh : BaseStateCtrl<Mesh> {
		protected override Mesh TargetValue {
			get => GetComponent<MeshFilter>()?.sharedMesh;
			set {
				var mf = GetComponent<MeshFilter>();
				if (mf) mf.sharedMesh = value;
			}
		}
	}
}