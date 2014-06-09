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
	/// Netron's 'Lithium'  tree control
	/// </summary>
	public class LithiumControl : ScrollableControl
	{
		#region Events
	

		/// <summary>
		/// notifies the host to show the properties usually in the property grid
		/// </summary>
		public event ShowProps OnShowProps;

		/// <summary>
		/// occurs when a new node is added to the diagram
		/// </summary>
		public event ShapeData OnNewNode;

		/// <summary>
		/// occurs when certain objects send info out
		/// </summary>
		public event Messager OnInfo;

		/// <summary>
		/// occurs when a shape is deleted
		/// </summary>
		public event ShapeData OnDeleteNode;

		#endregion

		#region Fields

		/// <summary>
		/// the current layout direction
		/// </summary>
		protected TreeDirection layoutDirection = TreeDirection.Vertical;

		/// <summary>
		/// the space between the nodes
		/// </summary>
		protected int wordSpacing = 20;

		/// <summary>
		/// the height between branches
		/// </summary>
		protected int branchHeight = 60;

		/// <summary>
		/// whether the tree layout algorithm will do its work by default
		/// </summary>
		protected bool layoutEnabled = true;
		/// <summary>
		/// the default name when the root is added
		/// </summary>
		protected readonly string defaultRootName = "Root";
		/// <summary>
		/// the abstract representation of the graph
		/// </summary>
		protected internal GraphAbstract graphAbstract;		
		/// <summary>
		/// the entity hovered by the mouse
		/// </summary>
		protected Entity hoveredEntity;
		/// <summary>
		/// the unique entity currently selected
		/// </summary>
		protected Entity selectedEntity;
		/// <summary>
		/// whether we are tracking, i.e. moving something around
		/// </summary>
		protected bool tracking = false;
		/// <summary>
		/// just a reference point for the OnMouseDown event
		/// </summary>
		protected Point refp;
		/// <summary>
		/// the context menu of the control
		/// </summary>
		protected ContextMenu menu;
		/// <summary>
		/// A simple, general purpose random generator
		/// </summary>
		protected Random rnd;
		/// <summary>
		/// simple proxy for the propsgrid of the control
		/// </summary>
		protected Proxy proxy;
		/// <summary>
		/// whether the diagram is moved globally
		/// </summary>
		protected bool globalMove = false;

		/// <summary>
		/// just the default gridsize used in the paint-background method
		/// </summary>
		protected Size gridSize = new Size(10,10);

		/// <summary>
		/// the new but volatile connection
		/// </summary>
		protected internal Connection neoCon = null;

		/// <summary>
		/// memory of a connection if the volatile does not end up to a solid connection
		/// </summary>
		protected ShapeBase memChild = null, memParent = null;

		/// <summary>
		/// the type of connection
		/// </summary>
		protected ConnectionType connectionType = ConnectionType.Default;

		#endregion

		#region Properties
		
		/// <summary>
		/// Gets or sets the type of connection drawn
		/// </summary>
		public ConnectionType ConnectionType
		{
			get{return connectionType;}
			set{connectionType = value;
			Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the direction the tree-layout expands the tree
		/// </summary>
		public TreeDirection LayoutDirection
		{
			get{return layoutDirection;}
			set{layoutDirection = value;
			//update in case it has changed
				switch(value)
				{
					case TreeDirection.Horizontal:
						branchHeight = 120;
						wordSpacing = 20;
						break;
					case TreeDirection.Vertical:
						branchHeight = 70;
						wordSpacing = 20;
						break;

				}
				DrawTree();
			}
		}
		/// <summary>
		/// Gets or sets whether the tree layout algorithm will do its work by default
		/// </summary>
		public bool LayoutEnabled
		{
			get{return layoutEnabled ;}
			set{
				layoutEnabled = value;
				if(value) //if set to true, let's rectify what eventually has been distorted by the user
					DrawTree();
			}
		}

		/// <summary>
		/// Gets or sets the shape collection
		/// </summary>
		[Browsable(false)]
		public ShapeCollection Shapes
		{
			get{return graphAbstract.Shapes;}
			set{graphAbstract.Shapes = value;}
		}
		/// <summary>
		/// Gets or sets the connection collection
		/// </summary>
		[Browsable(false)]
		public ConnectionCollection Connections
		{
			get{return graphAbstract.Connections;}
			set{graphAbstract.Connections = value;}
		}
	
		/// <summary>
		/// Gets the root of the diagram
		/// </summary>
		[Browsable(false)]
		public ShapeBase Root
		{
			get{return graphAbstract.Root;}
		}

		/// <summary>
		/// Gets or sets the height between the tree branches
		/// </summary>
		[Browsable(true)]
		public int BranchHeight
		{
			get{return branchHeight;}
			set{
				branchHeight = value;
				DrawTree();
			}
		}
		/// <summary>
		/// Gets or sets the spacing between the nodes
		/// </summary>
		[Browsable(true)]
		public int WordSpacing
		{
			get{return wordSpacing;}
			set
			{
				wordSpacing = value;
				DrawTree();
			}
		}


		#endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		public LithiumControl()
		{
			//double-buffering
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			//init the abstract
			graphAbstract = new GraphAbstract();			
			graphAbstract.Shapes.OnShapeAdded+=new ShapeData(OnShapeAdded);

			AddRoot(defaultRootName);

			//default menu, can be overwritten in the design of the application
			menu = new ContextMenu();
			BuildMenu();
			this.ContextMenu = menu;

			//init the randomizer
			rnd = new Random();

			//init the proxy
			proxy = new Proxy(this);

			//allow scrolling
			this.AutoScroll=true;			
			this.HScroll=true;
			this.VScroll=true;
		}

		#endregion

		#region Methods

		private void visitor_OnDelete(ShapeBase shape)
		{
			if(OnDeleteNode!=null)
				OnDeleteNode(shape);
		}
		/// <summary>
		/// Passes the event from the abstracts shape collection to the outside.
		/// Having the event in the GraphAbstract being raised centralizes it,
		/// otherwise the event should be raise in various places
		/// </summary>
		/// <param name="shape"></param>
		private void OnShapeAdded(ShapeBase shape)
		{
			if(this.OnNewNode!=null)
				OnNewNode(shape);
		}

		/// <summary>
		/// Builds the context menu
		/// </summary>
		private void BuildMenu()
		{
			MenuItem mnuDelete = new MenuItem("Delete",new EventHandler(OnDelete));
			menu.MenuItems.Add(mnuDelete);
			
			MenuItem mnuProps = new MenuItem("Properties", new EventHandler(OnProps));
			menu.MenuItems.Add(mnuProps);

			MenuItem mnuDash = new MenuItem("-");
			menu.MenuItems.Add(mnuDash);			

			MenuItem mnuShapes = new MenuItem("Change to");
			menu.MenuItems.Add(mnuShapes);

			MenuItem mnuRecShape = new MenuItem("Rectangular shape", new EventHandler(OnRecShape));
			mnuShapes.MenuItems.Add(mnuRecShape);
			
			MenuItem mnuOvalShape = new MenuItem("Oval shape", new EventHandler(OnOvalShape));
			mnuShapes.MenuItems.Add(mnuOvalShape);

			MenuItem mnuDash2 = new MenuItem("-");
			menu.MenuItems.Add(mnuDash2);

			MenuItem mnuTLShape = new MenuItem("Add Text label", new EventHandler(OnTextLabelShape));
			menu.MenuItems.Add(mnuTLShape);


			
		}


		#region Menu handlers
		/// <summary>
		/// Deletes the currently selected object from the canvas
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDelete(object sender, EventArgs e)
		{
			if(selectedEntity!=null)
			{
				if(typeof(ShapeBase).IsInstanceOfType(selectedEntity))
				{
					this.Shapes.Remove(selectedEntity as ShapeBase);
					this.Invalidate();
				}
				else
					if(typeof(Connection).IsInstanceOfType(selectedEntity))
				{
					this.Connections.Remove(selectedEntity as Connection);
					this.Invalidate();
				}

			}
		}


		/// <summary>
		/// Asks the host to show the props
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnProps(object sender, EventArgs e)
		{
			object thing;
			if(this.selectedEntity==null) 
				thing = this.proxy;
			else
				thing =selectedEntity;
			if(this.OnShowProps!=null)
				OnShowProps(thing);

		}

		
		private void OnRecShape(object sender, EventArgs e)
		{
			throw new NotImplementedException("Sorry, not implemented yet");
		}
		private void OnOvalShape(object sender, EventArgs e)
		{
			throw new NotImplementedException("Sorry, not implemented yet");
		}
		private void OnTextLabelShape(object sender, EventArgs e)
		{
			AddShape(ShapeTypes.TextLabel,refp);
		}
	
		#endregion

		/// <summary>
		/// Paints the control
		/// </summary>
		/// <remarks>
		/// If you switch the painting order of Connections and shapes the connection line
		/// will be underneath/above the shape
		/// </remarks>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			//use the best quality, with a performance penalty
			g.SmoothingMode= System.Drawing.Drawing2D.SmoothingMode.AntiAlias;		

			int scrollPositionX = this.AutoScrollPosition.X;
			int scrollPositionY = this.AutoScrollPosition.Y;
			g.TranslateTransform(scrollPositionX, scrollPositionY);

			//zoom
			//g.ScaleTransform(0.1f,0.1f);

			//draw the Connections
			for(int k=0; k<Connections.Count; k++)
			{
				Connections[k].Paint(g);				
			}
			if(neoCon!=null) neoCon.Paint(g);
			//loop over the shapes and draw
			for(int k=0; k<Shapes.Count; k++)
			{
				if(Shapes[k].visible)
					Shapes[k].Paint(g);
			}
			
			
		}
		/// <summary>
		/// Paints the background
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground (e);
//			Graphics g = e.Graphics;
//
//			g.DrawString("Lithium Tree Layout Control [version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "]",Font,Brushes.SlateGray,new Point(20,10));
//			g.DrawString("<Shift>: move the diagram",Font,Brushes.SlateGray,new Point(20,30));
//			g.DrawString("<Ctrl>: move a node to a new parent",Font,Brushes.SlateGray,new Point(20,40));
//			g.DrawString("<Alt>: add a new child node",Font,Brushes.SlateGray,new Point(20,50));
		}
		

		#region Add random node
		/// <summary>
		/// Adds a random node 
		/// </summary>
		public void AddRandomNode()
		{
			AddRandomNode("Random");
		}		

		/// <summary>
		/// Adds a random node to the diagram
		/// </summary>
		/// <param name="newName">the text of the newly added shape</param>
		public void AddRandomNode(string newName)
		{
			//Random rnd = new Random();

			int max = Shapes.Count-1;

			if(max<0) return;

			ShapeBase shape = Shapes[rnd.Next(0,max)];
			shape.AddChild(newName);

			DrawTree();
		}
		#endregion

		#region New diagram
		/// <summary>
		/// Starts a new diagram and forgets about everything
		/// You need to call the Save method before this if you wish to keep the current diagram.
		/// </summary>
		/// <param name="rootName">the text of the root in the new diagram</param>
		public void NewDiagram(string rootName)
		{
			this.graphAbstract=new GraphAbstract();
			this.AddRoot(rootName);
			CenterRoot();
			Invalidate();
		}

		/// <summary>
		/// Starts a new diagram and forgets about everything
		/// You need to call the Save method before this if you wish to keep the current diagram.
		/// </summary>
		public void NewDiagram()
		{
			this.graphAbstract = new GraphAbstract();
			this.AddRoot(defaultRootName);
			CenterRoot();
			Invalidate();
		}
		#endregion

		/// <summary>
		/// Adds the root of the diagram to the canvas
		/// </summary>
		/// <param name="rootText"></param>
		private ShapeBase AddRoot(string rootText)
		{
			if(Shapes.Count>0)
				throw new Exception("You cannot set the root unless the diagram is empty");
			SimpleRectangle root = new SimpleRectangle(this);
			root.Location = new Point(Width/2+50,Height/2+50);
			root.Width = 50;
			root.Height = 25;
			root.Text = rootText;
			root.ShapeColor = Color.SteelBlue;
			root.IsRoot = true;
			root.Font = Font;		
			root.visible = false;
			root.level = 0;
			Fit(root);
			//set the root of the diagram
			this.graphAbstract.Root = root;
			Shapes.Add(root);
			return root;
		}

	

		/// <summary>
		/// Centers the root on the control's canvas
		/// </summary>
		public void CenterRoot()
		{
			graphAbstract.Root.rectangle.Location = new Point(Width/2,Height/2);
			//Invalidate();
			DrawTree();
			
		}


		/// <summary>
		/// Adds a shape to the canvas or diagram
		/// </summary>
		/// <param name="shape"></param>
		public ShapeBase AddShape(ShapeBase shape)
		{
			Shapes.Add(shape);
			shape.Site = this;
			this.Invalidate();
			return shape;
		}
		/// <summary>
		/// Adds a predefined shape
		/// </summary>
		/// <param name="type"></param>
		/// <param name="location"></param>
		public ShapeBase AddShape(ShapeTypes type, Point location)
		{
			ShapeBase shape = null;
			switch(type)
			{
				case ShapeTypes.Rectangular:
					shape = new SimpleRectangle(this);
					break;
				case ShapeTypes.Oval:
					shape = new OvalShape(this);
					break;
				case ShapeTypes.TextLabel:
					shape = new TextLabel(this);
					shape.Location = location;
					shape.ShapeColor = Color.Transparent;
					shape.Text = "A text label (change the text in the property grid)";
					shape.Width = 350;
					shape.Height = 30;
					Shapes.Add(shape);
					return shape;


			}
			if(shape==null) return null;
			shape.ShapeColor = Color.FromArgb(rnd.Next(0,255),rnd.Next(0,255),rnd.Next(0,255));
			shape.Location = location;
			Shapes.Add(shape);
			return shape;
		}

		/// <summary>
		/// Move with the given vector
		/// </summary>
		/// <param name="p"></param>
		public void MoveDiagram(Point p)
		{
			
			//move the whole diagram
			
			foreach(ShapeBase shape in Shapes)
			{					
				shape.Move(p);										
				Invalidate();
			}				
			return;
			
		}




		#region Mouse event handlers

		/// <summary>
		/// Handles the mouse-down event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			Point p = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);		
			Rectangle r;
			#region SHIFT

//			if(Control.ModifierKeys==Keys.Shift)
//			{
//				globalMove = true;
//				refp = p; //useful for all kind of things
//				return;
//			}

			#endregion

			ShapeBase sh ;

			#region LMB & RMB
			//test for shapes
			for(int k=0; k<Shapes.Count; k++)
			{
				sh = Shapes[k];
				if(sh.childNodes.Count>0)//has a [+/-]
				{
					if(layoutDirection==TreeDirection.Vertical)
						r = new Rectangle(sh.Left + sh.Width/2 - 5, sh.Bottom, 10, 10);
					else
						r = new Rectangle(sh.Right, sh.Y + sh.Height/2-5, 10, 10);

					if(r.Contains(p))
					{
						if(sh.expanded)
							sh.Collapse(true);
						else
							sh.Expand();
						DrawTree();
					}
				}
				if(Shapes[k].Hit(p))
				{
					//shapes[k].ShapeColor = Color.WhiteSmoke;
					if(selectedEntity!=null) 
						selectedEntity.IsSelected=false;
					selectedEntity = Shapes[k];
					selectedEntity.IsSelected = true;
					sh = selectedEntity as ShapeBase;	
					#region CONTROL
//					if(Control.ModifierKeys==Keys.Control && !sh.IsRoot)
//					{
//						tracking = true;
//						//remove from parent							
//						sh.parentNode.childNodes.Remove(sh);
//						Connections.Remove(sh, sh.parentNode);						
//						//...but keep the reference in case the user didn't find a new location
//						memChild = sh;
//						memParent = sh.parentNode;
//						//now remove the reference
//						sh.parentNode = null;
//					}
					#endregion
					//set the point for the next round
					refp=p;

					#region Double-click
					if(e.Button==MouseButtons.Left && e.Clicks==2)
					{							
						if(sh.expanded)
							sh.Collapse(true);
						else
							sh.Expand();
						DrawTree();
					}
					#endregion

					#region ALT
//					if(Control.ModifierKeys==Keys.Alt)
//					{
//						sh.AddChild("New");	
//						DrawTree();
//					}



					#endregion
					if(OnShowProps!=null)
						OnShowProps(Shapes[k]);
						
					return;
				}
			}
			if(selectedEntity!=null) selectedEntity.IsSelected=false;
			selectedEntity = null;
			Invalidate();
			refp = p; //useful for all kind of things
			//nothing was selected but we'll show the props of the control in this case
			if(OnShowProps!=null)
				OnShowProps(this.proxy);
			

			
			#endregion
			

		}

		/// <summary>
		/// Handles the mouse-move event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
		
			Point p = new Point(e.X - this.AutoScrollPosition.X, e.Y - this.AutoScrollPosition.Y);		
			//move the whole diagram
			if(globalMove)
			{
				foreach(ShapeBase shape in Shapes)
				{
					shape.Move(new Point(p.X-refp.X,p.Y-refp.Y));					
					Invalidate();
				}
				refp=p;
				return;
			}

			//move just one and its kids
			if(tracking)			
			{	
		
				ShapeBase sh = selectedEntity as ShapeBase;
				
				ResetPickup(); //forget about what happened before
				Pickup(sh); //pickup the shape hanging underneath the shape to move next				
				foreach(ShapeBase shape in Shapes)
				{
					if(!shape.pickup) continue;
					shape.Move(new Point(p.X-refp.X,p.Y-refp.Y));					
					Invalidate();
				}
				refp=p;
				//try to find the new parent
				SeekNewParent(sh);
				Invalidate();	
				return;
			}


			//hovering stuff
			for(int k=0; k<Shapes.Count; k++)
			{
				if(Shapes[k].Hit(p))
				{
					if(hoveredEntity!=null) hoveredEntity.hovered = false;
					Shapes[k].hovered = true;
					hoveredEntity = Shapes[k];
					//hoveredEntity.Invalidate();
					Invalidate();
					return;
				}
			}

			for(int k=0; k<Connections.Count; k++)
			{
				if(Connections[k].Hit(p))
				{
					if(hoveredEntity!=null) hoveredEntity.hovered = false;
					Connections[k].hovered = true;
					hoveredEntity = Connections[k];
					hoveredEntity.Invalidate();
					Invalidate();
					return;
				}
			}		
			//reset the whole process if nothing happened above
			HoverNone();
			Invalidate();


			
		}
		/// <summary>
		/// Handles the mouse-up event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			globalMove = false;
			Connection con = null;
			//test if we connected a connection
			if(tracking)
			{
			
				tracking = false;
				//make the volatile solid
				if(neoCon!=null)
				{
					con = new Connection(neoCon.To, neoCon.From);
					con.site = this;
					Connections.Add(con);
					//the From is the shape seeking a parent
					neoCon.To.childNodes.Add(neoCon.From);
					neoCon.From.parentNode = neoCon.To;
					con.visible = true;
					neoCon.To.Expand();
					neoCon.From.connection = con;
				}
				else //the user hasn't released near anything, so reset to the original situation
				{
					con = new Connection(memChild, memParent);
					con.site = this;
					Connections.Add(con);
					memParent.childNodes.Add(memChild);
					memChild.parentNode = memParent;
					con.visible = true;
					memChild.connection = con;
				}

				//either case, restart the process next
				neoCon = null;
				memChild = null;
				memParent = null;
				DrawTree();

			}
			
		}

	

		/// <summary>
		/// Find a new parent for the given shape. This creates a new volatile connection which will be solidified
		/// in the MouseUp handler.
		/// </summary>
		/// <param name="shape">the shape being moved around by the user</param>
		private void SeekNewParent(ShapeBase shape)
		{
			/* this is the fast way but gives problems when the shape is surrounded by other shapes
			 * which makes it difficult to attach it to one you want
			for(int k=0;k<Shapes.Count; k++)
			{
				if(Shapes[k]!=shape && Environment(Shapes[k],shape) && Shapes[k].parentNode!=shape && !Shapes[k].pickup)
				{
					neoCon = new Connection(shape, Shapes[k],  Color.Red,2f);
					neoCon.visible = true;
					Invalidate();
					return;
				}
			}
			*/
			double best = 10000d; 
			int chosen = -1;
			double dist;
			ShapeBase other;
			for(int k=0;k<Shapes.Count; k++)
			{
				other = Shapes[k];
				if(other!=shape && other.visible && other.parentNode!=shape && !other.pickup)
				{
					dist = Math.Sqrt((other.X-shape.X)*(other.X-shape.X)+(other.Y-shape.Y)*(other.Y-shape.Y));
					if(dist<best && dist< 120)
						chosen = k;				
				}
			}
			if(chosen>-1)
			{
				neoCon = new Connection(shape, Shapes[chosen],  Color.Red,2f);
				neoCon.visible = true;
				neoCon.site = this;
				return;
			}

			neoCon = null;
		}

		private  bool Environment(ShapeBase shape1, ShapeBase shape2)
		{
			return Math.Sqrt((shape1.X-shape2.X)*(shape1.X-shape2.X)+(shape1.Y-shape2.Y)*(shape1.Y-shape2.Y))<100;
		}

		private void ResetPickup()
		{
			for(int k=0; k<Shapes.Count; k++)
				Shapes[k].pickup = false;
		}

		private void Pickup(ShapeBase shape)
		{
			shape.pickup=true;
			for(int k =0; k< shape.childNodes.Count; k++)
			{
				shape.childNodes[k].pickup = true;
				if(shape.childNodes[k].childNodes.Count>0) 
					Pickup(shape.childNodes[k]);
			}

		}

		

		#endregion

		/// <summary>
		/// Resets the hovering status of the control, i.e. the hoverEntity is set to null.
		/// </summary>
		private void HoverNone()
		{
			if(hoveredEntity!=null) 
			{
				hoveredEntity.hovered = false;
				hoveredEntity.Invalidate();
			}
			hoveredEntity = null;
		}



		/// <summary>
		/// Collapses the whole diagram
		/// </summary>
		public void CollapseAll()
		{
			this.Root.Collapse(true);
		}

		/// <summary>
		/// Deletes the currently selected shape
		/// </summary>
		public void Delete()
		{
			if(selectedEntity==null) return;
			ShapeBase sh = selectedEntity as ShapeBase;
			if(sh!=null)
			{
				if(sh.IsRoot) return; //cannot delete the root
				//delete the node from the parent's children
				sh.parentNode.childNodes.Remove(sh);
				//delete everything underneath the node 
				DeleteVisitor visitor = new DeleteVisitor(this.graphAbstract);
				visitor.OnDelete+=new ShapeData(visitor_OnDelete);
				graphAbstract.DepthFirstTraversal(visitor,sh);
				DrawTree();
			}
			//Invalidate();
				
		}

		/// <summary>
		/// Expands the whole diagram
		/// </summary>
		public void ExpandAll()
		{
			IVisitor expander = new ExpanderVisitor();
			this.graphAbstract.DepthFirstTraversal(expander);
			DrawTree();
			
		}

		/// <summary>
		/// Saves the diagram to file in XML format (XML serialization)
		/// </summary>
		/// <param name="filePath"></param>
		public void SaveGraphAs(string filePath)
		{
			XmlTextWriter tw = new XmlTextWriter(filePath,System.Text.Encoding.Unicode);
			GraphSerializer g = new GraphSerializer(this);
			g.Serialize(tw);
			tw.Close();
		}

		/// <summary>
		/// Opens a diagram which was saved to XML previously (XML deserialization)
		/// </summary>
		/// <param name="filePath"></param>
		public void OpenGraph(string filePath)
		{
			XmlTextReader reader = new XmlTextReader(filePath);
			GraphSerializer ser = new GraphSerializer(this);
			graphAbstract = ser.Deserialize(reader) as GraphAbstract;
			reader.Close();
			DrawTree();
			Invalidate();
		}


		#region Layout algorithm

		int marginLeft = 10;			
		
		
	

		

		/// <summary>
		/// Generic entry point to layout the diagram on the canvas.
		/// The default LayoutDirection is vertical. If you wish to layout the tree in a certain
		/// direction you need to specify this property first. Also, the direction is global, you cannot have 
		/// different parts being drawn in different ways though it can be implemented.
		/// 
		/// </summary>
		public void DrawTree()
		{
			if(!layoutEnabled) return;
			Point p = Point.Empty; //the shift vector difference between the original and the moved root
			try
			{
				//start the recursion
				//the layout will move the root but it's reset to its original position
				switch(layoutDirection)
				{
					case TreeDirection.Vertical:
						
						p = new Point(graphAbstract.Root.X, graphAbstract.Root.Y);
						VerticalDrawTree(graphAbstract.Root,false,marginLeft,this.graphAbstract.Root.Y);						
						p = new Point(-graphAbstract.Root.X+p.X, -graphAbstract.Root.Y+ p.Y);						
						MoveDiagram(p);						
						break;
					case TreeDirection.Horizontal:
						p = new Point(graphAbstract.Root.X, graphAbstract.Root.Y);
						HorizontalDrawTree(graphAbstract.Root,false,marginLeft,10);
						p = new Point(-graphAbstract.Root.X+p.X, -graphAbstract.Root.Y+ p.Y);		
						MoveDiagram(p);	
						break;
				}

				int maxY = 0;
				foreach (ShapeBase shape in Shapes)
				{
					if (shape.ShapeColor == Color.Ivory)
					{
						if (shape.Visible)
						{
							if (shape.Y > maxY)
							{
								maxY = shape.Y;
							}
						}
					}
				}

				foreach (ShapeBase shape in Shapes)
				{
					if (shape.ShapeColor == Color.Ivory)
					{
						if (shape.Visible)
						{
							shape.Move(new Point(0, maxY - shape.Y));
						}
					}
				}

				CalculateScrollBars();
				
				Invalidate();
			}
			catch(Exception exc)
			{
				Trace.WriteLine(exc.Message);				
			}
		}

		private void CalculateScrollBars()
		{
			Point minPoint = new Point(int.MaxValue,int.MaxValue);
			Size maxSize = new Size(0,0);
			foreach(ShapeBase shape in Shapes)
			{
				if (shape.Visible)
				{
					if (shape.X + shape.Width > maxSize.Width)
					{
						maxSize.Width = shape.X + shape.Width;
					}

					if (shape.Y + shape.Height > maxSize.Height)
					{
						maxSize.Height = shape.Y + shape.Height;
					}

					if (shape.X < minPoint.X)
					{
						minPoint.X = shape.X;
					}

					if (shape.Y < minPoint.Y)
					{
						minPoint.Y = shape.Y;
					}
				}
			}

			MoveDiagram(new Point(50 - minPoint.X, 50 - minPoint.Y)); 

			maxSize.Width = maxSize.Width - minPoint.X + 100;
			maxSize.Height = maxSize.Height - minPoint.Y + 100;
			this.AutoScrollMinSize = maxSize;
		}
		/// <summary>
		/// Positions everything underneath the node and returns the total width of the kids
		/// </summary>
		/// <param name="containerNode"></param>
		/// <param name="first"></param>
		/// <param name="shiftLeft"></param>
		/// <param name="shiftTop"></param>
		/// <returns></returns>
		private int VerticalDrawTree(ShapeBase containerNode, bool first, int shiftLeft, int shiftTop)
		{
			bool isFirst = false;
			bool isParent = containerNode.childNodes.Count>0? true: false;
			int childrenWidth = 0;
			int thisX, thisY;		
			int returned = 0;
			int verticalDelta = branchHeight ; //the applied vertical shift of the child depends on the Height of the containerNode
			#region Children width
			for(int i =0; i<containerNode.childNodes.Count; i++)
			{
				//determine the width of the label
				if(i==0)			
					isFirst = true;				
				else 				
					isFirst = false;
				if(containerNode.childNodes[i].visible)
				{
					if((branchHeight - containerNode.Height) < 30) //if too close to the child, shift it with 40 units
						verticalDelta = containerNode.Height + 40;
					returned = VerticalDrawTree(containerNode.childNodes[i], isFirst, shiftLeft + childrenWidth, shiftTop + verticalDelta );									
					childrenWidth += returned;
					
				}		

			}
			if(childrenWidth>0 && containerNode.expanded)
				childrenWidth=Math.Max(Convert.ToInt32(childrenWidth + (containerNode.Width-childrenWidth)/2), childrenWidth); //in case the length of the containerNode is bigger than the total length of the children
			#endregion

			if(childrenWidth==0) //there are no children; this is the branch end
				childrenWidth = containerNode.Width+wordSpacing;
			
			#region Positioning
			thisY = shiftTop;			
			if(containerNode.childNodes.Count>0 && containerNode.expanded)
			{
				if(containerNode.childNodes.Count==1)
				{

					thisX = Convert.ToInt32(containerNode.childNodes[0].X+containerNode.childNodes[0].Width/2 - containerNode.Width/2);
				}
				else
				{
					float firstChild = containerNode.childNodes[0].Left+ containerNode.childNodes[0].Width/2;
					float lastChild = containerNode.childNodes[containerNode.childNodes.Count-1].Left + containerNode.childNodes[containerNode.childNodes.Count-1].Width/2;
					//the following max in case the containerNode is larger than the childrenWidth
					thisX = Convert.ToInt32(Math.Max(firstChild + (lastChild -firstChild - containerNode.Width)/2, firstChild));
				}
			}
			else
			{
				thisX = shiftLeft;		
				
			}
			
			containerNode.rectangle.X = thisX;
			containerNode.rectangle.Y = thisY;
			#endregion

			return childrenWidth;
		}

		/// <summary>
		/// Horizontal layout algorithm
		/// </summary>
		/// <param name="containerNode"></param>
		/// <param name="first"></param>
		/// <param name="shiftLeft"></param>
		/// <param name="shiftTop"></param>
		/// <returns></returns>
		private int HorizontalDrawTree(ShapeBase containerNode, bool first, int shiftLeft, int shiftTop)
		{
			bool isFirst = false;
			bool isParent = containerNode.childNodes.Count>0? true: false;
			int childrenHeight = 0;
			int thisX, thisY;		
			int returned = 0;
			int horizontalDelta = branchHeight;
			#region Children width
			for(int i =0; i<containerNode.childNodes.Count; i++)
			{
				//determine the width of the label
				if(i==0)			
					isFirst = true;				
				else 				
					isFirst = false;
				if(containerNode.childNodes[i].visible)
				{
					if((branchHeight - containerNode.Width) < 30) //if too close to the child, shift it with 40 units
						horizontalDelta = containerNode.Width + 40;
					returned = HorizontalDrawTree(containerNode.childNodes[i], isFirst, shiftLeft + horizontalDelta , shiftTop + childrenHeight );					
					childrenHeight += returned;
				}
				

			}
			#endregion

			if(childrenHeight==0) //there are no children; this is the branch end
				childrenHeight = containerNode.Height+wordSpacing;
			
			#region Positioning
			thisX = shiftLeft;			
			if(containerNode.childNodes.Count>0 && containerNode.expanded)
			{
				
					int firstChild = containerNode.childNodes[0].Y;
					int lastChild = containerNode.childNodes[containerNode.childNodes.Count-1].Y;
					thisY = Convert.ToInt32(firstChild + (lastChild - firstChild)/2);
				
			}
			else
			{
				thisY = Convert.ToInt32(shiftTop);		
				
			}
			
			containerNode.rectangle.X = thisX;
			containerNode.rectangle.Y = thisY;
			#endregion

			return childrenHeight;
		}

		private int Measure(string text)
		{
			Graphics g = Graphics.FromHwnd(this.Handle);

			return Size.Round(g.MeasureString(text,Font)).Width +37;
		}
		/// <summary>
		/// Resizes the shape to fit the text
		/// </summary>
		/// <param name="shape"></param>
		public void Fit(ShapeBase shape)
		{
			Graphics g = Graphics.FromHwnd(this.Handle);
			Size s =  Size.Round(g.MeasureString(shape.Text,Font));
			shape.Width =s.Width +20;
			shape.Height = s.Height+8;
		}

		private void FitAll()
		{
			foreach(ShapeBase shape in graphAbstract.Shapes)
				Fit(shape);
			Invalidate();
		}


	

		/// <summary>
		/// When the font of the control is changed all shapes get
		/// the new font assigned
		/// </summary>
		/// <param name="e"></param>
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged (e);

			foreach(ShapeBase shape in graphAbstract.Shapes)
				shape.Font = Font;
		}

		/// <summary>
		/// Adds a child node to the currently selected one
		/// </summary>
		public void AddChild()
		{
			if(selectedEntity==null) return;

			ShapeBase sh = selectedEntity as ShapeBase;
			if(sh!=null)
			{
				sh.AddChild("New node");
				DrawTree();
			}

		}

		/// <summary>
		/// DFT of the diagram with the given visitor, starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		public void DepthFirstTraversal(IVisitor visitor, ShapeBase shape)
		{
			graphAbstract.DepthFirstTraversal(visitor, shape);
		}


		/// <summary>
		/// DFT of the diagram with the given visitor, starting from the root
		/// </summary>
		/// <param name="visitor"></param>
		public void DepthFirstTraversal(IVisitor visitor)
		{
			graphAbstract.DepthFirstTraversal(visitor);
		}

		/// <summary>
		/// BFT of the diagram with the given visitor, starting from the root
		/// </summary>
		/// <param name="visitor"></param>
		public void BreadthFirstTraversal(IVisitor visitor)
		{
			graphAbstract.BreadthFirstTraversal(visitor);
		}

		/// <summary>
		/// BFT of the diagram with the given visitor, starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		public void BreadthFirstTraversal(IVisitor visitor, ShapeBase shape)
		{
			graphAbstract.BreadthFirstTraversal(visitor, shape);
		}



		#endregion

		#endregion

		
	}

	
}
