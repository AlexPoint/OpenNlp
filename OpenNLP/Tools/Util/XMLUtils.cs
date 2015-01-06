using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNLP.Tools.Util
{
    /// <summary>
    /// Provides some utilities for dealing with XML files, both by properly 
    /// parsing them and by using the methods of a desperate Perl hacker.
    /// </summary>
    public class XmlUtils
    {

        /// <summary>
        /// Escape an unescaped xml string
        /// </summary>
        public static string XmlEscape(string unescaped)
        {
            var doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }
    }
}