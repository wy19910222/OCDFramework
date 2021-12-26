/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace StateControl {
	[CustomPropertyDrawer(typeof(StateControllerSelectAttribute))]
	public class StateControllerSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = property.serializedObject.targetObject as Component;

			List<StateController> controllers = new List<StateController>(target?.GetComponentsInParent<StateController>(true));
			List<string> titles = controllers.ConvertAll(controller => string.IsNullOrEmpty(controller.title) ? "匿名" : controller.title);
			controllers.Insert(0, null);
			titles.Insert(0, "无");

			StateController controller = property.objectReferenceValue as StateController;
			int index = controllers.IndexOf(controller);

			int newIndex = EditorGUI.Popup(position, label.text, index, titles.ToArray());
			if (newIndex != index) {
				property.objectReferenceValue = controllers[newIndex];
			}
		}
	}

	[CustomPropertyDrawer(typeof(SelfComponentSelectAttribute))]
	public class SelfComponentSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = property.serializedObject.targetObject as Component;
			if (!target) {
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			FieldInfo field = target.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Type fieldType = field.FieldType;
			List<Component> comps = new List<Component>(target.GetComponents(fieldType));
			List<string> names = comps.ConvertAll(comp => comp.GetType().Name);
			comps.Insert(0, null);
			names.Insert(0, "无");

			Component comp = property.objectReferenceValue as Component;
			int index = comps.IndexOf(comp);
			int newIndex = EditorGUI.Popup(position, label.text, index, names.ToArray());
			if (newIndex != index) {
				property.objectReferenceValue = comps[newIndex];
			}
		}
	}
}