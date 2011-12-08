using System;
using MonoTouch.UIKit;
using System.Linq;
using System.Collections.Generic;

namespace BunknotesApp
{
	public static class UIViewExtenstions
	{
		public static T GetSubviewElement<T>(this UIView view) where T:class{
			var found = view.Subviews.FirstOrDefault(x=> x.GetType() == typeof(T));
			return found as T;
		}
		
		public static List<T> GetSubviewElements<T>(this UIView view) where T:class{
			var result = new List<T>();
			foreach (var el in view.Subviews) {
				if (el.GetType() == typeof(T)) {
					result.Add(el as T);
				}
			}
			return result;
		}
	}
}

