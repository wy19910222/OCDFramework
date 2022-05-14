/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:56:32 233
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:56:32 222
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class TriggerCtrlSkeletonAnimationSpeed : BaseTriggerCtrl {
		public float timeScale = 1;
		
		protected override void DoTrigger() {
			GetComponent<SkeletonAnimation>().timeScale = timeScale;
		}
	}
}