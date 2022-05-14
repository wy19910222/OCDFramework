/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

using UnityEngine;

namespace Control {
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