/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 09:54:56 864
 * @LastEditor: wangyun
 * @EditTime: 2022-04-17 09:54:56 864
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorBool : BaseStateCtrl<bool> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Bool)]
		public string paramName;
		[ReadOnly]
		public bool paramValue;
		protected override bool TargetValue {
			get => paramValue = GetComponent<Animator>().GetBool(paramName);
			set => GetComponent<Animator>().SetBool(paramName, paramValue = value);
		}
	}
}