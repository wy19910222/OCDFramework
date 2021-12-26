/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

namespace StateControl {
	public class StateCtrlActive : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}
	}
}