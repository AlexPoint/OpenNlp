
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
    
    
namespace Netron.Lithium
{    
    /// <summary>
    /// Stores the edge properties in the form of an XML element
    /// </summary>
	[XmlType(IncludeInSchema=true, TypeName="edge.type")]
	[XmlRoot(ElementName="Edge", IsNullable=false, DataType="")]
	public class EdgeType 
	{
		#region Fields		
		/// <summary>
		/// the set of subnodes under the XML element
		/// </summary>
		private GraphDataCollection data = new GraphDataCollection();

		/// <summary>
		/// the uid of the shape where the edge originates
		/// </summary>
		private string from;
		/// <summary>
		/// the uid of the shape where the edge ends
		/// </summary>
		private string to;		
		
		#endregion 

		#region Properties
		
        /// <summary>
        /// Generic data collection of properties
        /// </summary>
		[XmlElement(ElementName="Data", Type=typeof(DataType))]
		public virtual GraphDataCollection Data 
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
        /// The UID of the shape where the connection starts
        /// </summary>
		[XmlAttribute(AttributeName="From")]
		public virtual string From 
		{
			get 
			{
				return this.from;
			}
			set 
			{
				this.from = value;
			}
		}
        
        
        /// <summary>
        /// The UID of the shape where the connection ends
        /// </summary>
		[XmlAttribute(AttributeName="To")]
		public virtual string To 
		{
			get 
			{
				return this.to;
			}
			set 
			{
				this.to = value;
			}
		}
		#endregion  
   
	}
}
