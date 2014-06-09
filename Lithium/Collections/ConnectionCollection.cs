using System;
using System.Collections;
namespace Netron.Lithium
{
	/// <summary>
	/// STC of connections
	/// 
	/// </summary>
	public class ConnectionCollection: CollectionBase
	{
		

		/// <summary>
		/// Adds a connection to the collection
		/// </summary>
		/// <param name="con">a connection</param>
		/// <returns>the index of the added element in the collection</returns>
		public int Add(Connection con)
		{
			return this.InnerList.Add(con);
		}

		/// <summary>
		/// integer indexer; gets the connection stored in the collection in the given position
		/// </summary>
		public Connection this[int index]
		{
			get{return this.InnerList[index] as Connection;}
		}
		/// <summary>
		/// Removes a connection from the collection
		/// </summary>
		/// <param name="con">a connection object</param>
		public void Remove(Connection con)
		{
			this.InnerList.Remove(con);
		}
		/// <summary>
		/// Removes a connection from the collection
		/// </summary>
		/// <param name="one">the 'from' or 'to' (ShapeBase) part of the connection</param>
		/// <param name="two">the complementary 'from' or 'to' (ShapeBase) part of the connection</param>
		public void Remove(ShapeBase one, ShapeBase two)
		{
			for(int k=0; k<InnerList.Count; k++)
			{
				if((this[k].From ==one && this[k].To==two) || (this[k].From ==two && this[k].To==one) )
				{
						this.InnerList.RemoveAt(k);
					break;
				}
			}
		}
	}


}
