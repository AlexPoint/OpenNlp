using System;

namespace Netron.Lithium
{
	/// <summary>
	/// the info coming with the show-props event
	/// </summary>
	public delegate void ShowProps(object ent);
	/// <summary>
	/// to pass shape data to the outside world
	/// </summary>
	public delegate void ShapeData(ShapeBase shape);

	/// <summary>
	/// General purpose delegate to pass info to the outside world
	/// </summary>
	public delegate void Messager(string message);
}
