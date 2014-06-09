using System;

namespace Netron.Lithium
{/// <summary>
	/// Interface of a visitor
	/// </summary>
	public interface IVisitor
	{
		/// <summary>
		/// The actual action to perform on visited objects
		/// </summary>
		/// <param name="shape"></param>
		void Visit(ShapeBase shape);
		/// <summary>
		/// Whether the visiting process is done
		/// </summary>
		bool IsDone { get; }
	}
	/// <summary>
	/// Interface of a prepost visitor which allows you to have
	/// an action before, during and after a visit
	/// </summary>
	public interface IPrePostVisitor : IVisitor
	{
		/// <summary>
		/// action before the visit
		/// </summary>
		/// <param name="shape"></param>
		void PreVisit(ShapeBase shape);		
		/// <summary>
		/// the action after the visit
		/// </summary>
		/// <param name="shape"></param>
		void PostVisit(ShapeBase shape);
		
	}
}
