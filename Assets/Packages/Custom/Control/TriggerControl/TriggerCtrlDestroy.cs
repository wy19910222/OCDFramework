/*
 * @Author: wangyun
 * @CreateTime: 2022-04-22 18:13:29 373
 * @LastEditor: wangyun
 * @EditTime: 2022-05-02 19:39:00 145
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlDestroy : BaseTriggerCtrl {
		public bool onlyDestroyChildren;
		
		protected override void DoTrigger() {
			if (onlyDestroyChildren) {
				Transform trans = transform;
				for (int index = trans.childCount - 1; index >= 0; --index) {
					Transform child = trans.GetChild(index);
#if UNITY_EDITOR
					if (Application.isPlaying) {
						Destroy(child.gameObject);
					} else {
						DestroyImmediate(child.gameObject);
					}
#else
					Destroy(child.gameObject);
#endif
				}
			} else {
#if UNITY_EDITOR
				if (Application.isPlaying) {
					Destroy(gameObject);
				} else {
					DestroyImmediate(gameObject);
				}
#else
				Destroy(gameObject);
#endif
			}
		}
	}
}