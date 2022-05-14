/*
 * @Author: wangyun
 * @CreateTime: 2022-04-24 21:51:29 462
 * @LastEditor: wangyun
 * @EditTime: 2022-04-24 21:51:29 457
 */

using UnityEngine;
using Spine.Unity;

namespace Control {
	[RequireComponent(typeof(SkeletonAnimation))]
	public class ProgressCtrlSkeletonAnimationSpeed : BaseProgressCtrlFloat {
		protected override float TargetValue {
			get => GetComponent<SkeletonAnimation>().timeScale;
			set => GetComponent<SkeletonAnimation>().timeScale = value;
		}
	}
}