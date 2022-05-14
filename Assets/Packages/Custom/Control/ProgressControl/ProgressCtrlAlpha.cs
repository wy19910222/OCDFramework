/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:35 694
 * @LastEditor: wangyun
 * @EditTime: 2022-05-10 16:46:10 880
 */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	public class ProgressCtrlAlpha : BaseProgressCtrlFloat {
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		
		protected override float TargetValue {
			get {
				var group = GetComponent<CanvasGroup>();
				if (group) {
					return group.alpha;
				}
				var graphic = GetComponent<Graphic>();
				if (graphic) {
					return graphic.color.a;
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					return spriteRenderer.color.a;
				}
				return 0;
			}
			set {
				var group = GetComponent<CanvasGroup>();
				if (group) {
#if UNITY_EDITOR
					if (tween && Application.isPlaying) {
#else
					if (tween) {
#endif
						group.DOFade(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
					} else {
						group.alpha = value;
					}
					return;
				}
				var graphic = GetComponent<Graphic>();
				if (graphic) {
#if UNITY_EDITOR
					if (tween && Application.isPlaying) {
#else
					if (tween) {
#endif
						graphic.DOFade(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
					} else {
						var _color = graphic.color;
						_color.a = value;
						graphic.color = _color;
					}
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
#if UNITY_EDITOR
					if (tween && Application.isPlaying) {
#else
					if (tween) {
#endif
						spriteRenderer.DOFade(value, tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
					} else {
						var _color = spriteRenderer.color;
						_color.a = value;
						spriteRenderer.color = _color;
					}
				}
			}
		}
	}
}