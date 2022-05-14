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
	public enum StateCtrlRectTransType {
		NONE,
		ANCHOR_MIN = 1,
		ANCHOR_MAX = 2,
		ANCHORED_POSITION = 3,
		SIZE_DELTA = 4,
		PIVOT = 5
	}

	[Flags]
	public enum StateCtrlRectTransPart {
		X = 1 << 0,
		Y = 1 << 1
	}

	[RequireComponent(typeof(RectTransform))]
	public class StateCtrlRectTrans : BaseStateCtrl<Vector2> {
		public StateCtrlRectTransType type;
		public StateCtrlRectTransPart part = StateCtrlRectTransPart.X | StateCtrlRectTransPart.Y;
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
					case StateCtrlRectTransType.ANCHOR_MIN:
						return trans.anchorMin;
					case StateCtrlRectTransType.ANCHOR_MAX:
						return trans.anchorMax;
					case StateCtrlRectTransType.ANCHORED_POSITION:
						return trans.anchoredPosition;
					case StateCtrlRectTransType.SIZE_DELTA:
						return trans.sizeDelta;
					case StateCtrlRectTransType.PIVOT:
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
						case StateCtrlRectTransType.ANCHOR_MIN:
							DOTween.To(() => trans.anchorMin, _value => trans.anchorMin = _value, SetValue(trans.anchorMin, value), tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case StateCtrlRectTransType.ANCHOR_MAX:
							DOTween.To(() => trans.anchorMax, _value => trans.anchorMax = _value, SetValue(trans.anchorMax, value), tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case StateCtrlRectTransType.ANCHORED_POSITION:
							DOTween.To(() => trans.anchoredPosition, _value => trans.anchoredPosition = _value, SetValue(trans.anchoredPosition, value), tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case StateCtrlRectTransType.SIZE_DELTA:
							DOTween.To(() => trans.sizeDelta, _value => trans.sizeDelta = _value, SetValue(trans.sizeDelta, value), tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
						case StateCtrlRectTransType.PIVOT:
							DOTween.To(() => trans.pivot, _value => trans.pivot = _value, SetValue(trans.pivot, value), tweenDuration)
									.SetTarget(transform).SetDelay(tweenDelay).SetEase(tweenEase);
							break;
					}
				} else {
					switch (type) {
						case StateCtrlRectTransType.ANCHOR_MIN:
							trans.anchorMin = SetValue(trans.anchorMin, value);
							break;
						case StateCtrlRectTransType.ANCHOR_MAX:
							trans.anchorMax = SetValue(trans.anchorMax, value);
							break;
						case StateCtrlRectTransType.ANCHORED_POSITION:
							trans.anchoredPosition = SetValue(trans.anchoredPosition, value);
							break;
						case StateCtrlRectTransType.SIZE_DELTA:
							trans.sizeDelta = SetValue(trans.sizeDelta, value);
							break;
						case StateCtrlRectTransType.PIVOT:
							trans.pivot = SetValue(trans.pivot, value);
							break;
					}
				}
			}
		}
		
		private Vector2 SetValue(Vector2 v3, Vector2 value) {
			if ((part & StateCtrlRectTransPart.X) != 0) {
				v3.x = value.x;
			}
			if ((part & StateCtrlRectTransPart.Y) != 0) {
				v3.y = value.y;
			}
			return v3;
		}
	}
}