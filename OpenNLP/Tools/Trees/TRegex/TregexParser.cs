using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util;

namespace OpenNLP.Tools.Trees.TRegex
{
    public class TregexParser : TregexParserConstants
    {
        /// <summary>
        /// this is so we can tell, at any point during the parse
        /// whether we are under a negation, which we need to know
        /// because labeling nodes under negation is illegal
        /// </summary>
        private bool underNegation = false;

        private readonly Func<string, string> basicCatFunction =
            TregexPatternCompiler.DEFAULT_BASIC_CAT_FUNCTION.Apply;

        private readonly IHeadFinder headFinder =
            TregexPatternCompiler.DEFAULT_HEAD_FINDER;

        /// <summary>
        /// keep track of which variables we've seen, so that we can reject
        /// some nonsense patterns such as ones that reset variables or link
        /// to variables that haven't been set
        /// </summary>
        private Set<string> knownVariables = new Set<string>();

        public TregexParser(TextReader stream,
            Func<string, string> basicCatFunction,
            IHeadFinder headFinder) :
                this(stream)
        {
            this.basicCatFunction = basicCatFunction;
            this.headFinder = headFinder;
        }

        // TODO: IDENTIFIER should not allow | after the first character, but it breaks some | queries to allow it.

        /// <summary>
        /// the grammar starts here
        /// each of these BNF rules will be converted into a function
        /// first expr is return val- passed up the tree after a production
        /// </summary>
        /// <returns></returns>
        public TregexPattern Root()
        {
            var nodes = new List<TregexPattern>();
            // a local variable

            TregexPattern node = SubNode(TRegex.Relation.Root);
            nodes.Add(node);
            //label_1:
            while (true)
            {
                if (Jj_2_1(2))
                {
                    ;
                }
                else
                {
                    //break label_1;
                    break;
                }
                Jj_consume_token(12);
                node = SubNode(TRegex.Relation.Root);
                nodes.Add(node);
            }
            Jj_consume_token(13);
            if (nodes.Count == 1)
            {
                return nodes[0];
            }
            else
            {
                return new CoordinationPattern(nodes, false);
            }
        }

        /// <summary>
        /// passing arguments down the tree - in this case the relation that
        /// pertains to this node gets passed all the way down to the Description node
        /// </summary>
        public DescriptionPattern Node(Relation r)
        {
            DescriptionPattern node;
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case 14:
                {
                    Jj_consume_token(14);
                    node = SubNode(r);
                    Jj_consume_token(15);
                    break;
                }
                case Identifier:
                case Blank:
                case Regex:
                case 16:
                case 17:
                case 20:
                case 21:
                {
                    node = ModDescription(r);
                    break;
                }
                default:
                    jj_la1[0] = jj_gen;
                    Jj_consume_token(-1);
                    throw new ParseException();
            }
            return node;
        }

