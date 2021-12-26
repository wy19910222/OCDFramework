/*
 * @Author: wangyun
 * @LastEditors: wangyun
 * @Date: 2021-11-04 16:21:41 627
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {
	#region Join 将所有元素挨个拼接成字符串
	public static string Join(this IEnumerable array, string separator = ", ") {
		int count = 0;
		foreach (var _ in array) {
			++count;
		}
		StringBuilder sb = new StringBuilder();
		int index = 0;
		foreach (var element in array) {
			sb.Append(element ?? "Null");
			++index;
			if (index < count && separator != null) {
				sb.Append(separator);
			}
		}
		return sb.ToString();
	}
	#endregion

	#region Filter 过滤元素，返回满足条件的子集
	public static List<T> Filter<T>(this IEnumerable<T> array, Func<T, bool> match) {
		List<T> list = new List<T>();
		foreach (var element in array) {
			if (match == null || match(element)) {
				list.Add(element);
			}
		}
		return list;
	}
	public static List<T> Filter<T>(this IEnumerable<T> array, Func<T, int, bool> match) {
		List<T> list = new List<T>();
		int i = 0;
		foreach (var element in array) {
			if (match == null || match(element, i)) {
				list.Add(element);
			}
			++i;
		}
		return list;
	}
	#endregion

	#region Count 统计满足条件的元素数量
	public static int Count<T>(this IEnumerable<T> array, Func<T, bool> match) {
		int count = 0;
		foreach (var element in array) {
			if (match == null || match(element)) {
				++count;
			}
		}
		return count;
	}
	public static int Count<T>(this IEnumerable<T> array, Func<T, int, bool> match) {
		int count = 0;
		int i = 0;
		foreach (var element in array) {
			if (match == null || match(element, i)) {
				++count;
			}
			++i;
		}
		return count;
	}
	#endregion
}
