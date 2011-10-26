using System;
using Bunk1DataAnnotations;
using System.Reflection;
using System.Linq;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace BunknotesApp
{
	public class Validator:IValidator
	{
		public Validator ()
		{
		}
		
		public bool Validate (object caller)
		{
			var inf = caller.GetType ();
			foreach (var field in inf.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) {
				var attributes = field.GetCustomAttributes (typeof(ValidationAttribute), true);
				var propValue = field.GetValue(caller);
				foreach (var attribute in attributes) {
					var attributeType = attribute.GetType ();
					var methodInfo = attributeType.GetMethod ("Validate");
					var instance = Activator.CreateInstance (attributeType) as ValidationAttribute;
					instance.ErrorMessage = ((ValidationAttribute)attribute).ErrorMessage;
					var validationResult = (bool)methodInfo.Invoke (instance, new object[] { propValue });
					if (!validationResult) return false;
				}
			} 
			return true;
		}
	}
}

