/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:35 694
 * @LastEditor: wangyun
 * @EditTime: 2022-03-31 02:11:35 694
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	[RequireComponent(typeof(Text))]
	public class StateCtrlText : BaseStateCtrl<string> {
		protected override string TargetValue {
			get => GetComponent<Text>().text;
			set => GetComponent<Text>().text = value;
		}
	}
}