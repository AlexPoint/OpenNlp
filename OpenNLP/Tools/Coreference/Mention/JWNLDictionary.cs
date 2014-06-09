//Copyright (C) 2006 Richard J. Northedge
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

//This file is based on the JWNLDictionary.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Thomas Morton
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
//UPGRADE_TODO: The type 'net.didion.jwnl.JWNLException' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using JWNLException = net.didion.jwnl.JWNLException;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.Adjective' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using Adjective = net.didion.jwnl.data.Adjective;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.FileDictionaryElementFactory' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using FileDictionaryElementFactory = net.didion.jwnl.data.FileDictionaryElementFactory;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.IndexWord' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using IndexWord = net.didion.jwnl.data.IndexWord;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.POS' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using POS = net.didion.jwnl.data.POS;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.Pointer' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using Pointer = net.didion.jwnl.data.Pointer;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.PointerType' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using PointerType = net.didion.jwnl.data.PointerType;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.Synset' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using Synset = net.didion.jwnl.data.Synset;
////UPGRADE_TODO: The type 'net.didion.jwnl.data.VerbFrame' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using VerbFrame = net.didion.jwnl.data.VerbFrame;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.FileBackedDictionary' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using FileBackedDictionary = net.didion.jwnl.dictionary.FileBackedDictionary;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.MorphologicalProcessor' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using MorphologicalProcessor = net.didion.jwnl.dictionary.MorphologicalProcessor;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.file_manager.FileManager' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using FileManager = net.didion.jwnl.dictionary.file_manager.FileManager;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.file_manager.FileManagerImpl' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using FileManagerImpl = net.didion.jwnl.dictionary.file_manager.FileManagerImpl;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.morph.DefaultMorphologicalProcessor' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using DefaultMorphologicalProcessor = net.didion.jwnl.dictionary.morph.DefaultMorphologicalProcessor;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.morph.DetachSuffixesOperation' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using DetachSuffixesOperation = net.didion.jwnl.dictionary.morph.DetachSuffixesOperation;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.morph.LookupExceptionsOperation' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using LookupExceptionsOperation = net.didion.jwnl.dictionary.morph.LookupExceptionsOperation;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.morph.LookupIndexWordOperation' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using LookupIndexWordOperation = net.didion.jwnl.dictionary.morph.LookupIndexWordOperation;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.morph.Operation' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using Operation = net.didion.jwnl.dictionary.morph.Operation;
////UPGRADE_TODO: The type 'net.didion.jwnl.dictionary.morph.TokenizerOperation' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using TokenizerOperation = net.didion.jwnl.dictionary.morph.TokenizerOperation;
////UPGRADE_TODO: The type 'net.didion.jwnl.princeton.data.PrincetonWN17FileDictionaryElementFactory' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using PrincetonWN17FileDictionaryElementFactory = net.didion.jwnl.princeton.data.PrincetonWN17FileDictionaryElementFactory;
////UPGRADE_TODO: The type 'net.didion.jwnl.princeton.file.PrincetonRandomAccessDictionaryFile' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using PrincetonRandomAccessDictionaryFile = net.didion.jwnl.princeton.file.PrincetonRandomAccessDictionaryFile;

using SharpWordNet;
using System.Collections.Generic;

namespace OpenNLP.Tools.Coreference.Mention
{
	
	/// <summary> An implementation of the Dictionary interface using the JWNL library. </summary>
	public class JWNLDictionary : IDictionary
	{
        private WordNetEngine mEngine;
		//private net.didion.jwnl.dictionary.Dictionary dict;

        private WordNetEngine.MorphologicalProcessOperation morphologicalProcess;

		//private MorphologicalProcessor morphy;
		private static string[] empty = new string[0];
		
