/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 01:47:15 267
 */

using System.Collections.Generic;

namespace Control {
	public class TriggerCtrlTrigger : BaseTriggerCtrl {
		[ComponentSelect(true)]
		public List<BaseTriggerCtrl> triggers = new List<BaseTriggerCtrl>();

		protected override void DoTrigger() {
			foreach (var trigger in triggers) {
				trigger.Trigger();
			}
		}
	}
}