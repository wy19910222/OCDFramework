/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:32:01 149
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 21:32:01 142
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class ProgressCtrlAnimatorInt : BaseProgressCtrlInt {
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