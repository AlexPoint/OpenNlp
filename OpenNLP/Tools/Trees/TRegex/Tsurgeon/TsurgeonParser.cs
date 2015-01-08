using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;

namespace OpenNLP.Tools.Trees.TRegex.Tsurgeon
{
    public class TsurgeonParser : TsurgeonParserTreeConstants
    {
        protected JjtTsurgeonParserState JjTree = new JjtTsurgeonParserState();
        private readonly ITreeFactory treeFactory = new LabeledScoredTreeFactory();

        
        // TODO: this is wasteful in terms of creating TsurgeonPatternRoot.
        // Should separate that out into another production
        public TsurgeonPatternRoot Root()
        {
            /*@bgen(jjtree) Root */
            var jjtn000 = new SimpleNode(JjtRoot);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            List<TsurgeonPattern> results = null;
            try
            {
                TsurgeonPattern result;
                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                {
                    case Delete:
                    case Prune:
                    case Relabel:
                    case Excise:
                    case Insert:
                    case Move:
                    case Replace:
                    case CreateSubtree:
                    case Adjoin:
                    case AdjoinToHead:
                    case AdjoinToFoot:
                    case Coindex:
                    {
                        result = Operation();
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return new TsurgeonPatternRoot(result);
                    }
                    default:
                        jj_la1[1] = jj_gen;
                        Token name;
                        if (Jj_2_1(2))
                        {
                            Jj_consume_token(If);
                            Jj_consume_token(Exists);
                            name = Jj_consume_token(Name);
                            result = Root();
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new TsurgeonPatternRoot(new IfExistsNode(name.Image, false, result.children));
                        }
                        else if (Jj_2_2(2))
                        {
                            Jj_consume_token(If);
                            Jj_consume_token(Not);
                            Jj_consume_token(Exists);
                            name = Jj_consume_token(Name);
                            result = Root();
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new TsurgeonPatternRoot(new IfExistsNode(name.Image, true, result.children));
                        }
                        else
                        {
                            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                            {
                                case OpenBracket:
                                {
                                    //label_1:
                                    while (true)
                                    {
                                        Jj_consume_token(OpenBracket);
                                        result = Root();
                                        Jj_consume_token(CloseBracket);
                                        if (results == null)
                                        {
                                            results = new List<TsurgeonPattern>();
                                        }
                                        foreach (TsurgeonPattern child in result.children)
                                        {
                                            results.Add(child);
                                        }
                                        switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                        {
                                            case OpenBracket:
                                            {
                                                ;
                                                break;
                                            }
                                            default:
                                                jj_la1[0] = jj_gen;
                                                goto post_label_1;
                                        }
                                    }
                                    post_label_1:
                                    {
                                        JjTree.CloseNodeScope(jjtn000, true);
                                        jjtc000 = false;
                                        var array = new TsurgeonPattern[results.Count];
                                        return new TsurgeonPatternRoot(results.ToArray());
                                    }
                                }
                                default:
                                    jj_la1[2] = jj_gen;
                                    Jj_consume_token(-1);
                                    throw new ParseException();
                            }
                        }
                }
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public TsurgeonPattern Operation()
        {
            /*@bgen(jjtree) Operation */
            var jjtn000 = new SimpleNode(JjtOperation);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            Token operatorToken;
            Token hashInt;
            try
            {
                TsurgeonPattern child1;
                TsurgeonPattern child2 = null;
                List<TsurgeonPattern> nodeSelections = null;
                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                {
                    case Delete:
                    {
                        operatorToken = Jj_consume_token(Delete);
                        nodeSelections = NodeSelectionList(new List<TsurgeonPattern>());
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return new DeleteNode(nodeSelections);
                    }
                    case Prune:
                    {
                        operatorToken = Jj_consume_token(Prune);
                        nodeSelections = NodeSelectionList(new List<TsurgeonPattern>());
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return new PruneNode(nodeSelections);
                    }
                    case Excise:
                    {
                        operatorToken = Jj_consume_token(Excise);
                        child1 = NodeSelection();
                        child2 = NodeSelection();
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return new ExciseNode(child1, child2);
                    }
                    default:
                        jj_la1[3] = jj_gen;
                        Token newLabel = null;
                        if (Jj_2_3(3))
                        {
                            operatorToken = Jj_consume_token(Relabel);
                            child1 = NodeSelection();
                            newLabel = Jj_consume_token(Identifier);
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new RelabelNode(child1, newLabel.Image);
                        }
                        else if (Jj_2_4(3))
                        {
                            operatorToken = Jj_consume_token(Relabel);
                            child1 = NodeSelection();
                            newLabel = Jj_consume_token(Quotex);
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new RelabelNode(child1, newLabel.Image);
                        }
                        else if (Jj_2_5(3))
                        {
                            operatorToken = Jj_consume_token(Relabel);
                            child1 = NodeSelection();
                            Token regex = Jj_consume_token(Regex);
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new RelabelNode(child1, regex.Image);
                        }
                        else if (Jj_2_6(3))
                        {
                            operatorToken = Jj_consume_token(Relabel);
                            child1 = NodeSelection();
                            newLabel = Jj_consume_token(GeneralRelabel);
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new RelabelNode(child1, newLabel.Image);
                        }
                        else if (Jj_2_7(3))
                        {
                            operatorToken = Jj_consume_token(Replace);
                            child1 = NodeSelection();
                            child2 = NodeSelection();
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new ReplaceNode(child1, new[] {child2});
                        }
                        else if (Jj_2_8(3))
                        {
                            operatorToken = Jj_consume_token(Replace);
                            child1 = NodeSelection();
                            List<AuxiliaryTree> treeList = TreeList(false);
                            JjTree.CloseNodeScope(jjtn000, true);
                            jjtc000 = false;
                            return new ReplaceNode(child1, treeList);
                        }
                        else
                        {
                            TreeLocation loc = null;
                            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                            {
                                case Move:
                                {
                                    operatorToken = Jj_consume_token(Move);
                                    child1 = NodeSelection();
                                    loc = Location();
                                    JjTree.CloseNodeScope(jjtn000, true);
                                    jjtc000 = false;
                                    return new MoveNode(child1, loc);
                                }
                                default:
                                    jj_la1[4] = jj_gen;
                                    if (Jj_2_9(3))
                                    {
                                        operatorToken = Jj_consume_token(Insert);
                                        child1 = NodeSelection();
                                        loc = Location();
                                        JjTree.CloseNodeScope(jjtn000, true);
                                        jjtc000 = false;
                                        return new InsertNode(child1, loc);
                                    }
                                    else
                                    {
                                        AuxiliaryTree tree = null;
                                        if (Jj_2_10(3))
                                        {
                                            operatorToken = Jj_consume_token(Insert);
                                            tree = TreeRoot(false);
                                            loc = Location();
                                            JjTree.CloseNodeScope(jjtn000, true);
                                            jjtc000 = false;
                                            return new InsertNode(tree, loc);
                                        }
                                        else
                                        {
                                            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                            {
                                                case CreateSubtree:
                                                {
                                                    operatorToken = Jj_consume_token(CreateSubtree);
                                                    tree = TreeRoot(false);
                                                    nodeSelections = NodeSelectionList(new List<TsurgeonPattern>());
                                                    JjTree.CloseNodeScope(jjtn000, true);
                                                    jjtc000 = false;
                                                    if (nodeSelections.Count == 1)
                                                    {
                                                        return new CreateSubtreeNode(nodeSelections[0], tree);
                                                    }
                                                    else if (nodeSelections.Count == 2)
                                                    {
                                                        return new CreateSubtreeNode(nodeSelections[0],
                                                            nodeSelections[1], tree);
                                                    }
                                                    else
                                                    {
                                                        throw new ParseException(
                                                            "Illegal number of nodes given to createSubtree (" +
                                                            nodeSelections.Count + ")");
                                                    }
                                                }
                                                case Adjoin:
                                                {
                                                    operatorToken = Jj_consume_token(Adjoin);
                                                    tree = TreeRoot(true);
                                                    child1 = NodeSelection();
                                                    JjTree.CloseNodeScope(jjtn000, true);
                                                    jjtc000 = false;
                                                    return new AdjoinNode(tree, child1);
                                                }
                                                case AdjoinToHead:
                                                {
                                                    operatorToken = Jj_consume_token(AdjoinToHead);
                                                    tree = TreeRoot(true);
                                                    child1 = NodeSelection();
                                                    JjTree.CloseNodeScope(jjtn000, true);
                                                    jjtc000 = false;
                                                    return new AdjoinToHeadNode(tree, child1);
                                                }
                                                case AdjoinToFoot:
                                                {
                                                    operatorToken = Jj_consume_token(AdjoinToFoot);
                                                    tree = TreeRoot(true);
                                                    child1 = NodeSelection();
                                                    JjTree.CloseNodeScope(jjtn000, true);
                                                    jjtc000 = false;
                                                    return new AdjoinToFootNode(tree, child1);
                                                }
                                                case Coindex:
                                                {
                                                    operatorToken = Jj_consume_token(Coindex);
                                                    nodeSelections = NodeSelectionList(new List<TsurgeonPattern>());
                                                    JjTree.CloseNodeScope(jjtn000, true);
                                                    jjtc000 = false;
                                                    return new CoindexNodes(nodeSelections.ToArray());
                                                }
                                                default:
                                                    jj_la1[5] = jj_gen;
                                                    Jj_consume_token(-1);
                                                    throw new ParseException();
                                            }
                                        }
                                    }
                            }
                        }
                }
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public TreeLocation Location()
        {
            /*@bgen(jjtree) Location */
            var jjtn000 = new SimpleNode(JjtLocation);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            try
            {
                Token rel = Jj_consume_token(LocationRelation);
                TsurgeonPattern child = NodeSelection();
                JjTree.CloseNodeScope(jjtn000, true);
                jjtc000 = false;
                return new TreeLocation(rel.Image, child);
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public List<TsurgeonPattern> NodeSelectionList(List<TsurgeonPattern> l)
        {
            /*@bgen(jjtree) NodeSelectionList */
            var jjtn000 = new SimpleNode(JjtNodeSelectionList);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            try
            {
                TsurgeonPattern result = NodeSelection();
                l.Add(result);
                //label_2:
                while (true)
                {
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case Identifier:
                        {
                            ;
                            break;
                        }
                        default:
                            jj_la1[6] = jj_gen;
                            //break label_2;
                            goto post_label_2;
                    }
                    result = NodeSelection();
                    l.Add(result);
                }
                post_label_2:
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                    jjtc000 = false;
                    return l;
                }
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        // TODO: what does this next comment mean?
        // we'll also put in a way to use a SELECTION with a list of nodes.
        public TsurgeonPattern NodeSelection()
        {
            /*@bgen(jjtree) NodeSelection */
            var jjtn000 = new SimpleNode(JjtNodeSelection);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            try
            {
                TsurgeonPattern result = NodeName();
                JjTree.CloseNodeScope(jjtn000, true);
                jjtc000 = false;
                return result;
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public TsurgeonPattern NodeName()
        {
            /*@bgen(jjtree) NodeName */
            var jjtn000 = new SimpleNode(JjtNodeName);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            Token t;
            try
            {
                t = Jj_consume_token(Identifier);
                JjTree.CloseNodeScope(jjtn000, true);
                jjtc000 = false;
                return new FetchNode(t.Image);
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public List<AuxiliaryTree> TreeList(bool requiresFoot)
        {
            /*@bgen(jjtree) TreeList */
            var jjtn000 = new SimpleNode(JjtTreeList);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            var trees = new List<AuxiliaryTree>();
            try
            {
                AuxiliaryTree tree = TreeRoot(requiresFoot);
                trees.Add(tree);
                //label_3:
                while (true)
                {
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case Identifier:
                        case TreeNodeTerminalLabel:
                        case TreeNodeNonterminalLabel:
                        {
                            ;
                            break;
                        }
                        default:
                            jj_la1[7] = jj_gen;
                            //break label_3;
                            goto post_label_3;
                    }
                    tree = TreeRoot(requiresFoot);
                    trees.Add(tree);
                }
                post_label_3:
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                    jjtc000 = false;
                    return trees;
                }
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        // the argument says whether there must be a foot node on the aux tree.
        public AuxiliaryTree TreeRoot(bool requiresFoot)
        {
            /*@bgen(jjtree) TreeRoot */
            var jjtn000 = new SimpleNode(JjtTreeRoot);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            Tree t;
            try
            {
                t = TreeNode();
                JjTree.CloseNodeScope(jjtn000, true);
                jjtc000 = false;
                return new AuxiliaryTree(t, requiresFoot);
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public Tree TreeNode()
        {
            /*@bgen(jjtree) TreeNode */
            var jjtn000 = new SimpleNode(JjtTreeNode);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            try
            {
                Token label;
                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                {
                    case TreeNodeNonterminalLabel:
                    {
                        label = Jj_consume_token(TreeNodeNonterminalLabel);
                        List<Tree> dtrs = TreeDtrs(new List<Tree>());
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return treeFactory.NewTreeNode(label.Image.Substring(1), dtrs);
                    }
                    case TreeNodeTerminalLabel:
                    {
                        label = Jj_consume_token(TreeNodeTerminalLabel);
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return treeFactory.NewTreeNode(label.Image, new List<Tree>());
                    }
                    case Identifier:
                    {
                        label = Jj_consume_token(Identifier);
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return treeFactory.NewTreeNode(label.Image, new List<Tree>());
                    }
                    default:
                        jj_la1[8] = jj_gen;
                        Jj_consume_token(-1);
                        throw new ParseException();
                }
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        public List<Tree> TreeDtrs(List<Tree> dtrs)
        {
            /*@bgen(jjtree) TreeDtrs */
            var jjtn000 = new SimpleNode(JjtTreeDtrs);
            bool jjtc000 = true;
            JjTree.OpenNodeScope(jjtn000);
            try
            {
                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                {
                    case Identifier:
                    case TreeNodeTerminalLabel:
                    case TreeNodeNonterminalLabel:
                    {
                        Tree tree = TreeNode();
                        TreeDtrs(dtrs);
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        dtrs.Insert(0, tree);
                        return dtrs;
                    }
                    case CloseParen:
                    {
                        Jj_consume_token(CloseParen);
                        JjTree.CloseNodeScope(jjtn000, true);
                        jjtc000 = false;
                        return dtrs;
                    }
                    default:
                        jj_la1[9] = jj_gen;
                        Jj_consume_token(-1);
                        throw new ParseException();
                }
            }
            catch (Exception jjte000)
            {
                if (jjtc000)
                {
                    JjTree.ClearNodeScope(jjtn000);
                    jjtc000 = false;
                }
                else
                {
                    JjTree.PopNode();
                }
                if (jjte000 is SystemException)
                {
                    throw jjte000;
                }
                if (jjte000 is ParseException)
                {
                    throw jjte000;
                }
                throw jjte000;
            }
            finally
            {
                if (jjtc000)
                {
                    JjTree.CloseNodeScope(jjtn000, true);
                }
            }
        }

        private bool Jj_2_1(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_1();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(0, xla);
            }
        }

        private bool Jj_2_2(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_2();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(1, xla);
            }
        }

        private bool Jj_2_3(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_3();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(2, xla);
            }
        }

        private bool Jj_2_4(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_4();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(3, xla);
            }
        }

        private bool Jj_2_5(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_5();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(4, xla);
            }
        }

        private bool Jj_2_6(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_6();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(5, xla);
            }
        }

        private bool Jj_2_7(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_7();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(6, xla);
            }
        }

        private bool Jj_2_8(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_8();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(7, xla);
            }
        }

        private bool Jj_2_9(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_9();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(8, xla);
            }
        }

        private bool Jj_2_10(int xla)
        {
            jj_la = xla;
            jj_lastpos = jj_scanpos = token;
            try
            {
                return !Jj_3_10();
            }
            catch (LookaheadSuccess ls)
            {
                return true;
            }
            finally
            {
                Jj_save(9, xla);
            }
        }

        private bool Jj_3R_15()
        {
            if (Jj_scan_token(CloseParen)) return true;
            return false;
        }

        private bool Jj_3R_8()
        {
            if (Jj_scan_token(Identifier)) return true;
            return false;
        }

        private bool Jj_3R_14()
        {
            if (Jj_3R_9()) return true;
            return false;
        }

        private bool Jj_3R_13()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_14())
            {
                jj_scanpos = xsp;
                if (Jj_3R_15()) return true;
            }
            return false;
        }

        private bool Jj_3R_4()
        {
            if (Jj_3R_8()) return true;
            return false;
        }

        private bool Jj_3R_12()
        {
            if (Jj_scan_token(Identifier)) return true;
            return false;
        }

        private bool Jj_3_10()
        {
            if (Jj_scan_token(Insert)) return true;
            if (Jj_3R_7()) return true;
            if (Jj_3R_6()) return true;
            return false;
        }

        private bool Jj_3R_11()
        {
            if (Jj_scan_token(TreeNodeTerminalLabel)) return true;
            return false;
        }

        private bool Jj_3_9()
        {
            if (Jj_scan_token(Insert)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_3R_6()) return true;
            return false;
        }

        private bool Jj_3R_9()
        {
            Token xsp;
            xsp = jj_scanpos;
            if (Jj_3R_10())
            {
                jj_scanpos = xsp;
                if (Jj_3R_11())
                {
                    jj_scanpos = xsp;
                    if (Jj_3R_12()) return true;
                }
            }
            return false;
        }

        private bool Jj_3R_10()
        {
            if (Jj_scan_token(TreeNodeNonterminalLabel)) return true;
            if (Jj_3R_13()) return true;
            return false;
        }

        private bool Jj_3_8()
        {
            if (Jj_scan_token(Replace)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_3R_5()) return true;
            return false;
        }

        private bool Jj_3_7()
        {
            if (Jj_scan_token(Replace)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_3R_4()) return true;
            return false;
        }

        private bool Jj_3R_7()
        {
            if (Jj_3R_9()) return true;
            return false;
        }

        private bool Jj_3_2()
        {
            if (Jj_scan_token(If)) return true;
            if (Jj_scan_token(Not)) return true;
            return false;
        }

        private bool Jj_3_6()
        {
            if (Jj_scan_token(Relabel)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_scan_token(GeneralRelabel)) return true;
            return false;
        }

        private bool Jj_3_1()
        {
            if (Jj_scan_token(If)) return true;
            if (Jj_scan_token(Exists)) return true;
            return false;
        }

        private bool Jj_3_5()
        {
            if (Jj_scan_token(Relabel)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_scan_token(Regex)) return true;
            return false;
        }

        private bool Jj_3_4()
        {
            if (Jj_scan_token(Relabel)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_scan_token(Quotex)) return true;
            return false;
        }

        private bool Jj_3R_6()
        {
            if (Jj_scan_token(LocationRelation)) return true;
            return false;
        }

        private bool Jj_3_3()
        {
            if (Jj_scan_token(Relabel)) return true;
            if (Jj_3R_4()) return true;
            if (Jj_scan_token(Identifier)) return true;
            return false;
        }

        private bool Jj_3R_5()
        {
            if (Jj_3R_7()) return true;
            return false;
        }

        /// <summary>Generated Token Manager</summary>
        public TsurgeonParserTokenManager token_source;
        private SimpleCharStream jj_input_stream;
        /// <summary>Current token</summary>
        public Token token;
        /// <summary>Next token</summary>
        public Token jj_nt;
        private int jj_ntk;
        private Token jj_scanpos, jj_lastpos;
        private int jj_la;
        private int jj_gen;
        private readonly int[] jj_la1 = new int[10];
        //private static int[] jj_la1_0;
        private static readonly int[] jj_la1_0 = new int[]
        {
            0x20, 0x1ffe00, 0x20, 0x1600, 0x4000, 0x1f0000, 0x2000000, unchecked ((int) 0xc2000000),
            unchecked ((int) 0xc2000000), unchecked ((int) 0xc2000000),
        };

        //private static int[] jj_la1_1;
        private static readonly int[] jj_la1_1 = new int[] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1,};

        /*private static 
        {
            jj_la1_init_0();
            jj_la1_init_1();
        }*/

        /*private static void jj_la1_init_0()
        {
            jj_la1_0 = new int[]
            {0x20, 0x1ffe00, 0x20, 0x1600, 0x4000, 0x1f0000, 0x2000000, 0xc2000000, 0xc2000000, 0xc2000000,};
        }*/

        /*private static void jj_la1_init_1()
        {
            jj_la1_1 = new int[] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x1,};
        }*/

        private readonly JjCalls[] jj_2_rtns = new JjCalls[10];
        private bool jj_rescan = false;
        private int jj_gc = 0;

        /// <summary>Constructor with Stream</summary>
        public TsurgeonParser( /*java.io.InputStream*/ Stream stream) :
            this(stream, null)
        {
        }

        /// <summary>Constructor with Stream</summary>
        public TsurgeonParser( /*java.io.InputStream*/ Stream stream, string encoding)
        {
            try
            {
                jj_input_stream = new SimpleCharStream(stream, encoding, 1, 1);
            }
            catch ( /*java.io.UnsupportedEncodingException*/Exception e)
            {
                throw new SystemException(e.Message);
            }
            token_source = new TsurgeonParserTokenManager(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 10; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /** Reinitialise. */
        /*public void ReInit(java.io.InputStream stream) {
             ReInit(stream, null);
          }*/
                /** Reinitialise. */
                /*public void ReInit(java.io.InputStream stream, string encoding) {
            try { jj_input_stream.ReInit(stream, encoding, 1, 1); } catch(java.io.UnsupportedEncodingException e) { throw new SystemException(e); }
            token_source.ReInit(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jjtree.reset();
            jj_gen = 0;
            for (int i = 0; i < 10; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();
          }*/

        /// <summary>Constructor with TextReader</summary>
        public TsurgeonParser(TextReader stream)
        {
            jj_input_stream = new SimpleCharStream(stream, 1, 1);
            token_source = new TsurgeonParserTokenManager(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 10; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /** Reinitialise. */
        /*public void ReInit(java.io.Reader stream) {
            jj_input_stream.ReInit(stream, 1, 1);
            token_source.ReInit(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jjtree.reset();
            jj_gen = 0;
            for (int i = 0; i < 10; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();
          }*/

        /** Constructor with generated Token Manager. */
        /*public TsurgeonParser(TsurgeonParserTokenManager tm) {
            token_source = tm;
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 10; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();
          }*/

        /** Reinitialise. */
        /*public void ReInit(TsurgeonParserTokenManager tm) {
            token_source = tm;
            token = new Token();
            jj_ntk = -1;
            jjtree.reset();
            jj_gen = 0;
            for (int i = 0; i < 10; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.length; i++) jj_2_rtns[i] = new JJCalls();
          }s*/

        private Token Jj_consume_token(int kind)
        {
            Token oldToken;
            if ((oldToken = token).Next != null) token = token.Next;
            else token = token.Next = token_source.GetNextToken();
            jj_ntk = -1;
            if (token.Kind == kind)
            {
                jj_gen++;
                if (++jj_gc > 100)
                {
                    jj_gc = 0;
                    for (int i = 0; i < jj_2_rtns.Length; i++)
                    {
                        JjCalls c = jj_2_rtns[i];
                        while (c != null)
                        {
                            if (c.Gen < jj_gen) c.First = null;
                            c = c.Next;
                        }
                    }
                }
                return token;
            }
            token = oldToken;
            jj_kind = kind;
            throw GenerateParseException();
        }

        private sealed class LookaheadSuccess : SystemException
        {
        }

        private readonly LookaheadSuccess jj_ls = new LookaheadSuccess();

        private bool Jj_scan_token(int kind)
        {
            if (jj_scanpos == jj_lastpos)
            {
                jj_la--;
                if (jj_scanpos.Next == null)
                {
                    jj_lastpos = jj_scanpos = jj_scanpos.Next = token_source.GetNextToken();
                }
                else
                {
                    jj_lastpos = jj_scanpos = jj_scanpos.Next;
                }
            }
            else
            {
                jj_scanpos = jj_scanpos.Next;
            }
            if (jj_rescan)
            {
                int i = 0;
                Token tok = token;
                while (tok != null && tok != jj_scanpos)
                {
                    i++;
                    tok = tok.Next;
                }
                if (tok != null) Jj_add_error_token(kind, i);
            }
            if (jj_scanpos.Kind != kind) return true;
            if (jj_la == 0 && jj_scanpos == jj_lastpos) throw jj_ls;
            return false;
        }

        /// <summary>
        /// Get the next Token
        /// </summary>
        public Token GetNextToken()
        {
            if (token.Next != null) token = token.Next;
            else token = token.Next = token_source.GetNextToken();
            jj_ntk = -1;
            jj_gen++;
            return token;
        }

        /** Get the specific Token. */
        /*public Token getToken(int index)
        {
            Token t = token;
            for (int i = 0; i < index; i++)
            {
                if (t.next != null) t = t.next;
                else t = t.next = token_source.getNextToken();
            }
            return t;
        }*/

        private int Jj_ntk_f()
        {
            if ((jj_nt = token.Next) == null)
                return (jj_ntk = (token.Next = token_source.GetNextToken()).Kind);
            else
                return (jj_ntk = jj_nt.Kind);
        }

        private readonly List<int[]> jj_expentries = new List<int[]>();
        private int[] jj_expentry;
        private int jj_kind = -1;
        private readonly int[] jj_lasttokens = new int[100];
        private int jj_endpos;

        private void Jj_add_error_token(int kind, int pos)
        {
            if (pos >= 100) return;
            if (pos == jj_endpos + 1)
            {
                jj_lasttokens[jj_endpos++] = kind;
            }
            else if (jj_endpos != 0)
            {
                jj_expentry = new int[jj_endpos];
                for (int i = 0; i < jj_endpos; i++)
                {
                    jj_expentry[i] = jj_lasttokens[i];
                }
                /*jj_entries_loop:
                for (java.util.Iterator < ? > it = jj_expentries.iterator();
                it.hasNext();)*/
                foreach (var jjExpentry in jj_expentries)
                {
                    //int[] oldentry = (int[]) (it.next());
                    int[] oldentry = jjExpentry;
                    if (oldentry.Length == jj_expentry.Length)
                    {
                        for (int i = 0; i < jj_expentry.Length; i++)
                        {
                            if (oldentry[i] != jj_expentry[i])
                            {
                                //continue jj_entries_loop;
                                goto post_jj_entries_loop;
                            }
                        }
                        jj_expentries.Add(jj_expentry);
                        //break jj_entries_loop;
                        goto post_jj_entries_loop;
                    }
                }
                post_jj_entries_loop:
                {
                    if (pos != 0) jj_lasttokens[(jj_endpos = pos) - 1] = kind;
                }
            }
        }

        public ParseException GenerateParseException()
        {
            jj_expentries.Clear();
            bool[] la1tokens = new bool[33];
            if (jj_kind >= 0)
            {
                la1tokens[jj_kind] = true;
                jj_kind = -1;
            }
            for (int i = 0; i < 10; i++)
            {
                if (jj_la1[i] == jj_gen)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((jj_la1_0[i] & (1 << j)) != 0)
                        {
                            la1tokens[j] = true;
                        }
                        if ((jj_la1_1[i] & (1 << j)) != 0)
                        {
                            la1tokens[32 + j] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < 33; i++)
            {
                if (la1tokens[i])
                {
                    jj_expentry = new int[1];
                    jj_expentry[0] = i;
                    jj_expentries.Add(jj_expentry);
                }
            }
            jj_endpos = 0;
            Jj_rescan_token();
            Jj_add_error_token(0, 0);
            var exptokseq = new int[jj_expentries.Count][];
            for (int i = 0; i < jj_expentries.Count; i++)
            {
                exptokseq[i] = jj_expentries[i];
            }
            return new ParseException(token, exptokseq, TokenImages);
        }

        private void Jj_rescan_token()
        {
            jj_rescan = true;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    JjCalls p = jj_2_rtns[i];
                    do
                    {
                        if (p.Gen > jj_gen)
                        {
                            jj_la = p.Arg;
                            jj_lastpos = jj_scanpos = p.First;
                            switch (i)
                            {
                                case 0:
                                    Jj_3_1();
                                    break;
                                case 1:
                                    Jj_3_2();
                                    break;
                                case 2:
                                    Jj_3_3();
                                    break;
                                case 3:
                                    Jj_3_4();
                                    break;
                                case 4:
                                    Jj_3_5();
                                    break;
                                case 5:
                                    Jj_3_6();
                                    break;
                                case 6:
                                    Jj_3_7();
                                    break;
                                case 7:
                                    Jj_3_8();
                                    break;
                                case 8:
                                    Jj_3_9();
                                    break;
                                case 9:
                                    Jj_3_10();
                                    break;
                            }
                        }
                        p = p.Next;
                    } while (p != null);
                }
                catch (LookaheadSuccess ls)
                {
                }
            }
            jj_rescan = false;
        }

        private void Jj_save(int index, int xla)
        {
            JjCalls p = jj_2_rtns[index];
            while (p.Gen > jj_gen)
            {
                if (p.Next == null)
                {
                    p = p.Next = new JjCalls();
                    break;
                }
                p = p.Next;
            }
            p.Gen = jj_gen + xla - jj_la;
            p.First = token;
            p.Arg = xla;
        }

        private class JjCalls
        {
            public int Gen;
            public Token First;
            public int Arg;
            public JjCalls Next;
        }
    }
}