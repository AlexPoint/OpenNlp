
using System;
using System.Collections;
using System.Reflection;

namespace Netron.Lithium
{
	/// <summary>
	/// Attribute class for designating which properties will be serialized
	/// by the GraphML serializer.
	/// </summary>
	/// 
	[AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
	public class GraphDataAttribute : System.Attribute
	{
		#region Delegates
		private delegate void ToStringDelegate();
		private delegate void FromStringDelegate();
		#endregion
		
		/// <summary>
		/// Returns the tagged properties of the gibven object
		/// </summary>
		/// <param name="value">whatever class</param>
		/// <returns>a hashtable of property names with their values</returns>
		public static Hashtable GetValuesOfTaggedFields(object value) 
		{
			Hashtable vs = new Hashtable();

			foreach (PropertyInfo pi in value.GetType().GetProperties()) 
			{	
				if (Attribute.IsDefined(pi, typeof(GraphDataAttribute))) 
				{
					vs.Add(pi.Name,pi.GetValue(value,null));
				}
			}

			return vs;
		}
	}
}
