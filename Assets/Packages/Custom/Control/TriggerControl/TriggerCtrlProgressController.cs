/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 20:33:25 096
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 20:33:25 086
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlProgressController : BaseTriggerCtrl {
		[ComponentSelect]
		public ProgressController controller;
		[Range(0, 1)]
		public float progress;

		protected override void DoTrigger() {
			if (controller) {
				controller.Progress = progress;
			}
		}
	}
}