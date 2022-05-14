/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:47:11 465
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:47:11 460
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class TriggerCtrlSkeletonAnimationLoop : BaseTriggerCtrl {
		public bool loop;
		
		protected override void DoTrigger() {
			GetComponent<SkeletonAnimation>().loop = loop;
		}
	}
}