        public DescriptionPattern SubNode(Relation r)
        {
            DescriptionPattern result = null;
            TregexPattern child = null;
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case 14:
                {
                    Jj_consume_token(14);
                    result = SubNode(r);
                    Jj_consume_token(15);
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case RELATION:
                        case MultiRelation:
                        case RelWStrArg:
                        case 14:
                        case 16:
                        case 23:
                        case 24:
                        {
                            child = ChildrenDisj();
                            break;
                        }
                        default:
                            jj_la1[1] = jj_gen;
                            break;
                    }
                    if (child != null)
                    {
                        var newChildren = new List<TregexPattern>();
                        newChildren.AddRange(result.GetChildren());
                        newChildren.Add(child);
                        result.SetChild(new CoordinationPattern(newChildren, true));
                    }
                    return result;
                }
                case Identifier:
                case Blank:
                case Regex:
                case 16:
                case 17:
                case 20:
                case 21:
                {
                    result = ModDescription(r);
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case RELATION:
                        case MultiRelation:
                        case RelWStrArg:
                        case 14:
                        case 16:
                        case 23:
                        case 24:
                        {
                            child = ChildrenDisj();
                            break;
                        }
                        default:
                            jj_la1[2] = jj_gen;
                            break;
                    }
                    if (child != null) result.SetChild(child);
                    return result;
                }
                default:
                    jj_la1[3] = jj_gen;
                    Jj_consume_token(-1);
                    throw new ParseException();
            }
        }

        public DescriptionPattern ModDescription(Relation r)
        {
            bool neg = false, cat = false;
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case 16:
                {
                    Jj_consume_token(16);
                    neg = true;
                    break;
                }
                default:
                    jj_la1[4] = jj_gen;
                    break;
            }
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case 17:
                {
                    Jj_consume_token(17);
                    cat = true;
                    break;
                }
                default:
                    jj_la1[5] = jj_gen;
                    break;
            }
            DescriptionPattern node = Description(r, neg, cat);
            return node;
        }

        public DescriptionPattern Description(Relation r, bool negateDesc, bool cat)
        {
            Token desc = null;
            Token name = null;
            Token linkedName = null;
            bool link = false;
            var varGroups = new List<Tuple<int, string>>();
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case Identifier:
                case Blank:
                case Regex:
                {
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case Identifier:
                        {
                            desc = Jj_consume_token(Identifier);
                            break;
                        }
                        case Regex:
                        {
                            desc = Jj_consume_token(Regex);
                            break;
                        }
                        case Blank:
                        {
                            desc = Jj_consume_token(Blank);
                            break;
                        }
                        default:
                            jj_la1[6] = jj_gen;
                            Jj_consume_token(-1);
                            throw new ParseException();
                    }
                    //label_2:
                    while (true)
                    {
                        switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                        {
                            case 18:
                            {
                                ;
                                break;
                            }
                            default:
                                jj_la1[7] = jj_gen;
                                //break label_2;
                                goto post_label_2;
                        }
                        Jj_consume_token(18);
                        Token groupNum = Jj_consume_token(Number);
                        Jj_consume_token(19);
                        Token groupVar = Jj_consume_token(Identifier);
                        varGroups.Add(new Tuple<int, string>(int.Parse(groupNum.Image), groupVar.Image));
                    }
                    post_label_2:
                    {
                        switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                        {
                            case 20:
                            {
                                Jj_consume_token(20);
                                name = Jj_consume_token(Identifier);
                                if (knownVariables.Contains(name.Image))
                                {
                                    throw new ParseException("Variable " + name.Image +
                                                                     " has been declared twice, which makes no sense");
                                }
                                else
                                {
                                    knownVariables.Add(name.Image);
                                }
                                if (underNegation)
                                {
                                    throw new ParseException("No named tregex nodes allowed in the scope of negation.");
                                }
                                break;
                            }
                            default:
                                jj_la1[8] = jj_gen;
                                break;
                        }
                        break;
                    }
                }
                case 21:
                {
                    Jj_consume_token(21);
                    linkedName = Jj_consume_token(Identifier);
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case 20:
                        {
                            Jj_consume_token(20);
                            name = Jj_consume_token(Identifier);
                            break;
                        }
                        default:
                            jj_la1[9] = jj_gen;
                            break;
                    }
                    if (!knownVariables.Contains(linkedName.Image))
                    {
                        throw new ParseException("Variable " + linkedName.Image +
                                                         " was referenced before it was declared");
                    }
                    if (name != null)
                    {
                        if (knownVariables.Contains(name.Image))
                        {
                            throw new ParseException("Variable " + name.Image +
                                                             " has been declared twice, which makes no sense");
                        }
                        else
                        {
                            knownVariables.Add(name.Image);
                        }
                    }
                    link = true;
                    break;
                }
                case 20:
                {
                    Jj_consume_token(20);
                    name = Jj_consume_token(Identifier);
                    if (!knownVariables.Contains(name.Image))
                    {
                        throw new ParseException("Variable " + name.Image +
                                                         " was referenced before it was declared");
                    }
                    break;
                }
                default:
                    jj_la1[10] = jj_gen;
                    Jj_consume_token(-1);
                    throw new ParseException();
            }
            var ret = new DescriptionPattern(r, negateDesc, desc != null ? desc.Image : null,
                name != null ? name.Image : null, cat, basicCatFunction, varGroups, link,
                linkedName != null ? linkedName.Image : null);
            return ret;
        }

        public TregexPattern ChildrenDisj()
        {
            var children = new List<TregexPattern>();
            // When we keep track of the known variables to assert that
            // variables are not redefined, or that links are only set to known
            // variables, we want to separate those done in different parts of the
            // disjunction.  Variables set in one part won't be set in the next
            // part if it gets there, since disjunctions exit once known.
            var originalKnownVariables = new Set<string>(knownVariables);
            // However, we want to keep track of all the known variables, so that after
            // the disjunction is over, we know them all.
            var allKnownVariables = new Set<string>(knownVariables);
            TregexPattern child = ChildrenConj();
            children.Add(child);
            allKnownVariables.AddAll(knownVariables);
            //label_3:
            while (true)
            {
                if (Jj_2_2(2))
                {
                    ;
                }
                else
                {
                    //break label_3;
                    break;
                }
                knownVariables = new Set<string>(originalKnownVariables);
                Jj_consume_token(12);
                child = ChildrenConj();
                children.Add(child);
                allKnownVariables.AddAll(knownVariables);
            }
            knownVariables = allKnownVariables;
            if (children.Count == 1)
            {
                return child;
            }
            else
            {
                return new CoordinationPattern(children, false);
            }
        }

        public TregexPattern ChildrenConj()
        {
            var children = new List<TregexPattern>();
            TregexPattern child = ModChild();
            children.Add(child);
            //label_4:
            while (true)
            {
                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                {
                    case RELATION:
                    case MultiRelation:
                    case RelWStrArg:
                    case 14:
                    case 16:
                    case 22:
                    case 23:
                    case 24:
                    {
                        ;
                        break;
                    }
                    default:
                        jj_la1[11] = jj_gen;
                        //break label_4;
                        goto post_label_4;
                }
                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                {
                    case 22:
                    {
                        Jj_consume_token(22);
                        break;
                    }
                    default:
                        jj_la1[12] = jj_gen;
                        break;
                }
                child = ModChild();
                children.Add(child);
            }
            post_label_4:
            {
                if (children.Count == 1)
                {
                    return child;
                }
                else
                {
                    return new CoordinationPattern(children, true);
                }
            }
        }

        public TregexPattern ModChild()
        {
            TregexPattern child;
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case RELATION:
                case MultiRelation:
                case RelWStrArg:
                case 14:
                case 24:
                {
                    child = Child();
                    break;
                }
                case 16:
                {
                    Jj_consume_token(16);
                    bool startUnderNeg = underNegation;
                    underNegation = true;
                    child = ModChild();
                    underNegation = startUnderNeg;
                    child.Negate();
                    break;
                }
                case 23:
                {
                    Jj_consume_token(23);
                    child = Child();
                    child.MakeOptional();
                    break;
                }
                default:
                    jj_la1[13] = jj_gen;
                    Jj_consume_token(-1);
                    throw new ParseException();
            }
            return child;
        }

        public TregexPattern Child()
        {
            TregexPattern child;
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case 24:
                {
                    Jj_consume_token(24);
                    child = ChildrenDisj();
                    Jj_consume_token(25);
                    break;
                }
                case 14:
                {
                    Jj_consume_token(14);
                    child = ChildrenDisj();
                    Jj_consume_token(15);
                    break;
                }
                case RELATION:
                case MultiRelation:
                case RelWStrArg:
                {
                    child = Relation();
                    break;
                }
                default:
                    jj_la1[14] = jj_gen;
                    Jj_consume_token(-1);
                    throw new ParseException();
            }
            return child;
        }

        public TregexPattern Relation()
        {
            Token t, strArg = null, numArg = null, negation = null, cat = null;
            // the easiest way to check if an optional production was used
            // is to set the token to null and then check it later
            Relation r;
            DescriptionPattern child;
            var children = new List<DescriptionPattern>();
            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
            {
                case RELATION:
                case RelWStrArg:
                {
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case RELATION:
                        {
                            t = Jj_consume_token(RELATION);
                            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                            {
                                case Number:
                                {
                                    numArg = Jj_consume_token(Number);
                                    break;
                                }
                                default:
                                    jj_la1[15] = jj_gen;
                                    break;
                            }
                            break;
                        }
                        case RelWStrArg:
                        {
                            t = Jj_consume_token(RelWStrArg);
                            switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                            {
                                case 14:
                                {
                                    Jj_consume_token(14);
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case 16:
                                        {
                                            negation = Jj_consume_token(16);
                                            break;
                                        }
                                        default:
                                            jj_la1[16] = jj_gen;
                                            break;
                                    }
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case 17:
                                        {
                                            cat = Jj_consume_token(17);
                                            break;
                                        }
                                        default:
                                            jj_la1[17] = jj_gen;
                                            break;
                                    }
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case Regex:
                                        {
                                            strArg = Jj_consume_token(Regex);
                                            break;
                                        }
                                        case Identifier:
                                        {
                                            strArg = Jj_consume_token(Identifier);
                                            break;
                                        }
                                        case Blank:
                                        {
                                            strArg = Jj_consume_token(Blank);
                                            break;
                                        }
                                        default:
                                            jj_la1[18] = jj_gen;
                                            Jj_consume_token(-1);
                                            throw new ParseException();
                                    }
                                    Jj_consume_token(15);
                                    break;
                                }
                                case 24:
                                {
                                    Jj_consume_token(24);
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case 16:
                                        {
                                            negation = Jj_consume_token(16);
                                            break;
                                        }
                                        default:
                                            jj_la1[19] = jj_gen;
                                            break;
                                    }
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case 17:
                                        {
                                            cat = Jj_consume_token(17);
                                            break;
                                        }
                                        default:
                                            jj_la1[20] = jj_gen;
                                            break;
                                    }
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case Regex:
                                        {
                                            strArg = Jj_consume_token(Regex);
                                            break;
                                        }
                                        case Identifier:
                                        {
                                            strArg = Jj_consume_token(Identifier);
                                            break;
                                        }
                                        case Blank:
                                        {
                                            strArg = Jj_consume_token(Blank);
                                            break;
                                        }
                                        default:
                                            jj_la1[21] = jj_gen;
                                            Jj_consume_token(-1);
                                            throw new ParseException();
                                    }
                                    Jj_consume_token(25);
                                    break;
                                }
                                case Regex:
                                case 16:
                                {
                                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                    {
                                        case 16:
                                        {
                                            negation = Jj_consume_token(16);
                                            break;
                                        }
                                        default:
                                            jj_la1[22] = jj_gen;
                                            break;
                                    }
                                    strArg = Jj_consume_token(Regex);
                                    break;
                                }
                                default:
                                    jj_la1[23] = jj_gen;
                                    Jj_consume_token(-1);
                                    throw new ParseException();
                            }
                            break;
                        }
                        default:
                            jj_la1[24] = jj_gen;
                            Jj_consume_token(-1);
                            throw new ParseException();
                    }
                    if (strArg != null)
                    {
                        string negStr = negation == null ? "" : "!";
                        string catStr = cat == null ? "" : "@";
                        r = TRegex.Relation.GetRelation(t.Image, negStr + catStr + strArg.Image,
                            basicCatFunction, headFinder);
                    }
                    else if (numArg != null)
                    {
                        if (t.Image.EndsWith("-"))
                        {
                            t.Image = t.Image.Substring(0, t.Image.Length - 1);
                            numArg.Image = "-" + numArg.Image;
                        }
                        r = TRegex.Relation.GetRelation(t.Image, numArg.Image,
                            basicCatFunction, headFinder);
                    }
                    else
                    {
                        r = TRegex.Relation.GetRelation(t.Image, basicCatFunction, headFinder);
                    }
                    child = Node(r);
                    return child;
                }
                case MultiRelation:
                {
                    t = Jj_consume_token(MultiRelation);
                    Jj_consume_token(26);
                    switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                    {
                        case Identifier:
                        case Blank:
                        case Regex:
                        case 14:
                        case 16:
                        case 17:
                        case 20:
                        case 21:
                        {
                            child = Node(null);
                            children.Add(child);
                            //label_5:
                            while (true)
                            {
                                switch ((jj_ntk == -1) ? Jj_ntk_f() : jj_ntk)
                                {
                                    case 27:
                                    {
                                        ;
                                        break;
                                    }
                                    default:
                                        jj_la1[25] = jj_gen;
                                        //break label_5;
                                        goto post_label_5;
                                }
                                Jj_consume_token(27);
                                child = Node(null);
                                children.Add(child);
                            }
                            post_label_5:
                            {
                                break;
                            }
                        }
                        default:
                            jj_la1[26] = jj_gen;
                            break;
                    }
                    Jj_consume_token(28);
                    return TRegex.Relation.ConstructMultiRelation(t.Image, children, basicCatFunction,
                                headFinder);
                }
                default:
                    jj_la1[27] = jj_gen;
                    Jj_consume_token(-1);
                    throw new ParseException();
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

        private bool Jj_3R_25()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_26())
            {
                jj_scanpos = xsp;
                if (Jj_3R_27()) return true;
            }
            return false;
        }

        private bool Jj_3R_9()
        {
            if (Jj_3R_11()) return true;
            return false;
        }

        private bool Jj_3R_24()
        {
            if (Jj_3R_25()) return true;
            return false;
        }

        private bool Jj_3R_23()
        {
            if (Jj_scan_token(14)) return true;
            return false;
        }

        private bool Jj_3R_20()
        {
            if (Jj_scan_token(21)) return true;
            return false;
        }

        private bool Jj_3_2()
        {
            if (Jj_scan_token(12)) return true;
            if (Jj_3R_7()) return true;
            return false;
        }

        private bool Jj_3R_22()
        {
            if (Jj_scan_token(24)) return true;
            return false;
        }

        private bool Jj_3R_16()
        {
            if (Jj_scan_token(17)) return true;
            return false;
        }

        private bool Jj_3R_18()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_22())
            {
                jj_scanpos = xsp;
                if (Jj_3R_23())
                {
                    jj_scanpos = xsp;
                    if (Jj_3R_24()) return true;
                }
            }
            return false;
        }

        private bool Jj_3R_8()
        {
            if (Jj_scan_token(14)) return true;
            return false;
        }

        private bool Jj_3R_6()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_8())
            {
                jj_scanpos = xsp;
                if (Jj_3R_9()) return true;
            }
            return false;
        }

        private bool Jj_3R_14()
        {
            if (Jj_scan_token(23)) return true;
            return false;
        }

        private bool Jj_3R_27()
        {
            if (Jj_scan_token(MultiRelation)) return true;
            return false;
        }

        private bool Jj_3R_19()
        {
            Token xsp = jj_scanpos;
            if (Jj_scan_token(8))
            {
                jj_scanpos = xsp;
                if (Jj_scan_token(10))
                {
                    jj_scanpos = xsp;
                    if (Jj_scan_token(9)) return true;
                }
            }
            return false;
        }

        private bool Jj_3R_13()
        {
            if (Jj_scan_token(16)) return true;
            return false;
        }

        private bool Jj_3R_17()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_19())
            {
                jj_scanpos = xsp;
                if (Jj_3R_20())
                {
                    jj_scanpos = xsp;
                    if (Jj_3R21()) return true;
                }
            }
            return false;
        }

        private bool Jj_3R_12()
        {
            if (Jj_3R_18()) return true;
            return false;
        }

        private bool Jj_3R_10()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_12())
            {
                jj_scanpos = xsp;
                if (Jj_3R_13())
                {
                    jj_scanpos = xsp;
                    if (Jj_3R_14()) return true;
                }
            }
            return false;
        }

        private bool Jj_3R_15()
        {
            if (Jj_scan_token(16)) return true;
            return false;
        }

        private bool Jj_3R_29()
        {
            if (Jj_scan_token(RelWStrArg)) return true;
            return false;
        }

        private bool Jj_3_1()
        {
            if (Jj_scan_token(12)) return true;
            if (Jj_3R_6()) return true;
            return false;
        }

        private bool Jj_3R_28()
        {
            if (Jj_scan_token(RELATION)) return true;
            return false;
        }

        private bool Jj_3R_11()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_15()) jj_scanpos = xsp;
            xsp = jj_scanpos;
            if (Jj_3R_16()) jj_scanpos = xsp;
            if (Jj_3R_17()) return true;
            return false;
        }

        private bool Jj_3R21()
        {
            if (Jj_scan_token(20)) return true;
            return false;
        }

        private bool Jj_3R_7()
        {
            if (Jj_3R_10()) return true;
            return false;
        }

        private bool Jj_3R_26()
        {
            Token xsp = jj_scanpos;
            if (Jj_3R_28())
            {
                jj_scanpos = xsp;
                if (Jj_3R_29()) return true;
            }
            return false;
        }

        /// <summary>Generated Token Manager</summary>
        public TregexParserTokenManager token_source;
        private readonly SimpleCharStream jj_input_stream;
        /// <summary>Current token</summary>
        public Token token;
        /// <summary>Next token</summary>
        public Token jj_nt;
        private int jj_ntk;
        private Token jj_scanpos, jj_lastpos;
        private int jj_la;
        private int jj_gen;
        private readonly int[] jj_la1 = new int[28];

        private static readonly int[] jj_la1_0 = new int[]
        {
            0x334700, 0x1814070, 0x1814070, 0x334700, 0x10000, 0x20000, 0x700, 0x40000, 0x100000, 0x100000, 0x300700,
            0x1c14070, 0x400000, 0x1814070, 0x1004070, 0x80, 0x10000, 0x20000, 0x700, 0x10000, 0x20000, 0x700,
            0x10000, 0x1014400, 0x50, 0x8000000, 0x334700, 0x70,
        };

        private readonly JjCalls[] jj_2_rtns = new JjCalls[2];
        private bool jj_rescan = false;
        private int jj_gc = 0;

        /// <summary>Constructor with Stream</summary>
        public TregexParser(Stream stream) :
            this(stream, null)
        {
        }

        /// <summary>
        /// Constructor with Stream and supplied encoding
        /// </summary>
        public TregexParser(Stream stream, string encoding)
        {
            try
            {
                jj_input_stream = new SimpleCharStream(stream, encoding, 1, 1);
            }
            catch (IOException e)
            {
                throw new SystemException("", e);
            }
            token_source = new TregexParserTokenManager(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 28; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /// <summary>Reinitialize</summary>
        public void ReInit(Stream stream)
        {
            ReInit(stream, null);
        }

        /// <summary>Reinitialize</summary>
        public void ReInit(Stream stream, string encoding)
        {
            try
            {
                jj_input_stream.ReInit(stream, encoding, 1, 1);
            }
            catch (IOException e)
            {
                throw new SystemException("", e);
            }
            token_source.ReInit(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 28; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /// <summary>Constructor</summary>
        public TregexParser(TextReader stream)
        {
            jj_input_stream = new SimpleCharStream(stream, 1, 1);
            token_source = new TregexParserTokenManager(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 28; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /// <summary>Reinitialize</summary>
        public void ReInit(TextReader stream)
        {
            jj_input_stream.ReInit(stream, 1, 1);
            token_source.ReInit(jj_input_stream);
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 28; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /// <summary>
        /// Constructor with generated Token Manager
        /// </summary>
        public TregexParser(TregexParserTokenManager tm)
        {
            token_source = tm;
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 28; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        /// <summary>Reinitialize</summary>
        public void ReInit(TregexParserTokenManager tm)
        {
            token_source = tm;
            token = new Token();
            jj_ntk = -1;
            jj_gen = 0;
            for (int i = 0; i < 28; i++) jj_la1[i] = -1;
            for (int i = 0; i < jj_2_rtns.Length; i++) jj_2_rtns[i] = new JjCalls();
        }

        private Token Jj_consume_token(int kind)
        {
            Token oldToken;
            if ((oldToken = token).Next != null)
            {
                token = token.Next;
            }
            else
            {
                token = token.Next = token_source.GetNextToken();
            }
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
                            if (c.gen < jj_gen) c.first = null;
                            c = c.next;
                        }
                    }
                }
                return token;
            }
            token = oldToken;
            jj_kind = kind;
            throw GenerateParseException();
        }

        private class LookaheadSuccess : Exception
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

        /// <summary>Get the next Token</summary>
        public Token GetNextToken()
        {
            if (token.Next != null) token = token.Next;
            else token = token.Next = token_source.GetNextToken();
            jj_ntk = -1;
            jj_gen++;
            return token;
        }

        /// <summary>Get the specific Token</summary>
        public Token GetToken(int index)
        {
            Token t = token;
            for (int i = 0; i < index; i++)
            {
                if (t.Next != null) t = t.Next;
                else t = t.Next = token_source.GetNextToken();
            }
            return t;
        }

        private int Jj_ntk_f()
        {
            if ((jj_nt = token.Next) == null)
            {
                //return (jj_ntk = (token.next = token_source.getNextToken()).kind);
                token.Next = token_source.GetNextToken();
                jj_ntk = token.Next.Kind;
                return jj_ntk;
            }
            else
            {
                return (jj_ntk = jj_nt.Kind);
            }
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
                /*jj_entries_loop: for (java.util.Iterator<?> it = jj_expentries.iterator(); it.hasNext();) {
                int[] oldentry = (int[])(it.next());
                if (oldentry.length == jj_expentry.length) {
                    for (int i = 0; i < jj_expentry.length; i++) {
                    if (oldentry[i] != jj_expentry[i]) {
                        continue jj_entries_loop;
                    }
                    }
                    jj_expentries.Add(jj_expentry);
                    break jj_entries_loop;
                }
                }*/
                foreach (var oldentry in jj_expentries)
                {
                    //int[] oldentry = (int[])(it.next());
                    if (oldentry.Length == jj_expentry.Length)
                    {
                        for (int i = 0; i < jj_expentry.Length; i++)
                        {
                            if (oldentry[i] != jj_expentry[i])
                            {
                                goto end_jj_entries_loop;
                            }
                        }
                        jj_expentries.Add(jj_expentry);
                        goto post_jj_entries_loop;
                    }

                    end_jj_entries_loop:
                    {
                    }
                }
                post_jj_entries_loop:
                {
                    if (pos != 0) jj_lasttokens[(jj_endpos = pos) - 1] = kind;
                }
            }
        }

        /// <summary>Generate ParseException</summary>
        public ParseException GenerateParseException()
        {
            jj_expentries.Clear();
            var la1Tokens = new bool[29];
            if (jj_kind >= 0)
            {
                la1Tokens[jj_kind] = true;
                jj_kind = -1;
            }
            for (int i = 0; i < 28; i++)
            {
                if (jj_la1[i] == jj_gen)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((jj_la1_0[i] & (1 << j)) != 0)
                        {
                            la1Tokens[j] = true;
                        }
                    }
                }
            }
            for (int i = 0; i < 29; i++)
            {
                if (la1Tokens[i])
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
            return new ParseException(token, exptokseq, TokenImage);
        }

        private void Jj_rescan_token()
        {
            jj_rescan = true;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    JjCalls p = jj_2_rtns[i];
                    do
                    {
                        if (p.gen > jj_gen)
                        {
                            jj_la = p.arg;
                            jj_lastpos = jj_scanpos = p.first;
                            switch (i)
                            {
                                case 0:
                                    Jj_3_1();
                                    break;
                                case 1:
                                    Jj_3_2();
                                    break;
                            }
                        }
                        p = p.next;
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
            while (p.gen > jj_gen)
            {
                if (p.next == null)
                {
                    p = p.next = new JjCalls();
                    break;
                }
                p = p.next;
            }
            p.gen = jj_gen + xla - jj_la;
            p.first = token;
            p.arg = xla;
        }
        
        private sealed class JjCalls
        {
            public int gen;
            public Token first;
            public int arg;
            public JjCalls next;
        }
    }
}