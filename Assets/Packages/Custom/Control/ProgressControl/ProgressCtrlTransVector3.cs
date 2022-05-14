/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 20:10:27 056
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 20:10:27 051
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlTransType {
		NONE,
		POSITION = 1,
		ANGLE = 2,
		SCALE = 3
	}

	public class ProgressCtrlTransVector3 : BaseProgressCtrlVector3 {
		public ProgressCtrlTransType type;
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
					case ProgressCtrlTransType.POSITION:
						return trans.localPosition;
					case ProgressCtrlTransType.ANGLE:
						return trans.eulerAngles;
					case ProgressCtrlTransType.SCALE:
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
						case ProgressCtrlTransType.POSITION:
							transform.DOLocalMove(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlTransType.ANGLE:
							transform.DORotate(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlTransType.SCALE:
							transform.DOScale(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
					}
				} else {
					switch (type) {
						case ProgressCtrlTransType.POSITION:
							transform.localPosition = value;
							break;
						case ProgressCtrlTransType.ANGLE:
							transform.localEulerAngles = value;
							break;
						case ProgressCtrlTransType.SCALE:
							transform.localScale = value;
							break;
					}
				}

			}
		}
	}
}