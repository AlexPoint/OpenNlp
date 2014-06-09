
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
    
namespace Netron.Lithium
{    
    
    /// <summary>
    /// Whatever data type the property of an entity is, this can hold it
    /// </summary>
	[XmlType(IncludeInSchema=true, TypeName="data.type")]
	[XmlRoot(ElementName="Data", IsNullable=false, DataType="")]
	public class DataType 
	{

		#region Fields
		/// <summary>
		/// a text collection
		/// </summary>
		private GraphDataCollection text = new GraphDataCollection();

		/// <summary>
		/// the key of the property
		/// </summary>
		private string key;

		/// <summary>
		/// the id of the property
		/// </summary>
		private string id;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the key
		/// </summary>
		[XmlAttribute(AttributeName="key")]
		public virtual string Key 
		{
			get 
			{
				return this.key;
			}
			set 
			{
				this.key = value;
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
        
        
        /// <summary>
        /// Get or sets the text of the property
        /// </summary>
		[XmlText(Type=typeof(string))]
		public virtual GraphDataCollection Text 
		{
			get 
			{
				return this.text;
			}
			set 
			{
				this.text = value;
			}
		}
		#endregion        

	}
}
