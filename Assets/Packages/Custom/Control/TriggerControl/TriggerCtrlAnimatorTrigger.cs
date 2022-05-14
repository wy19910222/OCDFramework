/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 01:47:15 263
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 01:47:15 267
 */

using UnityEngine;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class TriggerCtrlAnimatorTrigger : BaseTriggerCtrl {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Trigger)]
		public string paramName;
		
		protected override void DoTrigger() {
			GetComponent<Animator>().SetTrigger(paramName);
		}
	}
}