
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
    
    
namespace Netron.Lithium
{    
    /// <summary>
    /// The proxy class between the shape and the serialized XML
    /// </summary>
	[XmlType(IncludeInSchema=true, TypeName="node.type")]
	[XmlRoot(ElementName="Node", IsNullable=false, DataType="")]	
	public class NodeType 
	{
		#region Fields
		private GraphDataCollection data = new GraphDataCollection();		
		private string id;
		private string type;
		#endregion

		#region Properties		
		/// <summary>
		/// Generic collection of properties
		/// </summary>
		[XmlElement(ElementName="Data", Type=typeof(DataType))]	
		public virtual GraphDataCollection Items 
		{
			get 
			{
				return this.data;
			}
			set 
			{
				this.data = value;
			}
		}
        
        
        /// <summary>
        /// The Uid of the shape
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

		/// <summary>
		/// The Uid of the shape
		/// </summary>
		[XmlAttribute(AttributeName="Type")]
		public virtual string Type 
		{
			get 
			{
				return this.type;
			}
			set 
			{
				this.type = value;
			}
		}


		#endregion
 
	}
}
