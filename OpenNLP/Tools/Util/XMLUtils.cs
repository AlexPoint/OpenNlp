using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNLP.Tools.Util
{
    /**
 * Provides some utilities for dealing with XML files, both by properly
 * parsing them and by using the methods of a desperate Perl hacker.
 *
 * @author Teg Grenager
 */
    public class XMLUtils
    {

        public static string XmlEscape(string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }
    }
}
