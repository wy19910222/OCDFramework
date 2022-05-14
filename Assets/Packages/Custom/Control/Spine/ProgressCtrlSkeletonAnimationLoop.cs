/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:51:42 717
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:51:42 706
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class ProgressCtrlSkeletonAnimationLoop : BaseProgressCtrlConst<bool> {
		protected override bool TargetValue {
			get => GetComponent<SkeletonAnimation>().loop;
			set => GetComponent<SkeletonAnimation>().loop = value;
		}
	}
}