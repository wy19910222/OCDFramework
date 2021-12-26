/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using UnityEngine;

namespace StateControl {
	public class StateCtrlEnable : BaseStateCtrl<bool> {

		[SelfComponentSelect]
		public Behaviour target;

		protected override bool TargetValue {
			get => target && target.enabled;
			set {
				if (target) target.enabled = value;
			}
		}
	}
}