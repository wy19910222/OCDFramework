/*
 * @Author: wangyun
 * @CreateTime: 2022-07-01 12:49:02 083
 * @LastEditor: wangyun
 * @EditTime: 2022-07-01 12:49:02 076
 */

using UnityEditor;
using UnityEngine;

public class EditorMenus : Editor {
	[MenuItem("GameObject/批处理（要怎么处理自己临时改代码）", priority = -1)]
	protected static void BatchOperation(MenuCommand command) {
		GameObject go = command.context as GameObject;
		Debug.Log(go, go);
		// TODO: 对单个对象处理
	}
}
