using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Lithium
{
	/// <summary>
	/// A simple rectangular shape
	/// </summary>
	public class SimpleRectangle : ShapeBase
	{
		#region Fields
		string plus = "";

		
		int bshift = 10;				
		Region region, shadow;
		GraphicsPath path;
		
		
				
		#endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="s"></param>
		public SimpleRectangle(LithiumControl s) : base(s)
		{
			
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
			Rectangle r= new Rectangle(p, new Size(2,2));
			return rectangle.Contains(r);			
		}


		/// <summary>
		/// Paints the shape on the canvas
		/// </summary>
		/// <param name="g"></param>
		/// 

		public override void Paint(System.Drawing.Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle toggleNode = Rectangle.Empty; //the [+] [-]
			Point[] pts = new Point[12]
				{
					new Point(rectangle.X,rectangle.Y), //0
					new Point(rectangle.X+bshift,rectangle.Y), //1
					new Point(rectangle.Right-bshift,rectangle.Y), //2
					new Point(rectangle.Right,rectangle.Y), //3
					new Point(rectangle.Right,rectangle.Y+bshift), //4
					new Point(rectangle.Right,rectangle.Bottom-bshift), //5
					new Point(rectangle.Right,rectangle.Bottom), //6
					new Point(rectangle.Right-bshift,rectangle.Bottom), //7
					new Point(rectangle.X+bshift,rectangle.Bottom), //8
					new Point(rectangle.X,rectangle.Bottom), //9
					new Point(rectangle.X,rectangle.Bottom - bshift), //10
					new Point(rectangle.X,rectangle.Y+bshift), //11				
			};
			path = new GraphicsPath();
			path.AddBezier(pts[11],pts[0],pts[0],pts[1]);
			path.AddLine(pts[1],pts[2]);
			path.AddBezier(pts[2],pts[3],pts[3],pts[4]);
			path.AddLine(pts[4],pts[5]);
			path.AddBezier(pts[5],pts[6],pts[6],pts[7]);
			path.AddLine(pts[7],pts[8]);
			path.AddBezier(pts[8],pts[9],pts[9],pts[10]);
			path.AddLine(pts[10],pts[11]);
			path.CloseFigure();
			region = new Region(path);

			shapeBrush = new LinearGradientBrush(rectangle,this.shapeColor,Color.WhiteSmoke,0f);
			
			// start Draw Shadow
			shadow = region.Clone();
			shadow.Translate(5, 5);
			

			//add the amount of children
			if (childNodes.Count > 0)
			{
				plus = " [" + childNodes.Count + "]";				
			}
			else
			{
				plus = "";
			}
			
			g.FillRegion(new SolidBrush(Color.Gainsboro), shadow);
			//g.DrawPath(new Pen(Color.Gainsboro,1), shadow);
			//End Draw Shadow

			g.FillRegion(shapeBrush, region);
			
			
            
			if (hovered || isSelected)			
				pen = thickPen;				
			else			
				pen = blackPen;				
			
			g.DrawPath(pen, path);
		
			if (text != string.Empty)
			{
				//g.DrawString(text + plus,font,Brushes.Black, rectangle.X+5,rectangle.Y+5);
				g.DrawString(text,font,Brushes.Black, rectangle.X+5,rectangle.Y+5);
			}

		

			//draw the [+] expansion shape
			if (childNodes.Count > 0)
			{
				switch(site.LayoutDirection)
				{
				
					case TreeDirection.Vertical:
						toggleNode = new Rectangle(Left + this.Width/2 - 5, Bottom, 10, 10);
						break;
					case TreeDirection.Horizontal:
						toggleNode = new Rectangle(Right , Top +Height/2-5 , 10, 10);
						break;
				}
				
                
				//Draw [ ]
				g.FillRectangle(new SolidBrush(Color.White), toggleNode);
				g.DrawRectangle(blackPen, toggleNode);
                
				//Draw -
				g.DrawLine(blackPen, (toggleNode.X + 2), (toggleNode.Y + (toggleNode.Height / 2)), (toggleNode.X + (toggleNode.Width - 2)), (toggleNode.Y + (toggleNode.Height / 2)) );
                
				if (!this.Expanded)
				{
					//Draw |
					g.DrawLine(blackPen, (toggleNode.X + (toggleNode.Width /2)), (toggleNode.Y + 2), (toggleNode.X + (toggleNode.Width /2)), (toggleNode.Y + (toggleNode.Height - 2)));
				}
			}
		}

		
		/// <summary>
		/// Invalidates the shape
		/// </summary>
		public override void Invalidate()
		{
			Rectangle r = rectangle;
			r.Offset(-5,-5);
			r.Inflate(40,40);
			site.Invalidate(r);
		}

		/// <summary>
		/// Overrides the resize to repaint the shape
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public override void Resize(int width, int height)
		{
			base.Resize(width,height);
		
			Invalidate();
		}

		#endregion	
	}
}
