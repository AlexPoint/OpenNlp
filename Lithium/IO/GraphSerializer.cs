using System;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Xml.Schema;

namespace Netron.Lithium
{
	/// <summary>
	/// Assists the serialization of the diagram
	/// </summary>
	public class GraphSerializer
	{
		#region Fields

		/// <summary>
		/// the key/value pairs of to-be stored xml entries
		/// </summary>
		private Hashtable keyList = new Hashtable();
		/// <summary>
		/// the control this serializer is attached to
		/// </summary>
		private LithiumControl site;
		#endregion

		#region Constructor
		/// <summary>
		/// Default ctor
		/// </summary>
		/// <param name="control"></param>
		public GraphSerializer(LithiumControl control)
		{	
			site = control;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Serializes the given graph to xml
		/// </summary>
		public void Serialize(XmlWriter writer)
		{
			GraphType graph = new GraphType();
			GraphAbstract g	= site.graphAbstract;

			//here you can serialize whatever global pros
			graph.Description = g.Description;
			graph.ID = "id1";


			foreach ( ShapeBase s in g.Shapes )
			{
				graph.Nodes.Add(SerializeNode(s));
			}

			foreach(Connection c in g.Connections)
			{
				graph.Edges.Add(SerializeEdge(c));
			}

			// serialize
			XmlSerializer ser = new XmlSerializer(typeof(GraphType));
			ser.Serialize(writer,graph);
		}

		/// <summary>
		/// Deserializes the graph's xml
		/// </summary>
		/// <returns></returns>
		public GraphAbstract Deserialize(XmlReader reader)
		{
			XmlSerializer ser = new XmlSerializer(typeof(GraphType));
			GraphType gtype = ser.Deserialize(reader) as GraphType;
			return Deserialize(gtype);
		}


		/// <summary>
		/// Deserializes the graphtype
		/// </summary>
		/// <param name="g">the graphtype which acts as an intermediate storage between XML and the GraphAbstract
		/// </param>
		/// <returns></returns>
		private GraphAbstract Deserialize(GraphType g)
		{
			GraphAbstract abs = new GraphAbstract();

			abs.Description = g.Description;
			ShapeBase shape = null, from = null, to = null;
			NodeType node;
			DataType dt;	
			
			Connection con;
			
			ParentChildCollection pcs = new ParentChildCollection(); //temporary store for parent-child relations
			
			#region Load the nodes
			for(int k =0; k<g.Nodes.Count;k++) //loop over all serialized nodes
			{
				try
				{
					#region find out which type of shape needs to be instantiated

					node = g.Nodes[k] as NodeType;

					Type shapeType = Type.GetType(node.Type);

					if (shapeType != null)
					{
						object[] args = {this.site};
						shape = Activator.CreateInstance(shapeType, args) as ShapeBase;
					}

					#endregion
					
					#region use the attribs again to reconstruct the props				
					for(int m=0; m<node.Items.Count;m++) //loop over the serialized data
					{
						dt = node.Items[m] as DataType;
						
						if(dt.Key=="ParentNode")
						{							
							//forget the parenting, it's done in a separate loop to be sure all shapes are loaded
							if(dt.Text.Count>0) //could be if the shape is the root
								pcs.Add(new ParentChild(shape, dt.Text[0].ToString()));
							else
							{
								shape.IsRoot = true;
								abs.Root = shape;
							}

							continue;
						}
						foreach (PropertyInfo pi in shape.GetType().GetProperties()) 
						{	
							if (Attribute.IsDefined(pi, typeof(GraphDataAttribute))) 
							{
								if(pi.Name==dt.Key)
								{
									
									if(pi.GetIndexParameters().Length==0)
									{
										if(pi.PropertyType.Equals(typeof(int)))
											pi.SetValue(shape,Convert.ToInt32(dt.Text[0]),null);
										else if(pi.PropertyType.Equals(typeof(Color))) //Color is stored as an integer
											pi.SetValue(shape,Color.FromArgb(int.Parse(dt.Text[0].ToString())),null);
										else if(pi.PropertyType.Equals(typeof(string)))
											pi.SetValue(shape,(string)(dt.Text[0]),null);
										else if(pi.PropertyType.Equals(typeof(bool)))
											pi.SetValue(shape,Convert.ToBoolean(dt.Text[0]),null);	
										else if(pi.PropertyType.Equals(typeof(Guid)))
											pi.SetValue(shape,new Guid((string) dt.Text[0]),null);	
									}
									else
										pi.SetValue(shape,dt.Text,null);
									break;
								}
								
							}
						}

					}
					#endregion
					shape.Font = site.Font;
					shape.Fit();
					abs.Shapes.Add(shape);
				}
				catch(Exception exc)
				{
					Trace.WriteLine(exc.Message);
					continue;
				}

			}//loop over nodes
			#endregion

			//now for the edges;
			//every node has precisely one parent and one connection to it, unless it's the root

			for(int n=0; n<pcs.Count; n++)
			{	
				from = pcs[n].ChildShape;
				to = abs.Shapes[pcs[n].Parent];
				con = new Connection(from, to );
				abs.Connections.Add(con);
				con.site = site;
				if(pcs[n].ChildShape.visible)
					con.visible = true;
				from.connection = con;	//a lot of crossing...to make life easy really
				from.parentNode =to;
				to.childNodes.Add(from);
				
				
			}

			return abs;

		}


		/// <summary>
		/// Serializes the given shape to xml
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private NodeType SerializeNode(ShapeBase s)
		{
			Hashtable attributes = GraphDataAttribute.GetValuesOfTaggedFields(s);

			NodeType node = new NodeType();
			//node.ID = FormatID(s);		
			
			//node.Items.Add(DataTypeFromEntity(s));

			if(typeof(OvalShape).IsInstanceOfType(s))
				node.Type = "Netron.Lithium.OvalShape";
			else if(typeof(SimpleRectangle).IsInstanceOfType(s))
				node.Type = "Netron.Lithium.SimpleRectangle";
			else if(typeof(TextLabel).IsInstanceOfType(s))
				node.Type = "Netron.Lithium.TextLabel";
			
			foreach(DataType data in DataTypesFromAttributes(attributes))
			{
				node.Items.Add(data);
			}

			return node;
		}

		/// <summary>
		/// Serializes a diagram edge
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private EdgeType SerializeEdge(Connection c)
		{
			Hashtable attributes = GraphDataAttribute.GetValuesOfTaggedFields(c);

			EdgeType edge = new EdgeType();
			//edge.Source = c.From.Text;
			//edge.Target = c.To.Text;
			edge.From = c.From.UID.ToString();
			edge.To = c.To.UID.ToString();			
			//since there is only one type of edge we don't need the next one
			//edge.Data.Add(DataTypeFromEntity(c));

			foreach(DataType dt in DataTypesFromAttributes(attributes))
			{
				edge.Data.Add(dt);
			}

			return edge;
		}
		/// <summary>
		/// Returns the UID in string format of the given entity
		/// </summary>
		/// <param name="e">an Entity object</param>
		/// <returns>the UID as string</returns>
		private string FormatID(Entity e)
		{
			if(e==null)
				return string.Empty;
			else
				return String.Format("e{0}",e.UID.ToString());
		}
	
		/// <summary>
		/// Converts the set of key/Values to a DataType array
		/// </summary>
		/// <param name="attributes">an Hastable of key/value pairs</param>
		/// <returns>An array of DataTypes </returns>
		private DataType[] DataTypesFromAttributes(Hashtable attributes)
		{
			DataType[] dts = new DataType[attributes.Count];
			
			int i = 0;
			foreach ( DictionaryEntry de in attributes )
			{
				dts[i] = new DataType();
				dts[i].Key = de.Key.ToString();
				if (de.Value != null)
				{
					//the color is a bit different
					if(typeof(Color).IsInstanceOfType(de.Value))
					{
						int val = ((Color) de.Value).ToArgb();
						dts[i].Text.Add(val.ToString());
					}
					else if(typeof(ShapeBase).IsInstanceOfType(de.Value))
					{
						dts[i].Text.Add((de.Value as ShapeBase).UID.ToString());
					}
					else if(typeof(Guid).IsInstanceOfType(de.Value))
					{
						dts[i].Text.Add(((Guid) de.Value ).ToString());
					}
					else
						dts[i].Text.Add(de.Value.ToString());
				}
				if ( !keyList.Contains(de.Key.ToString()))
					keyList.Add(de.Key.ToString(),de.Value);
				++i;
			}
			return dts;

		}
		/// <summary>
		/// Returns qualified type name of o
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		private string GetTypeQualifiedName(Object o)
		{
			if (o==null)
				throw new ArgumentNullException("o");
			return this.GetTypeQualifiedName(o.GetType());
		}

		/// <summary>
		/// Creates the name of a type qualified by the display name of its assembly.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private string GetTypeQualifiedName(Type t)
		{
			return Assembly.CreateQualifiedName(
				t.Assembly.FullName,
				t.FullName
				);
		}

		private Type ToType(string text)
		{
			return Type.GetType(text,true);
		}

		private bool ToBool(string text)
		{
			return bool.Parse(text);
		}
		#endregion
	}
}
