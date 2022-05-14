/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:51:35 572
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:51:35 549
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class ProgressCtrlSkeletonAnimationName : BaseProgressCtrlConst<string> {
		protected override string TargetValue {
			get => GetComponent<SkeletonAnimation>().AnimationName;
			set => GetComponent<SkeletonAnimation>().AnimationName = value;
		}
	}
}