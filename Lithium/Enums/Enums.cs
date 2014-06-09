using System;

namespace Netron.Lithium
{
	/// <summary>
	/// The shape types available in this assembly
	/// </summary>
	public enum ShapeTypes
	{
		/// <summary>
		/// the default rectangular shape
		/// </summary>
		Rectangular,
		/// <summary>
		/// an oval shape
		/// </summary>
		Oval,
		/// <summary>
		/// a text label
		/// </summary>
		TextLabel
	}

	/// <summary>
	/// The direction in which the tree layout spreads the diagram
	/// </summary>
	public enum TreeDirection
	{
		/// <summary>
		/// the layout orders the shapes along the vertical line
		/// </summary>
		Vertical,
		/// <summary>
		/// the layout orders the shapes along an horizontal line
		/// </summary>
		Horizontal
	}

	/// <summary>
	/// The types of connections in this assembly
	/// </summary>
	public enum ConnectionType
	{
		/// <summary>
		/// The default connection simply connects the centers of the shapes
		/// </summary>
		Default,
		/// <summary>
		/// the traditional connection is a rectangular connections which mimics the traditional
		/// layout of hierarchies and flowcharts
		/// </summary>
		Traditional,
		/// <summary>
		/// a smoothly curved form connecting the shapes
		/// </summary>
		Bezier
	}
}