		public JWNLDictionary(string searchDirectory)
		{
            //PointerType.initialize();
            //Adjective.initialize();
            //VerbFrame.initialize();
            ////UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilHashMap'"
            //System.Collections.IDictionary suffixMap = new System.Collections.Hashtable();
            //suffixMap[POS.NOUN] = new string[][]{new string[]{"s", ""}, new string[]{"ses", "s"}, new string[]{"xes", "x"}, new string[]{"zes", "z"}, new string[]{"ches", "ch"}, new string[]{"shes", "sh"}, new string[]{"men", "man"}, new string[]{"ies", "y"}};
            //suffixMap[POS.VERB] = new string[][]{new string[]{"s", ""}, new string[]{"ies", "y"}, new string[]{"es", "e"}, new string[]{"es", ""}, new string[]{"ed", "e"}, new string[]{"ed", ""}, new string[]{"ing", "e"}, new string[]{"ing", ""}};
            //suffixMap[POS.ADJECTIVE] = new string[][]{new string[]{"er", ""}, new string[]{"est", ""}, new string[]{"er", "e"}, new string[]{"est", "e"}};
            //DetachSuffixesOperation tokDso = new DetachSuffixesOperation(suffixMap);
            //tokDso.addDelegate(DetachSuffixesOperation.OPERATIONS, new Operation[]{new LookupIndexWordOperation(), new LookupExceptionsOperation()});
            //TokenizerOperation tokOp = new TokenizerOperation(new string[]{" ", "-"});
            //tokOp.addDelegate(TokenizerOperation.TOKEN_OPERATIONS, new Operation[]{new LookupIndexWordOperation(), new LookupExceptionsOperation(), tokDso});
            //DetachSuffixesOperation morphDso = new DetachSuffixesOperation(suffixMap);
            //morphDso.addDelegate(DetachSuffixesOperation.OPERATIONS, new Operation[]{new LookupIndexWordOperation(), new LookupExceptionsOperation()});
            //Operation[] operations = new Operation[]{new LookupExceptionsOperation(), morphDso, tokOp};
            //morphy = new DefaultMorphologicalProcessor(operations);

            //FileManager manager = new FileManagerImpl(searchDirectory, typeof(PrincetonRandomAccessDictionaryFile));
            //FileDictionaryElementFactory factory = new PrincetonWN17FileDictionaryElementFactory();
            //FileBackedDictionary.install(manager, morphy, factory, true);
            //dict = net.didion.jwnl.dictionary.Dictionary.getInstance();
            //morphy = dict.getMorphologicalProcessor();

            mEngine = new DataFileEngine(searchDirectory);
            morphologicalProcess += mEngine.LookupExceptionsOperation;
            morphologicalProcess += mEngine.LookupIndexWordOperation;
		}
		
		public virtual string[] getLemmas(string word, string tag)
		{
            //try
            //{
                string pos;
                if (tag.StartsWith("N") || tag.StartsWith("n"))
                {
                    pos = "noun";
                }
                else if (tag.StartsWith("N") || tag.StartsWith("v"))
                {
                    pos = "verb";
                }
                else if (tag.StartsWith("J") || tag.StartsWith("a"))
                {
                    pos = "adjective";
                }
                else if (tag.StartsWith("R") || tag.StartsWith("r"))
                {
                    pos = "adverb";
                }
                else
                {
                    pos = "noun";
                }
            //    System.Collections.IList lemmas = morphy.lookupAllBaseForms(pos, word);
            //    return ((string[]) SupportClass.ICollectionSupport.ToArray(lemmas, new string[lemmas.Count]));
            //}
            //catch (JWNLException e)
            //{
            //    e.printStackTrace();
            //    return null;
            //}
            return mEngine.GetBaseForms(word, pos, morphologicalProcess);
		}
		
		public virtual string getSenseKey(string lemma, string pos, int sense)
		{
            //try
            //{
                IndexWord indexWord = mEngine.GetIndexWord(lemma, "noun");
                //IndexWord indexWord = dict.getIndexWord(POS.NOUN, lemma);
                if (indexWord == null)
                {
                    return null;
                }
                //return System.Convert.ToString(indexWord.getSynsetOffsets()[sense]);
                return indexWord.SynsetOffsets[sense].ToString(System.Globalization.CultureInfo.InvariantCulture);
            //}
            //catch (JWNLException e)
            //{
            //    e.printStackTrace();
            //    return null;
            //}
            //return null;
		}
		
