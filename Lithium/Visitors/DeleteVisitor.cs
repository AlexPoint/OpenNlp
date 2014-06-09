using System;

namespace Netron.Lithium
{
	/// <summary>
	///	 Deletion of shapes is implemented via a visitor pattern to handle the fact
	///	 that child nodes have to be deleted before its parent 
	/// </summary>
	public class DeleteVisitor: IVisitor
	{

		#region Fields
		/// <summary>
		/// Occurs when a shape is deleted
		/// </summary>
		public event ShapeData OnDelete;

		/// <summary>
		/// the reference to the abstract
		/// </summary>
		GraphAbstract graphAbstract;

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="graphAbstract">a GraphAbstract instance</param>
		public DeleteVisitor(GraphAbstract graphAbstract)
		{
			this.graphAbstract = graphAbstract;
		}
		
		#endregion

		/// <summary>
		/// Visits the shape
		/// </summary>
		/// <param name="sh"></param>
		public void Visit(ShapeBase sh)
		{
			//remove from the shape from the Shapes
			if(sh==null) return;
			//strictly speaking we should remove the shape from its parent's childNodes collection first
			graphAbstract.Connections.Remove(sh.connection);
			graphAbstract.Shapes.Remove(sh);
			if(OnDelete!=null)
				OnDelete(sh);
		}

		/// <summary>
		/// Gets whether the visiting is done
		/// </summary>
		public bool IsDone
		{
			get
			{
				// TODO:  Add DeleteVisitor.IsDone getter implementation
				return false;
			}
		}


	}
}
