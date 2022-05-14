/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:49:03 129
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:49:03 126
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class StateCtrlSkeletonAnimationSpeed : BaseStateCtrl<float> {
		protected override float TargetValue {
			get => GetComponent<SkeletonAnimation>().timeScale;
			set => GetComponent<SkeletonAnimation>().timeScale = value;
		}
	}
}