		public virtual int getNumSenses(string lemma, string pos)
		{
            //try
            //{
                IndexWord indexWord = mEngine.GetIndexWord(lemma, "noun");
                //IndexWord indexWord = dict.getIndexWord(POS.NOUN, lemma);
                if (indexWord == null)
                {
                    return 0;
                }
                //return indexWord.getSenseCount();
                return indexWord.SenseCount;
            //}
            //catch (JWNLException e)
            //{
            //    return 0;
            //}
            //return 0;
		}
		
		//private void  getParents(Synset synset, System.Collections.IList parents)
		//{
            //Pointer[] pointers = synset.getPointers();
            //for (int pi = 0, pn = pointers.length; pi < pn; pi++)
            //{
            //    if (pointers[pi].getType() == PointerType.HYPERNYM)
            //    {
            //        Synset parent = pointers[pi].getTargetSynset();
            //        parents.Add(System.Convert.ToString(parent.getOffset()));
            //        getParents(parent, parents);
            //    }
            //}
		//}

        private void getParents(Synset currentSynset, List<string> parentOffsets)
        {
            for (int currentRelation = 0;currentRelation < currentSynset.RelationCount;currentRelation++)
            {
                Relation relation = currentSynset.GetRelation(currentRelation);
                if (relation.SynsetRelationType.Name == "Hypernym")
                {
                    Synset parentSynset = relation.TargetSynset;
                    parentOffsets.Add(parentSynset.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    getParents(parentSynset, parentOffsets);
                }
            }
        }

		public virtual string[] getParentSenseKeys(string lemma, string pos, int sense)
		{
            ////System.err.println("JWNLDictionary.getParentSenseKeys: lemma="+lemma); this line was commented out in the java
            //try
            //{
            Synset[] synsets = mEngine.GetSynsets(lemma, "noun");
            //IndexWord indexWord= dict.getIndexWord(POS.NOUN, lemma);
            if (synsets.Length > sense)
            {
                //Synset synset = indexWord.getSense(sense + 1); //the sense+1 is because in JWNL sense ids start at 1
                List<string> parents = new List<string>();
                getParents(synsets[sense], parents);
                return parents.ToArray();
                //return (string[])SupportClass.ICollectionSupport.ToArray(parents, new string[parents.Count]);
            }
            else
            {
                return empty;
            }
            //}
            //catch (JWNLException e)
            //{
            //    e.printStackTrace();
            //    return null;
            //}
		}
		
        //[STAThread]
        //public static void  Main(string[] args)
        //{
        //    //UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangSystem'"
        //    string searchDir = System_Renamed.getProperty("WNSEARCHDIR");
        //    System.Console.Error.WriteLine("searchDir=" + searchDir);
        //    if (searchDir != null)
        //    {
        //        //UPGRADE_ISSUE: Method 'java.lang.System.getProperty' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangSystem'"
        //        Dictionary dict = new JWNLDictionary(System_Renamed.getProperty("WNSEARCHDIR"));
        //        string word = args[0];
        //        string[] lemmas = dict.getLemmas(word, "NN");
        //        for (int li = 0, ln = lemmas.Length; li < ln; li++)
        //        {
        //            for (int si = 0, sn = dict.getNumSenses(lemmas[li], "NN"); si < sn; si++)
        //            {
        //                //UPGRADE_TODO: Method 'java.util.Arrays.asList' was converted to 'System.Collections.ArrayList' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilArraysasList_javalangObject[]'"
        //                System.Console.Out.WriteLine(lemmas[li] + " (" + si + ")\t" + SupportClass.CollectionToString(new System.Collections.ArrayList(dict.getParentSenseKeys(lemmas[li], "NN", si))));
        //            }
        //        }
        //    }
        //}
	}
}