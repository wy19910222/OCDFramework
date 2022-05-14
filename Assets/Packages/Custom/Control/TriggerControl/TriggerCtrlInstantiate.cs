/*
 * @Author: wangyun
 * @CreateTime: 2022-05-02 19:01:25 097
 * @LastEditor: wangyun
 * @EditTime: 2022-05-02 19:01:25 101
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlInstantiate : BaseTriggerCtrl {
		public Transform prefab;
		public Transform pivot;
		public bool resetPos = true;
		public bool resetRot = true;
		public bool activeAtOnce = true;
		
		protected override void DoTrigger() {
			if (pivot) {
				Vector3 pos = resetPos ? pivot.position : pivot.TransformPoint(prefab.localPosition);
				Quaternion rot = resetRot ? pivot.rotation : pivot.rotation * prefab.localRotation;
				Transform trans = Instantiate(prefab, pos, rot, pivot);
				if (activeAtOnce) {
					trans.gameObject.SetActive(true);
				}
			} else {
				Vector3 pos = resetPos ? Vector3.zero : prefab.position;
				Quaternion rot = resetRot ? Quaternion.identity : prefab.rotation;
				Transform trans = Instantiate(prefab, pos, rot);
				if (activeAtOnce) {
					trans.gameObject.SetActive(true);
				}
			}
		}
	}
}