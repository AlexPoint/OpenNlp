using System;

namespace Netron.Lithium
{
	/// <summary>
	/// Utility class to speed up the deserialization of connections
	/// </summary>
	public struct ParentChild
	{
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="child"></param>
		/// <param name="parent"></param>
		public ParentChild( ShapeBase child, string parent)
		{
			this.Parent = parent;
			this.ChildShape = child;
			//this.ParentShape = null;
			//this.ChildShape = null;
		}
		/// <summary>
		/// Gets or sets the parent in this relation
		/// </summary>
		public string Parent;		
		/// <summary>
		/// Gets or sets the child in this relation
		/// </summary>
		public ShapeBase ChildShape;
	}
}
