/*
 * @Author: wangyun
 * @CreateTime: 2022-04-26 12:17:14 806
 * @LastEditor: wangyun
 * @EditTime: 2022-04-26 12:17:14 801
 */

using UnityEngine;

namespace Control {
	public class TriggerCtrlAudioOneShot : BaseTriggerCtrl {
		private static AudioSource s_Source;
		private static AudioSource Source {
			get {
#if UNITY_EDITOR
				if (!s_Source && Application.isPlaying) {
#else
				if (!s_Source) {
#endif
					s_Source = new GameObject("[" + nameof(TriggerCtrlAudioOneShot) + "]").AddComponent<AudioSource>();
				}
				return s_Source;
			}
		}
		
		public AudioClip clip;
		public float volumeScale = 1;

		protected override void DoTrigger() {
			AudioSource source = Source;
			if (source) {
				source.PlayOneShot(clip, volumeScale);
			}
		}
	}
}