/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using UnityEngine;

namespace StateControl {

	[Flags]
	public enum StateCtrlMeshColorPart {
		R = 1 << 0,
		G = 1 << 1,
		B = 1 << 2,
		RGB = R | G | B,
		A = 1 << 3,
	}
	
	[RequireComponent(typeof(MeshFilter))]
	public class StateCtrlMeshColor : BaseStateCtrl<Color> {
		public StateCtrlMeshColorPart part = StateCtrlMeshColorPart.RGB;
		public Color color = Color.white;
		
		protected override Color TargetValue {
			get => color;
			set {
				if (value == color) return;
				color = value;
				
				var mf = GetComponent<MeshFilter>();
				if (!mf) return;
				
				var mesh = mf.sharedMesh;
				if (!mesh) return;

				var _colors = mesh.colors;
				var _color = _colors.Length > 0 ? _colors[0] : Color.white;
				if ((part & StateCtrlMeshColorPart.R) != 0) {
					_color.r = color.r;
				}
				if ((part & StateCtrlMeshColorPart.G) != 0) {
					_color.g = color.g;
				}
				if ((part & StateCtrlMeshColorPart.B) != 0) {
					_color.b = color.b;
				}
				if ((part & StateCtrlMeshColorPart.A) != 0) {
					_color.a = color.a;
				}
				if (_color == Color.white) {
					mesh.SetColors(Array.Empty<Color>());
				} else {
					if (Application.isPlaying) {
						mesh = mf.mesh;
					}
					var length = mesh.vertexCount;
					var colors = new Color[length];
					for (var i = 0; i < length; ++i) {
						colors[i] = _color;
					}
					mesh.SetColors(colors);
				}
			}
		}
	}
}