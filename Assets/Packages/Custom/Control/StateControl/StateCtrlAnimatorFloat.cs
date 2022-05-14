/*
 * @Author: wangyun
 * @CreateTime: 2022-04-17 12:15:01 799
 * @LastEditor: wangyun
 * @EditTime: 2022-04-17 12:15:01 799
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class StateCtrlAnimatorFloat : BaseStateCtrl<float> {
		[SelfAnimatorParamSelect(AnimatorControllerParameterType.Float)]
		public string paramName;
		[ReadOnly]
		public float paramValue;
		protected override float TargetValue {
			get => paramValue = GetComponent<Animator>().GetFloat(paramName);
			set => GetComponent<Animator>().SetFloat(paramName, paramValue = value);
		}
	}
}