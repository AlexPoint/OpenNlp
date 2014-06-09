using System;
using System.Collections;

namespace Netron.Lithium
{
	/// <summary>
	/// STC of ParentChild collection
	/// </summary>
	public class ParentChildCollection : CollectionBase
	{		
		/// <summary>
		/// integer indexer
		/// </summary>
		public ParentChild this[int index]
		{
			get{return (ParentChild) this.InnerList[index] ;}
		}

		/// <summary>
		/// Adds an item to the collection
		/// </summary>
		/// <param name="pc">a ParentChild object</param>
		/// <returns></returns>
		public int Add(ParentChild pc)
		{
			return this.InnerList.Add(pc);
		}
	}
}
