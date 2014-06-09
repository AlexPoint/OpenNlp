using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace Netron.Lithium
{
	/// <summary>
	/// Represents the connection between two connectors
	/// </summary>
	public class Connection : Entity
	{

		#region Fields
		/// <summary>
		/// the shape where the connection starts
		/// </summary>
		protected ShapeBase from;
		/// <summary>
		/// the shape where the connection ends
		/// </summary>
		protected ShapeBase to;
		/// <summary>
		/// the start and end points
		/// </summary>
		protected Point start, end;
		/// <summary>
		/// the pen used to draw the connection,
		/// can switch depending on the hovering state e.g.
		/// </summary>
		protected Pen currentPen;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the shape where the connection starts
		/// </summary>
		public ShapeBase From
		{
			get{return from;}
			set{from = value;}
		}

		/// <summary>
		/// Gets or sets where the connection ends
		/// </summary>
		public ShapeBase To
		{
			get{return to;}
			set{to = value;}
		}

		/// <summary>
		/// Get the point where the connection starts
		/// </summary>
		public Point Start
		{
			get
			{
				
				
				return new Point(from.X+from.Width/2,from.Y+from.Height/2);	
			}
		}

		/// <summary>
		/// Gets the point where connection ends
		/// </summary>
		public Point End
		{
			get
			{
				end = new Point(to.X+to.Width/2,to.Y+to.Height/2);
				
				return end;
			}
		}

		#endregion

		#region Constructors
		/// <summary>
		/// Default ctor
		/// </summary>
		public Connection()
		{
			
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="from">the shape where the connection starts</param>
		/// <param name="to">the shape where the connection ends</param>
		public Connection(ShapeBase from, ShapeBase to)
		{
			this.from = from;			
			this.to = to;
			currentPen = blackPen;
			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="from">the shape where the connection starts</param>
		/// <param name="to">the shape where the connection ends</param>
		/// <param name="color">the color of the connection</param>
		public Connection(ShapeBase from, ShapeBase to, Color color) : this(from, to)
		{
			currentPen = new Pen(color, 1f);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="from">the shape where the connection starts</param>
		/// <param name="to">the shape where the connection ends</param>
		/// <param name="color">the color of the connection</param>
		/// <param name="width">the (float) width of the connection (in pixels)</param>
		public Connection(ShapeBase from, ShapeBase to, Color color, float width) : this(from, to, color)
		{
			currentPen = new Pen(color, width);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Paints the connection on the canvas
		/// The From part is always the child node while the To part is 
		/// always the parent node.
		/// Hence; 
		/// - vertical: Parent->Child <=> Top->Bottom
		/// - horizontal: Parent->Child <=> Left->Right
		/// </summary>
		/// <param name="g"></param>
		public override void Paint(System.Drawing.Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			PointF p1, p2, p3, p4; //intermediate points
			if(visible)
			{
				if(hovered || isSelected)
					pen = redPen;
				else
					pen = currentPen;

				switch(site.ConnectionType)
				{
					case ConnectionType.Default:
					switch(site.LayoutDirection)
					{
						case TreeDirection.Vertical:
							p1 = new PointF(from.Left + from.Width/2, from.Top); 
							p2 = new PointF(to.Left + to.Width/2, to.Bottom+5);
							g.DrawLine(pen,p1,p2);
							break;
						case TreeDirection.Horizontal:
							p1 = new PointF(from.Left, from.Top +  from.Height/2); 
							p2 = new PointF(to.Right +4, to.Top + to.Height/2);
							g.DrawLine(pen,p1,p2);
							break;
					}
						break;
					case ConnectionType.Traditional:
					switch(site.LayoutDirection)
					{
						case TreeDirection.Vertical:
							p1 = new PointF(from.Left + from.Width/2, from.Top - (from.Top - to.Bottom)/2); 
							p2 = new PointF(to.Left + to.Width/2, from.Top - (from.Top - to.Bottom)/2);
							g.DrawLine(pen, Start,p1);
							g.DrawLine(pen, p1, p2);
							g.DrawLine(pen, End, p2);
							break;
						case TreeDirection.Horizontal:

							p1 = new PointF(to.Right + (from.Left - to.Right)/2, from.Top + from.Height/2); 
							p2 = new PointF(to.Right + (from.Left - to.Right)/2, to.Top + to.Height/2);
							g.DrawLine(pen, Start,p1);
							g.DrawLine(pen, p1, p2);
							g.DrawLine(pen, End, p2);
							break;
					}
						break;
					
					case ConnectionType.Bezier:
					switch(site.LayoutDirection)
					{
						case TreeDirection.Vertical:
							p1 = new PointF(from.Left+from.Width/2,from.Top);
							p2 = new PointF(from.Left + from.Width/2, from.Top - (from.Top - to.Bottom)/2); 
							p3 = new PointF(to.Left + to.Width/2, from.Top - (from.Top - to.Bottom)/2);
							p4 = new PointF(to.Left+to.Width/2,to.Bottom);
							g.DrawBezier(pen, p1, p2, p3, p4);
							
							break;
						case TreeDirection.Horizontal:

							p1 = new PointF(to.Right, to.Top + to.Height/2); 							
							p2 = new PointF(to.Right + (from.Left - to.Right)/2, to.Top + to.Height/2);
							p3 = new PointF(to.Right + (from.Left - to.Right)/2, from.Top + from.Height/2); 
							p4 = new PointF(from.Left,from.Top + from.Height/2);
							g.DrawBezier(pen, p1, p2, p3, p4);
							break;
					}
						break;
				}
			}
			
		}
		/// <summary>
		/// Invalidates the connection
		/// </summary>
		public override void Invalidate()
		{
			
			site.Invalidate(Rectangle.Union(from.rectangle,to.rectangle));
		}

		/// <summary>
		/// Tests if the mouse hits this connection
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(Point p)
		{
			PointF p1,p2, p3, s;
			RectangleF r1, r2, r3;

			switch(site.ConnectionType)
			{
				case ConnectionType.Default:
					#region The default Hit method
					
					float o,u;
					p1 = start; p2 = end;
	
					// p1 must be the leftmost point.
					if (p1.X > p2.X) { s = p2; p2 = p1; p1 = s; }

					//this is specifically necessary when the layout works horizontally
					//the method beneth will not return true as should be in this case
					if(p1.Y==p2.Y)
					{
						p1.Y+=-3;
						return new RectangleF(p1,new SizeF(p2.X-p1.X,6)).Contains(p);
					}
					r1 = new RectangleF(p1.X, p1.Y, 0, 0);
					r2 = new RectangleF(p2.X, p2.Y, 0, 0);
					r1.Inflate(3, 3);
					r2.Inflate(3, 3);
					//this is like a topological neighborhood
					//the connection is shifted left and right
					//and the point under consideration has to be in between.						
					if (RectangleF.Union(r1, r2).Contains(p))
					{
				
						if (p1.Y < p2.Y) //SWNE
						{
							o = r1.Left + (((r2.Left - r1.Left) * (p.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
							u = r1.Right + (((r2.Right - r1.Right) * (p.Y - r1.Top)) / (r2.Top - r1.Top));
							return ((p.X > o) && (p.X < u));
						}
						else //NWSE
						{
							o = r1.Left + (((r2.Left - r1.Left) * (p.Y - r1.Top)) / (r2.Top - r1.Top));
							u = r1.Right + (((r2.Right - r1.Right) * (p.Y - r1.Bottom)) / (r2.Bottom - r1.Bottom));
							return ((p.X > o) && (p.X < u));
						}
					}
					#endregion
					break;
				case ConnectionType.Traditional:
					#region The rectangular Hit method
						switch(site.LayoutDirection)
						{
							case TreeDirection.Vertical:
								p1 = new PointF(from.Left + from.Width/2-5, from.Top - (from.Top - to.Bottom)/2-5); //shift 5 to contain the connection
								p2 = new PointF(to.Left + to.Width/2-5, from.Top - (from.Top - to.Bottom)/2-5);
								p3 = new Point(to.Left+to.Width/2-5,to.Bottom-5);

								r1 = new RectangleF(p1, new SizeF(10,(from.Top - to.Bottom)/2+5)); 
								if(p1.X<p2.X)
									r2 = new RectangleF(p1,new SizeF(p2.X-p1.X,10));
								else
									r2 = new RectangleF(p2,new SizeF(p1.X-p2.X,10));
								r3 = new RectangleF(p3, new SizeF(10, (from.Top - to.Bottom)/2+5));
								return r1.Contains(p.X,p.Y) || r2.Contains(p.X,p.Y) || r3.Contains(p.X,p.Y) ;
								
							case TreeDirection.Horizontal:

								p1 = new PointF(to.Right + (from.Left - to.Right)/2, from.Top + from.Height/2); 
								p2 = new PointF(to.Right + (from.Left - to.Right)/2, to.Top + to.Height/2);
							
								break;
						}
					#endregion
					break;
			}


			return false;
		}

		/// <summary>
		/// Moves the connection with the given shift
		/// </summary>
		/// <param name="p"></param>
		public override void Move(Point p)
		{

		}


		#endregion

	}
}
