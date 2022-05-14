/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 11:03:01 799
 * @LastEditor: wangyun
 * @EditTime: 2022-04-17 11:03:01 799
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorInt : BaseStateCtrl<int> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Int)]
		public string paramName;
		[ReadOnly]
		public int paramValue;
		protected override int TargetValue {
			get => paramValue = GetComponent<Animator>().GetInteger(paramName);
			set => GetComponent<Animator>().SetInteger(paramName, paramValue = value);
		}
	}
}