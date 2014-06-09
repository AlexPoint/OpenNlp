using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace Netron.Lithium
{
	/// <summary>
	/// This is the root of the XML serialization class.
	/// It acts as a kind of proxy class between the XML and the actual GraphAbstract.
	/// </summary>
		[XmlType(IncludeInSchema=true, TypeName="graph.type")]
		[XmlRoot(ElementName="NetronLightGraph", IsNullable=false, DataType="")]
		public class GraphType 
		{
			#region Fields
			private GraphDataCollection nodes = new GraphDataCollection();
			private GraphDataCollection edges = new GraphDataCollection();
		
			private string description;
			private string id;
			#endregion

			#region Properties
			/// <summary>
			/// Gets or sets the description of the diagram
			/// </summary>
			[XmlElement(ElementName="Description")]
			public virtual string Description 
			{
				get 
				{
					return this.description;
				}
				set 
				{
					this.description = value;
				}
			}
        
        
        
			
			/// <summary>
			/// Gets or sets the node collection
			/// </summary>
			[XmlElement(ElementName="Node", Type=typeof(NodeType))]			
			public virtual GraphDataCollection Nodes 
			{
				get 
				{
					return this.nodes;
				}
				set 
				{
					this.nodes = value;
				}
			}

			/// <summary>
			/// Gets or sets the edge collection
			/// </summary>
			[XmlElement(ElementName="Edge", Type=typeof(EdgeType))]					
			public virtual GraphDataCollection Edges 
			{
				get 
				{
					return this.edges;
				}
				set 
				{
					this.edges = value;
				}
			}
        
        
        
			/// <summary>
			/// Gets or sets the id
			/// </summary>
			[XmlAttribute(AttributeName="id")]
			public virtual string ID 
			{
				get 
				{
					return this.id;
				}
				set 
				{
					this.id = value;
				}
			}
        
        
        
		
  
			#endregion
		}
	}


