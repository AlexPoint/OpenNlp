using System;
using System.Diagnostics;
using System.Collections;
namespace Netron.Lithium
{
	/// <summary>
	/// 
	/// </summary>
	public class GraphAbstract
	{

		#region Fields

		/// <summary>
		/// the description of the diagram,
		/// can be expanded to much more and whatever you wish to store about author etc...
		/// </summary>
		protected string description = "No description";

		/// <summary>
		/// the collection of shapes on the canvas
		/// </summary>
		protected ShapeCollection shapes;

		/// <summary>
		/// the collection of connections on the canvas
		/// </summary>
		protected ConnectionCollection connections;

		/// <summary>
		/// the root of the diagram
		/// </summary>
		protected ShapeBase root;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the root of the diagram
		/// </summary>
		public ShapeBase Root
		{
			get{return root;}
			set{root = value;}
		}

		/// <summary>
		/// The description of the graph
		/// </summary>
		public string Description
		{
			get{return description;}
			set{description = value;}
		}

		/// <summary>
		/// Gets or sets the shape collection
		/// </summary>
		public ShapeCollection Shapes
		{
			get{return shapes;}
			set{shapes = value;}
		}

		/// <summary>
		/// Gets or sets the connection collection of the control
		/// </summary>
		public ConnectionCollection Connections
		{
			get{return connections;}
			set{connections = value;}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default ctor
		/// </summary>
		public GraphAbstract()
		{
			connections = new ConnectionCollection();
			shapes = new ShapeCollection();
		}

		#endregion

		#region Methods
		#region Traversals

		/// <summary>
		/// BFT of the diagram with the given visitor, starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		public void BreadthFirstTraversal(IVisitor visitor, ShapeBase shape)
		{
			for (int i = 0; i < shapes.Count; i++)
			{
				shapes[i].visited = false;
			}
			BFT(visitor, shape);
		}
		/// <summary>
		/// BFT of the diagram starting from the root
		/// </summary>
		/// <param name="visitor"></param>
		public void BreadthFirstTraversal(IVisitor visitor)
		{
		
			if(root==null) return;

			for (int i = 0; i < shapes.Count; i++)
			{
				shapes[i].visited = false;
			}
			BFT(visitor, root);
		}

		/// <summary>
		/// BFT of the diagram with the given visitor, starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		private  void BFT(IVisitor visitor, ShapeBase shape)
		{			
		
			Queue queue = new Queue();
			queue.Enqueue(shape);
			shape.visited = true;
			while (!(queue.Count==0 || visitor.IsDone))
			{
				ShapeBase node1 = queue.Dequeue() as ShapeBase;
				visitor.Visit(node1);		
				node1.visited = true;
				
				try
				{
					foreach(ShapeBase sh in node1.childNodes)
					{
						
						if (!sh.visited)
						{
							queue.Enqueue(sh);
							//visitor.Visit(sh);							
							//sh.visited= true;
						}
					}

				}
				catch(Exception exc)
				{
					Trace.WriteLine(exc.Message);
				}
				//visitor.PostVisit(node1);
			}
		}

		/// <summary>
		/// DFT of the (sub)graph starting from the given shape
		/// </summary>
		/// <param name="visitor"></param>
		/// <param name="shape"></param>
		private void DFT(IVisitor visitor, ShapeBase shape)
		{
			if (!visitor.IsDone)
			{
				if(typeof(IPrePostVisitor).IsInstanceOfType(visitor))
					(visitor as IPrePostVisitor).PreVisit(shape);

				visitor.Visit(shape);
				shape.visited = true;				
				try
				{
					foreach(ShapeBase sh in shape.childNodes)
					{						
						if (!sh.visited)
						{
							DepthFirstTraversal(visitor, sh);
						}
					}
				}
				catch(Exception exc)
				{
					Trace.WriteLine(exc.Message);
				}

				if(typeof(IPrePostVisitor).IsInstanceOfType(visitor))
					(visitor as IPrePostVisitor).PostVisit(shape);
			}
		}

		/// <summary>
		/// DFT of the (sub)graph starting from the given shape
		/// </summary>
		/// <param name="visitor">an IVisitor object</param>
		/// <param name="shape">the shape to start the visiting process from</param>
		public void DepthFirstTraversal(IVisitor visitor, ShapeBase shape)
		{
		
			for (int i = 0; i < shapes.Count; i++)
			{
				shapes[i].visited = false;
			}
			DFT(visitor, shape);
		}
		/// <summary>
		/// DFT of the diagram starting from the root
		/// </summary>
		/// <param name="visitor"></param>
		public void DepthFirstTraversal(IVisitor visitor)
		{
		
			if(root==null) return;

			for (int i = 0; i < shapes.Count; i++)
			{
				shapes[i].visited = false;
			}
			DFT(visitor, root);
		}
		
		#endregion
		#endregion
	}
}
