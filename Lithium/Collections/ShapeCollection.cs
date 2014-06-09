using System;
using System.Collections;
namespace Netron.Lithium
{
	/// <summary>
	/// STC of shapes
	/// </summary>
	public class ShapeCollection: CollectionBase
	{
		/// <summary>
		/// Occurse when a shape is added to the collection
		/// </summary>
		public event ShapeData OnShapeAdded;
		/// <summary>
		/// Adds a shape to the collection
		/// </summary>
		/// <param name="shape">a ShapeBase object</param>
		/// <returns>the index of the added object in the collection</returns>
		public int Add(ShapeBase shape)
		{	
			int newid = this.InnerList.Add(shape);
			if(OnShapeAdded!=null)
				OnShapeAdded(shape);
			return newid;
		}

		/// <summary>
		/// integer indexer
		/// </summary>
		public ShapeBase this[int index]
		{
			get{return this.InnerList[index] as ShapeBase;}
		}
		/// <summary>
		/// string indexer
		/// Gets the connection (if any) from the collection with the given UID
		/// </summary>
		public ShapeBase this[string uid]
		{
			get{
				for(int k=0;k<InnerList.Count; k++)
					if(this[k].UID.ToString()==uid) return this[k];

				return null;
				}
		}
		/// <summary>
		/// Removes the connection from the collection
		/// </summary>
		/// <param name="shape">a ShapeBase object</param>
		public void Remove(ShapeBase shape)
		{
			this.InnerList.Remove(shape);
		}


	}


}

