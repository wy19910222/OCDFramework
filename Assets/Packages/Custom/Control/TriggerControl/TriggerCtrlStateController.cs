/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 20:37:18 063
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 20:37:18 054
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlStateController : BaseTriggerCtrl {
		[ComponentSelect]
		public StateController controller;
		[HideInInspector]
		public int index;

		protected override void DoTrigger() {
			if (controller) {
				controller.Index = index;
			}
		}
	}
}
