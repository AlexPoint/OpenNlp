using System;

namespace Netron.Lithium
{
	/// <summary>
	/// Generic Collectioon of GraphML related data
	/// </summary>
	public class GraphDataCollection : System.Collections.CollectionBase 
	{
    
		#region Properties
		/// <summary>
		/// integer indexer
		/// </summary>
		public virtual object this[int index] 
		{
			get 
			{
				return this.List[index];
			}
			set 
			{
				this.List[index] = value;
			}
		}
            
		#endregion

		#region Methods
		/// <summary>
		/// Adds and object to the collection
		/// </summary>
		/// <param name="o"></param>
		public virtual void Add(object o) 
		{
			this.List.Add(o);
		}
            
		
		/// <summary>
		/// Returns whether the given object is stored in the collection
		/// </summary>
		/// <param name="o">whatever object</param>
		/// <returns>true if contained otherwise false</returns>
		public virtual bool Contains(object o) 
		{
			return this.List.Contains(o);
		}
            
		
		/// <summary>
		/// Removes an element from the collection
		/// </summary>
		/// <param name="o"></param>
		public virtual void Remove(object o) 
		{
			this.List.Remove(o);
		}
		

		/// <summary>
		/// Overrides the default implementation to collect the data of the collection
		/// </summary>
		/// <returns></returns>
		public override string ToString() 
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			// <foreach>
			// This loop mimics a foreach call. See C# programming language, pg 247
			// Here, the enumerator is seale and does not implement IDisposable
			System.Collections.IEnumerator enumerator = this.List.GetEnumerator();
			for (
				; enumerator.MoveNext(); 
				) 
			{
				string s = ((string)(enumerator.Current));
				// foreach body
				sw.Write(s);
			}
			// </foreach>
			return sw.ToString();
		}

		#endregion
	}
}
