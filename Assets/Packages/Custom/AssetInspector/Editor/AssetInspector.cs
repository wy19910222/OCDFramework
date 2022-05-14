using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UObject = UnityEngine.Object;

public abstract class AssetInspector : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		foreach (UObject item in targets) {
			DrawTarget(item);
			GUILayout.Space(10);
		}
	}

	protected abstract void DrawTarget(UObject item);
	
	// private const int MAX_STRING_LENGTH = 16382;	// 旧版Unity显示不下这么多内容，但新版没这个限制了
	protected static void DrawItem(string title, params string[] contents) {
		GUI.enabled = true;
		GUILayout.TextField(title);
		foreach (var content in contents) {
			GUILayout.TextArea(content);
			// int totalLength = content.Length;
			// int length = 0;
			// while (length < totalLength) {
			// 	int currentLength = Mathf.Min(totalLength - length, MAX_STRING_LENGTH);
			// 	GUILayout.TextArea(content.Substring(length, currentLength));
			// 	length += currentLength;
			// }
		}
	}

	protected static string PathListToString(string title, IEnumerable<string> pathList) {
		var sb = new StringBuilder();
		sb.Append(title);
		foreach (var path in pathList) {
			sb.AppendLine();
			sb.Append("- ");
			sb.Append(path);
		}
		return sb.ToString();
	}
}

[CanEditMultipleObjects]
[CustomEditor(typeof(DefaultAsset))]
public class DefaultAssetInspector : AssetInspector {
	private readonly Dictionary<string, string> s_ContentCaches = new Dictionary<string, string>();
	
	protected override void DrawTarget(UObject item) {
		string path = AssetDatabase.GetAssetPath(item);
		if (s_ContentCaches.TryGetValue(path, out string content)) {
			DrawItem(path, content);
		} else {
			if (Directory.Exists(path)) {
				content = GetDirectoryContent(path);
			} else if (File.Exists(path)) {
				if (path.StartsWith("Assets/StreamingAssets/")) {
					if (path.EndsWith(".manifest") || path.EndsWith(".txt")) {
						content = GetTextContent(path);
					} else if (path.EndsWith(".unity3d") || path.EndsWith(".ab") || path.IndexOf(".") == -1) {
						content = GetAssetBundleContent(path);
					}
				}
			}
			s_ContentCaches.Add(path, content);
			DrawItem(path, content);
		}
	}

	private static string GetDirectoryContent(string path) {
		var filePaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
		var filePathList = new List<string>();
		var regex = new Regex("\\+");
		foreach (var filePath in filePaths) {
			if (!filePath.EndsWith(".meta")) {
				filePathList.Add(regex.Replace(filePath, "/"));
			}
		}
		return PathListToString("Files:", filePathList);
	}

	private static string GetTextContent(string path) {
		string text;
		FileInfo file = new FileInfo(path);
		using (FileStream fs = file.OpenRead()) {
			using (MemoryStream ms = new MemoryStream()) {
				var bytesTemp = new byte[4096];
				int readLength;
				while ((readLength = fs.Read(bytesTemp, 0, 4096)) > 0) {
					ms.Write(bytesTemp, 0, readLength);
				}
				ms.Flush();
				text = Encoding.UTF8.GetString(ms.ToArray());
			}
		}
		return text;
	}

	private static string GetAssetBundleContent(string path) {
		var assetBundle = AssetBundle.LoadFromFile(path);
		if (assetBundle) {
			var assetNames = assetBundle.GetAllAssetNames();
			assetBundle.Unload(true);
			return PathListToString("Assets:", assetNames);
		}
		return null;
	}
}

[CanEditMultipleObjects]
[CustomEditor(typeof(SceneAsset))]
public class SceneAssetInspector : AssetInspector {
	private readonly Dictionary<string, string> s_ContentCaches = new Dictionary<string, string>();
	
	protected override void DrawTarget(UObject item) {
		string path = AssetDatabase.GetAssetPath(item);
		if (s_ContentCaches.TryGetValue(path, out string content)) {
			DrawItem(path, content);
		} else {
			var dependencies = AssetDatabase.GetDependencies(new[] {path});
			var dependencyList = new List<string>();
			foreach (var dependency in dependencies) {
				if (dependency != path) {
					dependencyList.Add(dependency);
				}
			}
			dependencyList.Sort();

			content = PathListToString("Dependencies:", dependencyList);
			s_ContentCaches.Add(path, content);
			
			DrawItem(path, content);
		}
	}
}