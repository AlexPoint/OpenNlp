using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    
    public class JjtTsurgeonParserState
    {
        private readonly List<INode> nodes;
        private readonly List<int> marks;

        /// <summary>number of nodes on stack</summary>
        private int sp;
        /// <summary>current mark</summary>
        private int mk;
        private bool node_created;

        public JjtTsurgeonParserState()
        {
            nodes = new List<INode>();
            marks = new List<int>();
            sp = 0;
            mk = 0;
        }

        /// <summary>
        /// Determines whether the current node was actually closed and pushed.
        /// This should only be called in the final user action of a node scope.
        /// </summary>
        public bool NodeCreated()
        {
            return node_created;
        }

        /// <summary>
        /// Call this to reinitialize the node stack.
        /// It is called automatically by the parser's ReInit() method.
        /// </summary>
        public void Reset()
        {
            nodes.Clear();
            marks.Clear();
            sp = 0;
            mk = 0;
        }

        /// <summary>
        /// Returns the root node of the AST.  It only makes sense to call this after a successful parse.
        /// </summary>
        public INode RootNode()
        {
            return nodes[0];
        }

        /// <summary>
        /// Pushes a node on to the stack
        /// </summary>
        public void PushNode(INode n)
        {
            nodes.Add(n);
            ++sp;
        }

        /// <summary>
        /// Returns the node on the top of the stack, and remove it from the stack.
        /// </summary>
        public INode PopNode()
        {
            if (--sp < mk)
            {
                //mk = marks.remove(marks.size() - 1);
                mk = marks.Last();
                marks.Remove(mk);
            }
            var lNode = nodes.Last();
            nodes.Remove(lNode);
            return lNode;
            //return nodes.remove(nodes.size() - 1);
        }

        /// <summary>
        /// Returns the node currently on the top of the stack.
        /// </summary>
        public INode PeekNode()
        {
            return nodes[nodes.Count - 1];
        }

        /// <summary>
        /// Returns the number of children on the stack in the current node scope.
        /// </summary>
        public int NodeArity()
        {
            return sp - mk;
        }


        public void ClearNodeScope(INode n)
        {
            while (sp > mk)
            {
                PopNode();
            }
            //mk = marks.remove(marks.size()-1);
            mk = marks[marks.Count - 1];
            marks.Remove(mk);
        }


        public void OpenNodeScope(INode n)
        {
            marks.Add(mk);
            mk = sp;
            n.JjtOpen();
        }

        /// <summary>
        /// A definite node is constructed from a specified number of children.
        /// That number of nodes are popped from the stack and
        /// made the children of the definite node.
        /// Then the definite node is pushed on to the stack.
        /// </summary>
        public void CloseNodeScope(INode n, int num)
        {
            //mk = marks.remove(marks.size()-1);
            mk = marks[marks.Count - 1];
            marks.Remove(mk);
            while (num-- > 0)
            {
                INode c = PopNode();
                c.JjtSetParent(n);
                n.JjtAddChild(c, num);
            }
            n.JjtClose();
            PushNode(n);
            node_created = true;
        }


        /// <summary>
        /// A conditional node is constructed if its condition is true.
        /// All the nodes that have been pushed since the node was opened are
        /// made children of the conditional node, which is then pushed
        /// on to the stack.  If the condition is false the node is not
        /// constructed and they are left on the stack.
        /// </summary>
        public void CloseNodeScope(INode n, bool condition)
        {
            if (condition)
            {
                int a = NodeArity();
                //mk = marks.remove(marks.size()-1);
                mk = marks.Last();
                marks.Remove(mk);
                while (a-- > 0)
                {
                    INode c = PopNode();
                    c.JjtSetParent(n);
                    n.JjtAddChild(c, a);
                }
                n.JjtClose();
                PushNode(n);
                node_created = true;
            }
            else
            {
                //mk = marks.remove(marks.size()-1);
                mk = marks.Last();
                marks.Remove(mk);
                node_created = false;
            }
        }
    }
}