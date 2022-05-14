/*
 * @Author: wangyun
 * @CreateTime: 2022-04-18 21:27:57 579
 * @LastEditor: wangyun
 * @EditTime: 2022-04-18 21:27:57 573
 */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public enum ProgressCtrlRectTransType {
		NONE,
		ANCHOR_MIN = 1,
		ANCHOR_MAX = 2,
		ANCHORED_POSITION = 3,
		SIZE_DELTA = 4,
		PIVOT = 5
	}

	[RequireComponent(typeof(RectTransform))]
	public class ProgressCtrlRectTrans : BaseProgressCtrlVector2 {
		public ProgressCtrlRectTransType type;
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;

		protected override Vector2 TargetValue {
			get {
				var trans = transform as RectTransform;
				switch (type) {
					case ProgressCtrlRectTransType.ANCHOR_MIN:
						return trans.anchorMin;
					case ProgressCtrlRectTransType.ANCHOR_MAX:
						return trans.anchorMax;
					case ProgressCtrlRectTransType.ANCHORED_POSITION:
						return trans.anchoredPosition;
					case ProgressCtrlRectTransType.SIZE_DELTA:
						return trans.sizeDelta;
					case ProgressCtrlRectTransType.PIVOT:
						return trans.pivot;
				}
				return Vector2.zero;
			}
			set {
				var trans = transform as RectTransform;
#if UNITY_EDITOR
				if (tween && Application.isPlaying) {
#else
				if (tween) {
#endif
					switch (type) {
						case ProgressCtrlRectTransType.ANCHOR_MIN:
							DOTween.To(() => trans.anchorMin, _value => trans.anchorMin = _value, value, tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlRectTransType.ANCHOR_MAX:
							DOTween.To(() => trans.anchorMax, _value => trans.anchorMax = _value, value, tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlRectTransType.ANCHORED_POSITION:
							DOTween.To(() => trans.anchoredPosition, _value => trans.anchoredPosition = _value, value, tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlRectTransType.SIZE_DELTA:
							DOTween.To(() => trans.sizeDelta, _value => trans.sizeDelta = _value, value, tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case ProgressCtrlRectTransType.PIVOT:
							DOTween.To(() => trans.pivot, _value => trans.pivot = _value, value, tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
					}
				} else {
					switch (type) {
						case ProgressCtrlRectTransType.ANCHOR_MIN:
							trans.anchorMin = value;
							break;
						case ProgressCtrlRectTransType.ANCHOR_MAX:
							trans.anchorMax = value;
							break;
						case ProgressCtrlRectTransType.ANCHORED_POSITION:
							trans.anchoredPosition = value;
							break;
						case ProgressCtrlRectTransType.SIZE_DELTA:
							trans.sizeDelta = value;
							break;
						case ProgressCtrlRectTransType.PIVOT:
							trans.pivot = value;
							break;
					}
				}
			}
		}
	}
}