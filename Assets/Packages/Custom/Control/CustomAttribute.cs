/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-05-10 21:50:36 621
 */

using System;
using System.Diagnostics;
using UnityEngine;

namespace Control {
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class StateControllerSelectAttribute : PropertyAttribute {
	}
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class ProgressControllerSelectAttribute : PropertyAttribute {
	}
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class SelfAnimatorParamSelectAttribute : PropertyAttribute {
		public AnimatorControllerParameterType Type { get; }
		public SelfAnimatorParamSelectAttribute() {
		}
		public SelfAnimatorParamSelectAttribute(AnimatorControllerParameterType type) {
			Type = type;
		}
	}
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class ComponentSelectAttribute : PropertyAttribute {
		public bool ExceptSelf { get; }
		public ComponentSelectAttribute(bool exceptSelf = false) {
			ExceptSelf = exceptSelf;
		}
	}
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class CanResetCurveAttribute : PropertyAttribute {
	}
}