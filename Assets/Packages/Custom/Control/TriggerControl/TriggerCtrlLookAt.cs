/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 17:11:17 160
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 17:11:17 148
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlLookAt : BaseTriggerCtrl {
		public Vector3 baseDir;
		public Transform target;
		
		protected override void DoTrigger() {
			Transform trans = transform;
			Vector3 fromDirection = trans.TransformDirection(baseDir);
			Vector3 toDirection = target.position - trans.position;
			trans.rotation = Quaternion.FromToRotation(fromDirection, toDirection);
		}
	}
}