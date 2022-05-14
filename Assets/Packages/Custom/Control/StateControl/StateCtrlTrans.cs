/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-03-30 16:21:41 627
 */

using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Control {
	public enum StateCtrlTransType {
		NONE,
		POSITION = 1,
		ANGLE = 2,
		SCALE = 3
	}

	[Flags]
	public enum StateCtrlTransPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2
	}

	public class StateCtrlTrans : BaseStateCtrl<Vector3> {
		public StateCtrlTransType type;
		public StateCtrlTransPart part = StateCtrlTransPart.X | StateCtrlTransPart.Y | StateCtrlTransPart.Z;
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;

		protected override Vector3 TargetValue {
			get {
				var trans = transform;
				switch (type) {
					case StateCtrlTransType.POSITION:
						return trans.localPosition;
					case StateCtrlTransType.ANGLE:
						return trans.eulerAngles;
					case StateCtrlTransType.SCALE:
						return trans.localScale;
				}
				return Vector3.zero;
			}
			set {
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					switch (type) {
						case StateCtrlTransType.POSITION:
							transform.DOLocalMove(SetValue(transform.localPosition, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case StateCtrlTransType.ANGLE:
							transform.DOLocalRotate(SetValue(transform.localEulerAngles, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case StateCtrlTransType.SCALE:
							transform.DOScale(SetValue(transform.localScale, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
					}
				} else {
					switch (type) {
						case StateCtrlTransType.POSITION:
							transform.localPosition = SetValue(transform.localPosition, value);
							break;
						case StateCtrlTransType.ANGLE:
							transform.localEulerAngles = SetValue(transform.localEulerAngles, value);
							break;
						case StateCtrlTransType.SCALE:
							transform.localScale = SetValue(transform.localScale, value);
							break;
					}
				}
			}
		}
		
		private Vector3 SetValue(Vector3 v3, Vector3 value) {
			if ((part & StateCtrlTransPart.X) != 0) {
				v3.x = value.x;
			}
			if ((part & StateCtrlTransPart.Y) != 0) {
				v3.y = value.y;
			}
			if ((part & StateCtrlTransPart.Z) != 0) {
				v3.z = value.z;
			}
			return v3;
		}
	}
}