/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using UnityEngine;

namespace StateControl {

	public enum StateCtrlTransType {
		NONE,
		POSITION = 1,
		ANGLE = 2,
		SCALE = 3
	}

	[Flags]
	public enum StateCtrlTransPart {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2
	}

	public class StateCtrlTrans : BaseStateCtrl<Vector3> {

		public StateCtrlTransType type;
		public StateCtrlTransPart part = StateCtrlTransPart.X | StateCtrlTransPart.Y | StateCtrlTransPart.Z;

		protected override Vector3 TargetValue {
			get {
				var trans = transform;
				return type switch {
					StateCtrlTransType.POSITION => trans.localPosition,
					StateCtrlTransType.ANGLE => trans.eulerAngles,
					StateCtrlTransType.SCALE => trans.localScale,
					_ => Vector3.zero
				};
			}
			set {
				switch (type) {
					case StateCtrlTransType.POSITION: {
						var position = transform.localPosition;
						if ((part & StateCtrlTransPart.X) != 0) {
							position.x = value.x;
						}
						if ((part & StateCtrlTransPart.Y) != 0) {
							position.y = value.y;
						}
						if ((part & StateCtrlTransPart.Z) != 0) {
							position.z = value.z;
						}
						transform.localPosition = position;
						break;
					}
					case StateCtrlTransType.ANGLE: {
						var angles = transform.localEulerAngles;
						if ((part & StateCtrlTransPart.X) != 0) {
							angles.x = value.x;
						}
						if ((part & StateCtrlTransPart.Y) != 0) {
							angles.y = value.y;
						}
						if ((part & StateCtrlTransPart.Z) != 0) {
							angles.z = value.z;
						}
						transform.localEulerAngles = angles;
						break;
					}
					case StateCtrlTransType.SCALE: {
						var scale = transform.localScale;
						if ((part & StateCtrlTransPart.X) != 0) {
							scale.x = value.x;
						}
						if ((part & StateCtrlTransPart.Y) != 0) {
							scale.y = value.y;
						}
						if ((part & StateCtrlTransPart.Z) != 0) {
							scale.z = value.z;
						}
						transform.localScale = scale;
						break;
					}
				}
			}
		}
	}
}