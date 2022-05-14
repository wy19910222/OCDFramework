/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:31:07 371
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 21:31:07 366
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	[RequireComponent(typeof(Animator))]
	public class ProgressCtrlAnimatorBool : BaseProgressCtrlConst<bool> {
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