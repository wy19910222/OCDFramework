/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-05-10 21:50:47 060
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Control {
	[CustomPropertyDrawer(typeof(StateControllerSelectAttribute))]
	public class StateControllerSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = (Component) property.serializedObject.targetObject;
			List<StateController> controllers = new List<StateController>(target.GetComponentsInParent<StateController>(true));
			List<string> titles = controllers.ConvertAll(_controller => string.IsNullOrEmpty(_controller.title) ? "匿名" : _controller.title);
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
	
	[CustomPropertyDrawer(typeof(ProgressControllerSelectAttribute))]
	public class ProgressControllerSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = (Component) property.serializedObject.targetObject;
			List<ProgressController> controllers = new List<ProgressController>(target.GetComponentsInParent<ProgressController>(true));
			List<string> titles = controllers.ConvertAll(_controller => string.IsNullOrEmpty(_controller.title) ? "匿名" : _controller.title);
			controllers.Insert(0, null);
			titles.Insert(0, "无");

			ProgressController controller = property.objectReferenceValue as ProgressController;
			int index = controllers.IndexOf(controller);

			int newIndex = EditorGUI.Popup(position, label.text, index, titles.ToArray());
			if (newIndex != index) {
				property.objectReferenceValue = controllers[newIndex];
			}
		}
	}

	[CustomPropertyDrawer(typeof(SelfAnimatorParamSelectAttribute))]
	public class SelfAnimatorParamSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = (Component) property.serializedObject.targetObject;
			if (target.gameObject.activeInHierarchy) {
				Animator anim = target.GetComponent<Animator>();
				List<AnimatorControllerParameter> parameters = anim.runtimeAnimatorController ?
						new List<AnimatorControllerParameter>(anim.parameters) : new List<AnimatorControllerParameter>();
				AnimatorControllerParameterType type = ((SelfAnimatorParamSelectAttribute) attribute).Type;
				if (type > 0) {
					parameters = parameters.FindAll(parameter => parameter.type == type);
				}
				if (parameters.Count > 0) {
					List<string> paramNames = parameters.ConvertAll(parameter => parameter.name);
					int index = paramNames.IndexOf(property.stringValue);
					int newIndex = EditorGUI.Popup(position, label.text, index, paramNames.ToArray());
					if (newIndex != index) {
						property.stringValue = paramNames[newIndex];
					}
					return;
				}
			}
			bool prevEnabled = GUI.enabled;
			GUI.enabled = false;
			EditorGUI.Popup(position, label.text, 0, new []{property.stringValue});
			GUI.enabled = prevEnabled;
		}
	}

	[CustomPropertyDrawer(typeof(ComponentSelectAttribute))]
	public class ComponentSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Type type = fieldInfo.FieldType;
			if (!typeof(Component).IsAssignableFrom(type)) {
				if (type.IsArray) {
					type = type.GetElementType();
				} else if (type.IsGenericType) {
					type = type.GetGenericArguments()[0];
				} else {
					type = typeof(Component);
				}
			}
			Component comp = property.objectReferenceValue as Component;
			if (comp) {
				Rect leftPosition = position;
				leftPosition.width = (position.width + EditorGUIUtility.labelWidth) * 0.5F;
				Rect rightPosition = leftPosition;
				rightPosition.x += rightPosition.width;
				rightPosition.width = position.width - leftPosition.width;
			
				EditorGUI.PropertyField(leftPosition, property, label);
				
				List<Component> comps = new List<Component>(comp.GetComponents(type));
				if (((ComponentSelectAttribute) attribute).ExceptSelf) {
					Component self = property.serializedObject.targetObject as Component;
					comps.Remove(self);
					if (property.objectReferenceValue == self) {
						property.objectReferenceValue = null;
					}
				}
				int compCount = comps.Count;
				string[] compNames = new string[compCount + 1];
				int[] compIndexes = new int[compCount + 1];
				compNames[0] = "0.None";
				compIndexes[0] = -1;
				for (int index = 0; index < compCount; ++index) {
					compNames[index + 1] = index + 1 + "." + comps[index].GetType().Name;
					compIndexes[index + 1] = index;
				}
				int currentIndex = comps.IndexOf(comp);
				int dataIndex = EditorGUI.IntPopup(rightPosition, currentIndex, compNames, compIndexes);
				if (dataIndex != currentIndex) {
					property.objectReferenceValue = dataIndex == -1 ? null : comps[dataIndex];
				}
			} else {
				const float buttonWidth = 60F;
				Rect leftPosition = position;
				leftPosition.width = position.width - buttonWidth;
				Rect rightPosition = leftPosition;
				rightPosition.x += rightPosition.width;
				rightPosition.width = buttonWidth;
			
				EditorGUI.PropertyField(leftPosition, property, label);
				
				Component target = (Component) property.serializedObject.targetObject;
				List<Component> comps = new List<Component>(target.GetComponents(type));
				if (((ComponentSelectAttribute) attribute).ExceptSelf) {
					comps.Remove(property.serializedObject.targetObject as Component);
				}
				bool prevEnabled = GUI.enabled;
				GUI.enabled = prevEnabled && comps.Count > 0;
				if (GUI.Button(rightPosition, new GUIContent("ThisGo"))) {
					property.objectReferenceValue = comps[0];
				}
				GUI.enabled = prevEnabled;
			}
		}
	}

	[CustomPropertyDrawer(typeof(CanResetCurveAttribute))]
	public class CanResetCurveDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			const float buttonWidth = 60F;
			Rect leftPosition = position;
			leftPosition.width = position.width - buttonWidth;
			Rect rightPosition = leftPosition;
			rightPosition.x += rightPosition.width;
			rightPosition.width = buttonWidth;
			
			EditorGUI.PropertyField(leftPosition, property, label);
			if (GUI.Button(rightPosition, new GUIContent("Reset"))) {
				AnimationCurve curve = property.animationCurveValue;
				Keyframe[] curveKeys = curve.keys;
				if (curveKeys.Length != 1 || curveKeys[0].time != 0 || curveKeys[0].value != 0) {
					property.animationCurveValue = new AnimationCurve(new Keyframe(0, 0));
				}
			}
		}
	}

	public static class CustomDrawer {
		public static T DrawComponentSelector<T>(T t, Converter<T, string> nameConvert = null, Action beforeChange = null) where T : Component {
			T newT = EditorGUILayout.ObjectField(t, typeof(T), true) as T;
			if (newT != t) {
				beforeChange?.Invoke();
				t = newT;
			}
			if (t) {
				List<T> ts = new List<T>(t.GetComponents<T>());
				List<string> names = ts.ConvertAll(nameConvert ?? (_t => _t.GetType().Name));
				ts.Insert(0, null);
				names.Insert(0, "无");
				for (int i = 0, length = names.Count; i < length; ++i) {
					names[i] = i + ":" + names[i];
				}
				int index = ts.IndexOf(t);
				int newIndex = EditorGUILayout.Popup(index, names.ToArray());
				if (newIndex != index) {
					beforeChange?.Invoke();
					t = ts[newIndex];
				}
			}
			return t;
		}
	}
}