/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:31:13 095
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 21:31:13 090
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class ProgressCtrlAnimatorFloat : BaseProgressCtrlFloat {
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