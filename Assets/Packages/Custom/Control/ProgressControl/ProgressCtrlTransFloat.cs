/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 14:22:51 442
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 14:22:51 436
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlTransPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2
	}

	public class ProgressCtrlTransFloat : BaseProgressCtrlFloat {
		public ProgressCtrlTransType type;
		public ProgressCtrlTransPart part = ProgressCtrlTransPart.X;
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;

		protected override float TargetValue {
			get {
				var trans = transform;
				switch (type) {
					case ProgressCtrlTransType.POSITION:
						return GetValue(trans.localPosition);
					case ProgressCtrlTransType.ANGLE:
						return GetValue(trans.eulerAngles);
					case ProgressCtrlTransType.SCALE:
						return GetValue(trans.localScale);
				}
				return 0;
			}
			set {
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					switch (type) {
						case ProgressCtrlTransType.POSITION:
							transform.DOLocalMove(SetValue(transform.localPosition, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlTransType.ANGLE:
							transform.DORotate(SetValue(transform.localEulerAngles, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlTransType.SCALE:
							transform.DOScale(SetValue(transform.localScale, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
					}
				} else {
					switch (type) {
						case ProgressCtrlTransType.POSITION:
							transform.localPosition = SetValue(transform.localPosition, value);
							break;
						case ProgressCtrlTransType.ANGLE:
							transform.localEulerAngles = SetValue(transform.localEulerAngles, value);
							break;
						case ProgressCtrlTransType.SCALE: {
							transform.localScale = SetValue(transform.localScale, value);
							break;
						}
					}
				}
			}
		}

		private float GetValue(Vector3 v3) {
			switch (part) {
				case ProgressCtrlTransPart.X:
					return v3.x;
				case ProgressCtrlTransPart.Y:
					return v3.y;
				case ProgressCtrlTransPart.Z:
					return v3.z;
				default:
					return 0;
			}
		}
		
		private Vector3 SetValue(Vector3 v3, float value) {
			switch (part) {
				case ProgressCtrlTransPart.X:
					v3.x = value;
					break;
				case ProgressCtrlTransPart.Y:
					v3.y = value;
					break;
				case ProgressCtrlTransPart.Z:
					v3.z = value;
					break;
			}
			return v3;
		}
	}
}