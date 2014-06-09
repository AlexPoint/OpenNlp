using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
namespace Netron.Lithium
{
	/// <summary>
	/// Base class for shapes
	/// </summary>
	public class ShapeBase : Entity
	{
		#region Fields
		/// <summary>
		/// the rectangle on which any shape lives
		/// </summary>
		protected internal Rectangle rectangle;
		/// <summary>
		/// the backcolor of the shapes
		/// </summary>
		protected Color shapeColor = Color.SteelBlue;
		/// <summary>
		/// the brush corresponding to the backcolor
		/// </summary>
		protected Brush shapeBrush;
		/// <summary>
		/// the text on the shape
		/// </summary>
		protected string text = string.Empty;
		/// <summary>
		/// whether this shape if the root
		/// </summary>
		protected bool isRoot = false;
		/// <summary>
		/// the child nodes collection
		/// </summary>
		protected internal ShapeCollection childNodes;
		/// <summary>
		/// used to drag child nodes
		/// </summary>
		protected internal bool pickup = false;
		/// <summary>
		/// points to the unique parent of this shape, unless it's the root and then Null
		/// </summary>
		protected internal ShapeBase parentNode = null;
		/// <summary>
		/// whether the shape is expanded
		/// If expanded, all the child nodes will have visible=true and vice versa
		/// </summary>
		protected internal bool expanded = false;
		/// <summary>
		/// this is the unique link to the parent unless this shape is the root
		/// </summary>
		protected internal Connection connection = null;
		/// <summary>
		/// used by the visiting pattern and tags whether this shape has been visited already
		/// </summary>
		protected internal bool visited = false;

		/// <summary>
		/// the level of the shape in the hierarchy
		/// </summary>
		protected internal int level = -1;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the child node collection of this shape
		/// </summary>
		[Browsable(false)]
		public ShapeCollection ChildNodes
		{
			get{return childNodes;}
			set{childNodes = value;}
		}


		/// <summary>
		/// Gets or sets whether this is the root of the diagram
		/// </summary>
		[Browsable(false)]
		public bool IsRoot
		{
			get{return isRoot;}
			set{isRoot = value;}
		}

	
		/// <summary>
		/// Gets the level of the shape in the hierarchy
		/// </summary>
		public int Level
		{
			get{return level;}			
		}

