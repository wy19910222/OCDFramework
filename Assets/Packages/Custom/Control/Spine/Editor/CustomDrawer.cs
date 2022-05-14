/*
 * @Author: wangyun
 * @CreateTime: 2022-04-28 00:41:42 075
 * @LastEditor: wangyun
 * @EditTime: 2022-04-28 00:41:42 079
 */

using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

namespace Control {
	[CustomPropertyDrawer(typeof(SelfSkeletonAnimationNameSelectAttribute))]
	public class SelfSkeletonAnimationNameSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = property.serializedObject.targetObject as Component;
			if (!target) {
				EditorGUI.PropertyField(position, property, label);
				return;
			}
			
			SkeletonAnimation anim = target.GetComponent<SkeletonAnimation>();
			List<string> names = new List<Spine.Animation>(anim.skeleton.Data.Animations).ConvertAll(animation => animation.Name);
			List<string> options = new List<string>(names);
			names.Insert(0, "");
			options.Insert(0, "无");
			int index = names.IndexOf(property.stringValue);
			int newIndex = EditorGUI.Popup(position, label.text, index, options.ToArray());
			if (newIndex != index) {
				property.stringValue = names[newIndex];
			}
		}
	}
}