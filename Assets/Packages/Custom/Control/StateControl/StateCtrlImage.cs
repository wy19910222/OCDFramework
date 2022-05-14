/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:32:39 506
 * @LastEditor: wangyun
 * @EditTime: 2022-03-31 02:32:39 506
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Image))]
	public class StateCtrlImage : BaseStateCtrl<Sprite> {
		protected override Sprite TargetValue {
			get => GetComponent<Image>().sprite;
			set => GetComponent<Image>().sprite = value;
		}
	}
}