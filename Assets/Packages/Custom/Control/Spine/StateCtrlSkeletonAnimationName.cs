/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:48:47 530
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:48:47 525
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class StateCtrlSkeletonAnimationName : BaseStateCtrl<string> {
		protected override string TargetValue {
			get => GetComponent<SkeletonAnimation>().AnimationName;
			set => GetComponent<SkeletonAnimation>().AnimationName = value;
		}
	}
}