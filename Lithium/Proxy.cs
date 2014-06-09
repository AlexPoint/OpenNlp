using System;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Collections;
namespace Netron.Lithium
{
	/// <summary>
	/// Simple proxy class for the control to display only specific properties.
	/// Not as sophisticated as the property-bag of the full Netron-control
	/// but does the job in this simple context.
	/// </summary>
	public class Proxy
	{
		#region Fields
		private LithiumControl site;
		#endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="site"></param>
		public Proxy(LithiumControl site)
		{this.site = site;}
		#endregion

		#region Methods
		/// <summary>
		/// Gets or sets the reference to the control instance
		/// </summary>
		[Browsable(false)]
		public LithiumControl Site
		{
			get{return site;}
			set{site = value;}
		}
		/// <summary>
		/// Gets or sets the backcolor 
		/// </summary>
		[Browsable(true), Description("The backcolor of the canvas"), Category("Layout")]
		public Color BackColor
		{
			get{return this.site.BackColor;}
			set{this.site.BackColor = value;}
		}

		/// <summary>
		/// Gets or set the branch height
		/// </summary>
		[Browsable(true), Description("The height between the tree branches")]
		public int BranchHeight
		{
			get{return site.BranchHeight;}
			set{site.BranchHeight = value;}
		}
		/// <summary>
		/// Gets or sets the spacing between the nodes
		/// </summary>
		[Browsable(true)]
		public int WordSpacing
		{
			get{return site.WordSpacing;}
			set
			{
				site.WordSpacing = value;				
			}
		}


	

		/// <summary>
		/// Gets or sets the description of the diagram
		/// </summary>
		[Browsable(true), Description("The description of the diagram."), Category("Diagram")]
		public string Description
		{
			get{return this.site.graphAbstract.Description;}
			set{this.site.graphAbstract.Description = value;}
		}

		/// <summary>
		/// Gets or sets the global connection type
		/// </summary>
		[Browsable(true), Description("The type of connection."), Category("Diagram")]
		public ConnectionType ConnectionType
		{
			get{return site.ConnectionType;}
			set
			{
				site.ConnectionType = value;				
			}
		}
		#endregion
	}
}
