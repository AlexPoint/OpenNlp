using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    public class SimpleNode : Node
    {
        protected Node parent;
        protected Node[] children;
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

        public void jjtOpen()
        {
        }

        public void jjtClose()
        {
        }

        public void jjtSetParent(Node n)
        {
            parent = n;
        }

        public Node jjtGetParent()
        {
            return parent;
        }

        public void jjtAddChild(Node n, int i)
        {
            if (children == null)
            {
                children = new Node[i + 1];
            }
            else if (i >= children.Length)
            {
                Node[] c = new Node[i + 1];
                Array.Copy(children, 0, c, 0, children.Length);
                children = c;
            }
            children[i] = n;
        }

        public Node jjtGetChild(int i)
        {
            return children[i];
        }

        public int jjtGetNumChildren()
        {
            return (children == null) ? 0 : children.Length;
        }

        /* You can override these two methods in subclasses of SimpleNode to
     customize the way the node appears when the tree is dumped.  If
     your output uses more than one line you should override
     ToString(String), otherwise overriding ToString() is probably all
     you need to do. */

        //@Override
        public override String ToString()
        {
            return TsurgeonParserTreeConstants.jjtNodeName[id];
        }

        public String ToString(String prefix)
        {
            return prefix + ToString();
        }

        /* Override this method if you want to customize how the node dumps
     out its children. */

        public void dump(String prefix)
        {
            //System.out.println(ToString(prefix));
            if (children != null)
            {
                for (int i = 0; i < children.Length; ++i)
                {
                    SimpleNode n = (SimpleNode) children[i];
                    if (n != null)
                    {
                        n.dump(prefix + " ");
                    }
                }
            }
        }

    }
}