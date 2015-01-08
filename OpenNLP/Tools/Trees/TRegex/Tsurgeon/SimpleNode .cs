using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class SimpleNode : INode
    {
        protected INode parent;
        protected INode[] children;
        protected int id;
        protected TsurgeonParser parser;

        public SimpleNode(int i)
        {
            id = i;
        }

        public SimpleNode(TsurgeonParser p, int i) : this(i)
        {
            parser = p;
        }

        public void JjtOpen()
        {
        }

        public void JjtClose()
        {
        }

        public void JjtSetParent(INode n)
        {
            parent = n;
        }

        public INode JjtGetParent()
        {
            return parent;
        }

        public void JjtAddChild(INode n, int i)
        {
            if (children == null)
            {
                children = new INode[i + 1];
            }
            else if (i >= children.Length)
            {
                var c = new INode[i + 1];
                Array.Copy(children, 0, c, 0, children.Length);
                children = c;
            }
            children[i] = n;
        }

        public INode JjtGetChild(int i)
        {
            return children[i];
        }

        public int JjtGetNumChildren()
        {
            return (children == null) ? 0 : children.Length;
        }

        /* 
         * You can override these two methods in subclasses of SimpleNode to
         * customize the way the node appears when the tree is dumped.
         * If your output uses more than one line you should override
         * ToString(string), otherwise overriding ToString() is probably all
         * you need to do.
         */

        public override string ToString()
        {
            return TsurgeonParserTreeConstants.JjtNodeNames[id];
        }

        public string ToString(string prefix)
        {
            return prefix + ToString();
        }

        // Override this method if you want to customize how the node dumps out its children
        public void Dump(string prefix)
        {
            if (children != null)
            {
                for (int i = 0; i < children.Length; ++i)
                {
                    var n = (SimpleNode) children[i];
                    if (n != null)
                    {
                        n.Dump(prefix + " ");
                    }
                }
            }
        }

    }
}