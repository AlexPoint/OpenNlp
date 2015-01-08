using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    /// <summary>
    /// All AST nodes must implement this interface.  It provides basic
    /// machinery for constructing the parent and child relationships
    /// between nodes.
    /// </summary>
    public interface INode
    {

        /// <summary>
        /// This method is called after the node has been made the current node.
        /// It indicates that child nodes can now be added to it
        /// </summary>
        void JjtOpen();

        /// <summary>
        /// This method is called after all the child nodes have been added
        /// </summary>
        void JjtClose();

        /// <summary>
        /// This pair of methods are used to inform the node of its parent
        /// </summary>
        void JjtSetParent(INode n);

        INode JjtGetParent();

        /// <summary>
        /// This method tells the node to add its argument to the node's list of children
        /// </summary>
        void JjtAddChild(INode n, int i);

        /// <summary>
        /// This method returns a child node.  The children are numbered from zero, left to right
        /// </summary>
        INode JjtGetChild(int i);

        /// <summary>
        /// Return the number of children the node has
        /// </summary>
        int JjtGetNumChildren();
    }
}