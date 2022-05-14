/*
 * @Author: wangyun
 * @CreateTime: 2022-03-31 02:11:28 314
 * @LastEditor: wangyun
 * @EditTime: 2022-05-10 16:57:21 901
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Control {
	[Flags]
	public enum StateCtrlColorPart {
		R = 1 << 0,
		G = 1 << 1,
		B = 1 << 2,
		RGB = R | G | B,
		A = 1 << 3,
	}
	
	public class StateCtrlColor : BaseStateCtrl<Color> {
		public StateCtrlColorPart part = StateCtrlColorPart.RGB;
		public bool tween;
		[HideIf("@!this.tween")]
		public float tweenDelay;
		[HideIf("@!this.tween")]
		public float tweenDuration = 0.3F;
		[HideIf("@!this.tween")]
		public Ease tweenEase = Ease.OutQuad;
		
		protected override Color TargetValue {
			get {
				var graphic = GetComponent<Graphic>();
				if (graphic) {
					return graphic.color;
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					return spriteRenderer.color;
				}
				return Color.white;
			}
			set {
				var graphic = GetComponent<Graphic>();
				if (graphic) {
#if UNITY_EDITOR
					if (tween && Application.isPlaying) {
#else
					if (tween) {
#endif
						graphic.DOColor(SetValue(graphic.color, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
					} else {
						graphic.color = SetValue(graphic.color, value);
					}
				}
				var spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
#if UNITY_EDITOR
					if (tween && Application.isPlaying) {
#else
					if (tween) {
#endif
						spriteRenderer.DOColor(SetValue(spriteRenderer.color, value), tweenDuration).SetDelay(tweenDelay).SetEase(tweenEase);
					} else {
						spriteRenderer.color = SetValue(spriteRenderer.color, value);
					}
				}
			}
		}
		
		private Color SetValue(Color c, Color value) {
			if ((part & StateCtrlColorPart.R) != 0) {
				c.r = value.r;
			}
			if ((part & StateCtrlColorPart.G) != 0) {
				c.g = value.g;
			}
			if ((part & StateCtrlColorPart.B) != 0) {
				c.b = value.b;
			}
			if ((part & StateCtrlColorPart.A) != 0) {
				c.a = value.a;
			}
			return c;
		}
	}
}