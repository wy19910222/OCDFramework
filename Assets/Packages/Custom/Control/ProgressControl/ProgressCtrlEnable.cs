/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2022-03-30 16:21:41 627
 */

using UnityEngine;

namespace Control {
	public class ProgressCtrlEnable : BaseProgressCtrlConst<bool> {
		[ComponentSelect(true)]
		public Behaviour target;

		protected override bool TargetValue {
			get => target && target.enabled;
			set {
				if (target) target.enabled = value;
			}
		}
	}
}