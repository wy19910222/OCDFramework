/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:34 542
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 14:22:34 536
 */

using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {

	public abstract class BaseProgressCtrlColor : BaseProgressCtrl {
		[SerializeField, ReadOnly]
		private Color m_FromValue;
		[SerializeField, ReadOnly]
		private Color m_ToValue;
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_CurveR = new AnimationCurve(new Keyframe(0, 0));
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_CurveG = new AnimationCurve(new Keyframe(0, 0));
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_CurveB = new AnimationCurve(new Keyframe(0, 0));
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_CurveA = new AnimationCurve(new Keyframe(0, 0));

		private AnimationCurve[] m_Curves;

		private void Reset() {
			controller = GetComponentInParent<ProgressController>();
			m_ToValue = m_FromValue = TargetValue;
			m_Curves = new []{m_CurveR, m_CurveG, m_CurveB, m_CurveA};
		}

		private Color GetValue(float progress) {
			var ret = Color.black;
			for (int part = 0; part < 4; part++) {
				var t = m_Curves[part].Evaluate(progress);
				ret[part] = Lerp(m_FromValue[part], m_ToValue[part], t);
			}
			return ret;
		}

		private void SetValue(float progress, Color value) {
			for (int part = 0; part < 4; part++) {
				var t = GetT(m_FromValue[part], m_ToValue[part], value[part]);
				AnimationCurve curve = m_Curves[part];
				var curveKeys = curve.keys;
				for (int index = 0, length = curveKeys.Length; index < length; index++) {
					var curveKey = curveKeys[index];
					if (Mathf.Abs(curveKey.time - progress) < Mathf.Epsilon) {
						curve.RemoveKey(index);
						break;
					}
				}
				AddKey(curve, progress, t);
			}
		}

		protected void AddKey(AnimationCurve curve, float progress, float t) {
			float curT = curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - curve.Evaluate(progress - deltaProgress)) / deltaProgress;
				curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				curve.AddKey(progress, t);
#if UNITY_EDITOR
				// Clamped auto
				for (int i = 1, length = curve.length - 1; i < length; ++i) {
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
				}
#endif
			}
		}

		protected virtual Color TargetValue { get; set; }

		protected float Lerp(float from, float to, float t) {
			return from + t * (to - from);
		}

		protected float GetT(float from, float to, float value) {
			var delta = to - from;
			return delta == 0 ? 0 : (value - from) / delta;
		}
		
		protected bool Equals(float value1, float value2) {
			return Mathf.Abs(value1 - value2) < Mathf.Epsilon;
		}

		public override void Capture(float progress) {
			Color targetValue = TargetValue;
			// 如果form和to相同，则把to设置成新记录的值，然后曲线上所有点的纵坐标都设置为0
			for (int part = 0; part < 4; part++) {
				if (Equals(m_FromValue[part], m_ToValue[part])) {
					m_ToValue[part] = targetValue[part];
					AnimationCurve curve = m_Curves[part];
					var curveKeys = curve.keys;
					for (int index = 0, length = curveKeys.Length; index < length; index++) {
						var curveKey = curveKeys[index];
						curve.MoveKey(index, new Keyframe(curveKey.time, 0));
					}
				}
			}
			SetValue(progress, targetValue);
#if UNITY_EDITOR
			// 缩放curve
			for (int part = 0; part < 4; part++) {
				AnimationCurve curve = m_Curves[part];
				var curveKeys = curve.keys;
				var curveLength = curveKeys.Length;
				if (curveLength > 0) {
					if (curveLength == 1) {
						var keyFrame = curveKeys[0];
						if (keyFrame.time != 0 || keyFrame.value != 0) {
							// 把唯一的点重置成(0, 0)
							float from = m_FromValue[part], to = m_ToValue[part];
							m_FromValue[part] = Lerp(from, to, keyFrame.value);
							curve.RemoveKey(0);
							curve.AddKey(0, 0);
						}
					} else {
						// 找到最低点和最高点
						float minT = float.MaxValue, maxT = float.MinValue;
						foreach (var curveKey in curveKeys) {
							if (curveKey.value < minT) {
								minT = curveKey.value;
							}
							if (curveKey.value > maxT) {
								maxT = curveKey.value;
							}
						}
						float deltaT = maxT - minT;
						if (deltaT < Mathf.Epsilon) {
							deltaT = 1;
						}
						// 缩放到最低点为0，最高点为1
						m_Curves[part] = curve = new AnimationCurve();
						foreach (var curveKey in curveKeys) {
							curve.AddKey(new Keyframe(
									curveKey.time,
									(curveKey.value - minT) / deltaT,
									curveKey.inTangent / deltaT,
									curveKey.outTangent / deltaT
							));
						}
						float from = m_FromValue[part], to = m_ToValue[part];
						(m_FromValue[part], m_ToValue[part]) = (Lerp(from, to, minT), Lerp(from, to, maxT));
					}
				}
			}
			(m_CurveR, m_CurveG, m_CurveB, m_CurveA) = (m_Curves[0], m_Curves[1], m_Curves[2], m_Curves[3]);
#endif
		}

		public override void Apply(float progress) {
			TargetValue = GetValue(progress);
		}
	}
}