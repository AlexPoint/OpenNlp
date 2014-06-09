using System;

namespace Netron.Lithium
{
	/// <summary>
	/// Expands each node on visit
	/// </summary>
	public class ExpanderVisitor : IVisitor
	{
		/// <summary>
		/// Visits the shape
		/// </summary>
		/// <param name="sh"></param>
		public void Visit(ShapeBase sh)
		{
			sh.Expand();
		}

		/// <summary>
		/// Gets whether the visiting is done
		/// </summary>
		public bool IsDone
		{
			get
			{				
				return false;
			}
		}


	}
}
