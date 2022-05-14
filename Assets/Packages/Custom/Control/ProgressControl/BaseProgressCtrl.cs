/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:34 542
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 14:22:34 536
 */

using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Control {
	public abstract class BaseProgressCtrl : MonoBehaviour {
		[ProgressControllerSelect]
		public ProgressController controller;
		public abstract void Capture(float progress);
		public abstract void Apply(float progress);
	}

	public abstract class BaseProgressCtrl<TValue> : BaseProgressCtrl {
		[SerializeField, ReadOnly]
		private TValue m_FromValue;
		[SerializeField, ReadOnly]
		private TValue m_ToValue;
		[SerializeField, CanResetCurve]
		protected AnimationCurve m_Curve = new AnimationCurve(new Keyframe(0, 0));

		protected void Reset() {
			controller = GetComponentInParent<ProgressController>();
			m_ToValue = m_FromValue = TargetValue;
		}

		private TValue GetValue(float progress) {
			var t = m_Curve.Evaluate(progress);
			return Lerp(m_FromValue, m_ToValue, t);
		}

		private void SetValue(float progress, TValue value) {
			var t = GetT(m_FromValue, m_ToValue, value);
			var curveKeys = m_Curve.keys;
			for (int index = 0, length = curveKeys.Length; index < length; index++) {
				var curveKey = curveKeys[index];
				if (Mathf.Abs(curveKey.time - progress) < Mathf.Epsilon) {
					m_Curve.RemoveKey(index);
					break;
				}
			}
			AddKey(progress, t);
		}

		protected virtual void AddKey(float progress, float t) {
			m_Curve.AddKey(progress, t);
		}

		protected virtual TValue TargetValue { get; set; }

		protected abstract TValue Lerp(TValue from, TValue to, float t);
		protected abstract float GetT(TValue from, TValue to, TValue value);
		protected abstract bool Equals(TValue value1, TValue value2);

		public override void Capture(float progress) {
			TValue targetValue = TargetValue;
			// 如果form和to相同，则把to设置成新记录的值，然后曲线上所有点的纵坐标都设置为0
			if (Equals(m_FromValue, m_ToValue)) {
				m_ToValue = targetValue;
				var curveKeys = m_Curve.keys;
				for (int index = 0, length = curveKeys.Length; index < length; index++) {
					var curveKey = curveKeys[index];
					m_Curve.MoveKey(index, new Keyframe(curveKey.time, 0));
				}
			}
			SetValue(progress, targetValue);
#if UNITY_EDITOR
			// 缩放curve
			ScaleCurve();
#endif
		}


		protected virtual void ScaleCurve() {
				// 缩放到最低点为0，最高点为1
				var curveKeys = m_Curve.keys;
				var curveLength = curveKeys.Length;
				if (curveLength > 0) {
					if (curveLength == 1) {
						var keyFrame = curveKeys[0];
						if (keyFrame.time != 0 || keyFrame.value != 0) {
							// 把唯一的点重置成(0, 0)
							m_FromValue = Lerp(m_FromValue, m_ToValue, keyFrame.value);
							m_Curve.RemoveKey(0);
							m_Curve.AddKey(0, 0);
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
						m_Curve = new AnimationCurve();
						foreach (var curveKey in curveKeys) {
							m_Curve.AddKey(new Keyframe(
									curveKey.time,
									(curveKey.value - minT) / deltaT,
									curveKey.inTangent / deltaT,
									curveKey.outTangent / deltaT
							));
						}
						(m_FromValue, m_ToValue) = (Lerp(m_FromValue, m_ToValue, minT), Lerp(m_FromValue, m_ToValue, maxT));
					}
				}
		}

		public override void Apply(float progress) {
			TargetValue = GetValue(progress);
		}
	}
	
	public abstract class BaseProgressCtrlFloat : BaseProgressCtrl<float> {
		protected override float Lerp(float from, float to, float t) {
			return from + t * (to - from);
		}

		protected override float GetT(float from, float to, float value) {
			var delta = to - from;
			return delta == 0 ? 0 : (value - from) / delta;
		}

		protected override bool Equals(float value1, float value2) {
			return Mathf.Abs(value1 - value2) < Mathf.Epsilon;
		}

		protected override void AddKey(float progress, float t) {
			float curT = m_Curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - m_Curve.Evaluate(progress - deltaProgress)) / deltaProgress;
				m_Curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				m_Curve.AddKey(progress, t);
#if UNITY_EDITOR
				// Clamped auto
				for (int i = 1, length = m_Curve.length - 1; i < length; ++i) {
					UnityEditor.AnimationUtility.SetKeyLeftTangentMode(m_Curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
					UnityEditor.AnimationUtility.SetKeyRightTangentMode(m_Curve, i, UnityEditor.AnimationUtility.TangentMode.ClampedAuto);
				}
#endif
			}
		}
	}
	
	public abstract class BaseProgressCtrlInt : BaseProgressCtrl<int> {
		protected override int Lerp(int from, int to, float t) {
			return Mathf.RoundToInt(from + t * (to - from));
		}

		protected override float GetT(int from, int to, int value) {
			var delta = to - from;
			return delta == 0 ? 0 : (float) (value - from) / delta;
		}

		protected override bool Equals(int value1, int value2) {
			return value1 == value2;
		}

		protected override void AddKey(float progress, float t) {
			float curT = m_Curve.Evaluate(progress);
			if (Mathf.Abs(curT - t) < Mathf.Epsilon) {
				const float deltaProgress = 0.001F;
				float tangent = (t - m_Curve.Evaluate(progress - deltaProgress)) / deltaProgress;
				m_Curve.AddKey(new Keyframe(progress, t, tangent, tangent));
			} else {
				var curveKeys = m_Curve.keys;
				var curveLength = curveKeys.Length;
				var index = curveLength;
				for (int i = 0; i < curveLength; i++) {
					if (progress < curveKeys[i].time) {
						index = i;
						break;
					}
				}
				if (index > 0) {
					var prev = curveKeys[index - 1];
					var inTangent = (t - prev.value) / (progress - prev.time);
					if (index < curveLength) {
						var next = curveKeys[index];
						var outTangent = (t - next.value) / (progress - next.time);
						m_Curve.AddKey(new Keyframe(progress, t, inTangent, outTangent));
						m_Curve.MoveKey(index - 1, new Keyframe(prev.time, prev.value, prev.inTangent, inTangent));
						m_Curve.MoveKey(index + 1, new Keyframe(next.time, next.value, outTangent, next.outTangent));
					} else {
						m_Curve.AddKey(new Keyframe(progress, t, inTangent, inTangent));
						m_Curve.MoveKey(index - 1, new Keyframe(prev.time, prev.value, prev.inTangent, inTangent));
					}
				} else {
					if (index < curveLength) {
						var next = curveKeys[index];
						var outTangent = (t - next.value) / (progress - next.time);
						m_Curve.AddKey(new Keyframe(progress, t, outTangent, outTangent));
						m_Curve.MoveKey(index + 1, new Keyframe(next.time, next.value, outTangent, next.outTangent));
					} else {
						m_Curve.AddKey(progress, t);
					}
				}
			}
		}
	}
	
	public abstract class BaseProgressCtrlConst<TValue> : BaseProgressCtrl<TValue> where TValue : IEquatable<TValue>  {
		protected override TValue Lerp(TValue from, TValue to, float t) {
			return t >= 1 ? to : from;
		}

		protected override float GetT(TValue from, TValue to, TValue value) {
			return object.Equals(value, to) && !object.Equals(value, from) ? 1 : 0;
		}

		protected override bool Equals(TValue value1, TValue value2) {
			return object.Equals(value1, value2);
		}

		protected override void AddKey(float progress, float t) {
			m_Curve.AddKey(new Keyframe(progress, t, Mathf.Infinity, Mathf.Infinity));
		}
	}
}