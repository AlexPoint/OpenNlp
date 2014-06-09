using System;
using System.Drawing;
namespace Netron.Lithium
{
	/// <summary>
	/// A simple text label
	/// </summary>
	public class TextLabel : ShapeBase
	{
		#region Fields
		
		#endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="s"></param>
		public TextLabel(LithiumControl s) : base(s)
		{
			this.shapeColor = Color.Transparent;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Tests whether the mouse hits this shape
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(System.Drawing.Point p)
		{
			Rectangle r= new Rectangle(p, new Size(5,5));
			return rectangle.Contains(r);			
		}



		/// <summary>
		/// Paints the shape on the canvas
		/// </summary>
		/// <param name="g"></param>
		public override void Paint(System.Drawing.Graphics g)
		{
			g.FillRectangle(shapeBrush,rectangle);
			if(hovered || isSelected)
				g.DrawRectangle(new Pen(Color.Red,2F),rectangle);
			
			
			//well, a lot should be said here like
			//the fact that one should measure the text before drawing it,
			//resize the width and height if the text if bigger than the rectangle,
			//alignment can be set and changes the drawing as well...
			//here we keep it really simple:
			if(text !=string.Empty)
				g.DrawString(text,font,Brushes.Black, rectangle.X+10,rectangle.Y+10);
		}

		/// <summary>
		/// Invalidates the shape
		/// </summary>
		public override void Invalidate()
		{
			Rectangle r = rectangle;
			r.Offset(-5,-5);
			r.Inflate(20,20);
			site.Invalidate(r);
		}

		/// <summary>
		/// Overrides the resize to repaint the shape
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public override void Resize(int width, int height)
		{
			base.Resize (width, height);
			Invalidate();
		}

		#endregion	
	}
}