		/// <summary>
		/// Gets or sets the width of the shape
		/// </summary>
		[Browsable(true), Description("The width of the shape"), Category("Layout")]
		[GraphData]
		public int Width
		{
			get{return rectangle.Width;}
			set{
				Resize(value,Height); 
				site.DrawTree();
				site.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the height of the shape
		/// </summary>		
		[Browsable(true), Description("The height of the shape"), Category("Layout")]
		[GraphData]
		public int Height
		{
			get{return this.rectangle.Height;}
			set{
				Resize(this.Width,value); 
				site.DrawTree();
				site.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the text of the shape
		/// </summary>
		[Browsable(true), Description("The text shown on the shape"), Category("Layout")]
		[GraphData]
		public string Text
		{
			get{return text;}
			set{text = value; 
				Fit();
				site.DrawTree();
				site.Invalidate();
			}
		}

		/// <summary>
		/// the x-coordinate of the upper-left corner
		/// </summary>
		[Browsable(false), Description("The x-coordinate of the upper-left corner"), Category("Layout")]
		[GraphData]
		public int X
		{
			get{return rectangle.X;}
			set{
				Point p = new Point(value - rectangle.X, rectangle.Y);
				this.Move(p);
				site.Invalidate(); //note that 'this.Invalidate()' will not be enough
			}
		}

		/// <summary>
		/// the y-coordinate of the upper-left corner
		/// </summary>
		[Browsable(false), Description("The y-coordinate of the upper-left corner"), Category("Layout")]
		[GraphData]
		public int Y
		{
			get{return rectangle.Y;}
			set{
				Point p = new Point(rectangle.X, value - rectangle.Y);
				this.Move(p);
				site.Invalidate();
			}
		}
		/// <summary>
		/// The backcolor of the shape
		/// </summary>
		[Browsable(true), Description("The backcolor of the shape"), Category("Layout")]
		[GraphData]
		public Color ShapeColor
		{
			get{return shapeColor;}
			set{shapeColor = value; SetBrush(); Invalidate();}
		}
		/// <summary>
		/// Gets or sets whether the shape is visible
		/// </summary>
		[Browsable(false), GraphData]
		public bool Visible
		{
			get{return visible;}
			set{visible = value;}
		}
		/// <summary>
		/// Gets or sets whether the shape is expanded/collapsed
		/// </summary>
		[Browsable(false), GraphData]
		public bool Expanded
		{
			get{return expanded;}
			set{expanded = value;}
		}
		/// <summary>
		/// Gets the (unique) parent node of this shape
		/// Null if this is the root
		/// </summary>
		[Browsable(false), GraphData]
		public ShapeBase ParentNode
		{
			get{return parentNode;}			
		}

		/// <summary>
		/// Gets or sets the location of the shape;
		/// </summary>
		[Browsable(false)]
		public Point Location
		{
			get{return new Point(this.rectangle.X,this.rectangle.Y);}
			set{
				//we use the move method but it requires the delta value, not an absolute position!
				Point p = new Point(value.X-rectangle.X,value.Y-rectangle.Y);
				//if you'd use this it would indeed move the shape but not the connector s of the shape
				//this.rectangle.X = value.X; this.rectangle.Y = value.Y; Invalidate();
				this.Move(p);
			}
		}

		/// <summary>
		/// Gets the left coordiante of the rectangle
		/// </summary>
		[Browsable(false)]
		public int Left
		{
			get{return this.rectangle.Left;}
		}

		/// <summary>
		/// Gets the right coordiante of the rectangle
		/// </summary>
		[Browsable(false)]
		public int Right
		{
			get{return this.rectangle.Right;}
		}

		/// <summary>
		/// Get the bottom coordinate of the shape
		/// </summary>
		[Browsable(false)]
		public int Bottom
		{
			get{return this.rectangle.Bottom;}
		}

		/// <summary>
		/// Gets the top coordinate of the rectangle
		/// </summary>
		[Browsable(false)]
		public int Top
		{
			get{return this.rectangle.Top;}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default ctor
		/// </summary>
		public ShapeBase()
		{
				Init();
		}
		/// <summary>
		/// Constructor with the site of the shape
		/// </summary>
		/// <param name="site">the graphcontrol instance to which the shape is attached</param>
		public ShapeBase(LithiumControl site) : base(site)
		{
				Init();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Resizes the shape's rectangle in function of the containing text
		/// </summary>
		public void Fit()
		{
			Graphics g = Graphics.FromHwnd(site.Handle);
			Size s =  Size.Round(g.MeasureString(text,Font));
			//if(childNodes.Count>0)
				//rectangle.Width =s.Width +10 + this.childNodes.Count.ToString().Length*5 +5; //the last part is to addition for the '[child count]' part
			//else
				rectangle.Width =s.Width +10;
			rectangle.Height = s.Height+8;
			Invalidate();
		}

		/// <summary>
		/// Expand the children, if any
		/// </summary>
		public void Expand()
		{
			expanded = true;
			visible = true;
			for(int k =0; k<childNodes.Count;k++)
			{
				childNodes[k].visible = true;
				childNodes[k].connection.visible = true;
				if(childNodes[k].expanded) childNodes[k].Expand();
			}
				
		}
		/// <summary>
		/// Collapses the children underneath this shape
		/// </summary>
		/// <param name="change">true to set the expanded field to true</param>
		public void Collapse(bool change)
		{
			if(change) expanded = false;
			for(int k =0; k<childNodes.Count;k++)
			{
				childNodes[k].visible = false;
				childNodes[k].connection.visible = false;
				if(childNodes[k].expanded) childNodes[k].Collapse(false);
			}
		}

		/// <summary>
		/// Adds a child to this shape
		/// </summary>
		/// <param name="text">the text of the newly created shape</param>
		/// <returns>the create shape</returns>
		public ShapeBase AddChild(string text)
		{
			
			SimpleRectangle shape = new SimpleRectangle(site);
			shape.Location = new Point(Width/2+50,Height/2+50);
			shape.Width = 50;
			shape.Height = 25;
			shape.Text = text;
			shape.ShapeColor = Color.Linen;
			shape.IsRoot = false;
			shape.parentNode = this;
			shape.Font = font;
			shape.level = level+1;
			shape.Fit(); //fit the child
			
			
			//add to the collections
			site.graphAbstract.Shapes.Add(shape);
			this.childNodes.Add(shape);
			
			
			//add a connection; From=child, To=parent
			Connection con = new Connection(shape, this);
			site.Connections.Add(con);
			con.site = this.site;
			con.visible = true;
			shape.connection = con;

			if(visible)
				Expand();
			else
			{
				shape.visible = false;
				con.visible = false;
			}
			Fit(); //the cild count added at the end will enlarge the rectangle, so we have to fit 
			return shape;
		}

		/// <summary>
		/// Summarizes the initialization used by the constructors
		/// </summary>
		private void Init()
		{
			rectangle = new Rectangle(0,0,100,70);
		
			childNodes = new ShapeCollection();
			SetBrush();
		}


	
		/// <summary>
		/// Sets the brush corresponding to the backcolor
		/// </summary>
		protected void SetBrush()
		{
			if(isSelected)
				shapeBrush = new SolidBrush(Color.YellowGreen);
			else
				shapeBrush = new SolidBrush(shapeColor);

			
		}

		/// <summary>
		/// Overrides the abstract paint method
		/// </summary>
		/// <param name="g">a graphics object onto which to paint</param>
		public override void Paint(System.Drawing.Graphics g)
		{
			return;
		}

		/// <summary>
		/// Override the abstract Hit method
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override bool Hit(System.Drawing.Point p)
		{
			return false;
		}

		/// <summary>
		/// Overrides the abstract Invalidate method
		/// </summary>
		public override void Invalidate()
		{
				site.Invalidate(rectangle);
		}



		/// <summary>
		/// Moves the shape with the given shift
		/// </summary>
		/// <param name="p">represent a shift-vector, not the absolute position!</param>
		public override void Move(Point p)
		{	
				this.rectangle.X += p.X;
				this.rectangle.Y += p.Y;				
				this.Invalidate();
			
		}

		/// <summary>
		/// Resizes the shape 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public virtual void Resize(int width, int height)
		{
			this.rectangle.Height = height;
			this.rectangle.Width = width;
			this.site.Invalidate();
		}


		#endregion
	}
}
