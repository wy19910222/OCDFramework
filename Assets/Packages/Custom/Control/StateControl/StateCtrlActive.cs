/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

namespace Control {
	public class StateCtrlActive : BaseStateCtrl<bool> {
		protected override bool TargetValue {
			get => gameObject.activeSelf;
			set => gameObject.SetActive(value);
		}
	}
}