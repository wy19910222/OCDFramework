/*
 * @Author: wangyun
 * @CreateTime: 2022-04-20 14:45:01 688
 * @LastEditor: wangyun
 * @EditTime: 2022-04-20 14:45:01 683
 */

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector;

namespace Control {
	public enum TriggerCtrlDOTweenType {
		PLAY_FORWARD = 0,
		PLAY_BACKWARDS = 1,
		REWIND = 2
	}
	
	public class TriggerCtrlDOTween : BaseTriggerCtrl {
		public TriggerCtrlDOTweenType type = TriggerCtrlDOTweenType.PLAY_FORWARD;
		[HideIf("@type==TriggerCtrlDOTweenType.PLAY_BACKWARDS")]
		public bool includeDelay = true;
		[ShowIf("@type==TriggerCtrlDOTweenType.PLAY_FORWARD"), Tooltip("给DOTweenPath用的")]
		public bool fromHere;	// DOTweenPath用的
		[ComponentSelect]
		public List<ABSAnimationComponent> tweenAnims = new List<ABSAnimationComponent>();

		private void Reset() {
			tweenAnims.Clear();
			tweenAnims.AddRange(gameObject.GetComponents<ABSAnimationComponent>());
		}
		
		protected override void DoTrigger() {
			foreach (var tweenAnim in tweenAnims) {
				switch (type) {
					case TriggerCtrlDOTweenType.PLAY_FORWARD:
						if (fromHere && tweenAnim is DOTweenPath tweenPath) {
							tweenPath.DORestart(true);
							// DORestart可以fromHere，但是默认includeDelay，所以如果不includeDelay就再调一次Restart
							if (!includeDelay) {
								tweenAnim.tween?.Restart(false);
							}
						} else {
							tweenAnim.tween?.Restart(includeDelay);
						}
						break;
					case TriggerCtrlDOTweenType.PLAY_BACKWARDS:
						tweenAnim.tween?.PlayBackwards();
						break;
					case TriggerCtrlDOTweenType.REWIND:
						tweenAnim.tween?.Rewind(includeDelay);
						break;
				}
			}
		}
	}
}