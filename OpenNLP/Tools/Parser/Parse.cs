//Copyright (C) 2005 Richard J. Northedge
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

//This file is based on the Parse.java source file found in the
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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace OpenNLP.Tools.Parser
{

	/// <summary>
	/// Exception class for problems detected during parsing.
	/// </summary>
	[Serializable]
	public class ParseException : ApplicationException
	{
		public ParseException()
		{
		}

		public ParseException(string message) : base(message)
		{
		}

		public ParseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}

	/// <summary>
	/// Class for holding constituents.
	/// </summary>
	public class Parse : System.ICloneable, System.IComparable
	{
		/// <summary>
		/// The text string on which this parse is based.  This object is shared among all parses for the same sentence.
		/// </summary>
		private string mText;

		/// <summary>
		/// The character offsets into the text for this constituent.
		/// </summary>
		private Util.Span mSpan;

		/// <summary>
		/// The syntactic type of this parse.
		/// </summary>
		private string mType;

		/// <summary>
		/// The sub-constituents of this parse.
		/// </summary>
		private List<Parse> mParts;

		/// <summary>
		/// The head parse of this parse. A parse can be its own head.
		/// </summary>
		private Parse mHead;

		/// <summary>
		/// The outcome assigned to this parse during cconstruction of its parent parse.
		/// </summary>
		private string mLabel;

		/// <summary>
		/// The parent parse of this parse. 
		/// </summary>
		private Parse mParent;
		
		/// <summary>
		/// The probability associated with the syntactic type assigned to this parse.
		/// </summary>
		private double mProbability;

		/// <summary>
		/// The string buffer used to track the derivation of this parse.
		/// </summary>
		private StringBuilder mDerivation;
		
		#region property accessors and methods for manipulating properties

		///<summary>
		///Returns the text of the sentence over which this parse was formed.
		///</summary>
		public virtual string Text
		{
			get
			{
				return mText;
			}
		}
		
		///<summary>
		///Returns the character offsets for this constituent.
		///</summary>
		public virtual Util.Span Span
		{
			get
			{
				return mSpan;
			}
		}
		
		public virtual string Type
		{
			get
			{
				return mType;
			}
			set
			{
				mType = value;
			}
		}

		/// <summary>
		/// The sub-constituents of this parse.
		/// </summary>
		public virtual Parse[] GetChildren()
		{
			return mParts.ToArray();
		}

		/// <summary>
		/// The number of children for this parse node.
		/// </summary>
		public int ChildCount
		{
			get
			{
				return mParts.Count;
			}
		}

		/// <summary>
		/// Replaces the child at the specified index with a new child with the specified label. 
		/// </summary>
		/// <param name="index">
		/// The index of the child to be replaced.
		/// </param>
		/// <param name="label">
		/// The label to be assigned to the new child.
		/// </param>
		public void SetChild(int index, string label) 
		{
			Parse newChild = (Parse) (mParts[index]).Clone();
			newChild.Label = label;
			mParts[index] = newChild;
		}
    
		/// <summary>
		/// Returns the index of this specified child.
		/// </summary>
		/// <param name="child">
		/// A child of this parse.
		/// </param>
		/// <returns>
		/// the index of this specified child or -1 if the specified child is not a child of this parse.
		/// </returns>
		public int IndexOf(Parse child) 
		{
			return mParts.IndexOf(child);
		}
		
		public virtual Parse Head
		{
			get
			{
				return mHead;
			}	
		}

		public virtual string Label
		{
			get
			{
				return mLabel;
			}
			set
			{
				mLabel = value;
			}
		}

		public virtual Parse Parent
		{
			get
			{
				return mParent;
			}
			set
			{
				mParent = value;
			}
		}

		///<summary>
		///Returns the log of the product of the probability associated with all the decisions which formed this constituent.
		///</summary>
		public virtual double Probability
		{
			get
			{
				return mProbability;
			}
		}
		
		///<summary>
		///Adds the specified probability log to this current log for this parse.
		///</summary>
		///<param name="logProbability">
		///The probaility of an action performed on this parse.
		///</param>
		internal void AddProbability(double logProbability) 
		{
			mProbability += logProbability;
		}		

		public virtual string Derivation
		{
			get
			{
				return mDerivation.ToString();
			}
		}

		internal void InitializeDerivationBuffer()
		{
			mDerivation = new StringBuilder(100);
		}

		internal void AppendDerivationBuffer(string derivationData)
		{
			mDerivation.Append(derivationData);
		}

		public virtual bool IsPosTag
		{
			get
			{
				return (mParts.Count == 1 && (mParts[0]).Type == MaximumEntropyParser.TokenNode);
			}
		}

		///<summary>Returns whether this parse is complete.</summary>
		///<returns>Returns true if the parse contains a single top-most node.</returns>
		public virtual bool IsComplete
		{
			get
			{
				return (mParts.Count == 1);
			}
		}

#endregion

		#region IClonable implementation

		public object Clone()
		{
			Parse clonedParse = (Parse)base.MemberwiseClone();
				
			clonedParse.mParts = new List<Parse>(mParts);
			if (mDerivation != null)
			{
				clonedParse.InitializeDerivationBuffer();
				clonedParse.AppendDerivationBuffer(mDerivation.ToString());
			}
			return (clonedParse);
		}

		#endregion

		#region IComparable implementation

		public virtual int CompareTo(object o)
		{
			if (!(o is Parse))
			{
				throw new ArgumentException("A Parse object is required for comparison.");
			}

			Parse testParse = (Parse) o;
			if (this.Probability > testParse.Probability)
			{
				return - 1;
			}
			else if (this.Probability < testParse.Probability)
			{
				return 1;
			}
			return 0;
		}

		#endregion

		#region constructors

		public Parse(string parseText, Util.Span span, string type, double probability)
		{
			mText = parseText;
			mSpan = span;
			mType = type;
			mProbability = probability;
			mHead = this;
			mParts = new List<Parse>();
			mLabel = null;
			mParent = null;
		}
		
		public Parse(string parseText, Util.Span span, string type, double probability, Parse head) : this(parseText, span, type, probability)
		{
			mHead = head;
		}
				
		#endregion

		#region System.Object overrides

		public override string ToString()
		{
			return mText.Substring(mSpan.Start, (mSpan.End) - (mSpan.Start));
		}
		
        //public override bool Equals (Object o)
        //{
        //    if (o == null) return false;

        //    if (this.GetType() != o.GetType()) 
        //    {
        //        return false;
        //    }

        //    Parse testParse = (Parse)o;
        //    return (this.Probability == testParse.Probability);
        //}  

        //public override int GetHashCode ()
        //{
        //    return mProbability.GetHashCode();
        //}  

		#endregion

		///<summary>
		///Returns the probability associated with the pos-tag sequence assigned to this parse.
		///</summary>
		///<returns>
		///The probability associated with the pos-tag sequence assigned to this parse.
		///</returns>
		public virtual double GetTagSequenceProbability()
		{
			//System.Console.Error.WriteLine("Parse.GetTagSequenceProbability: " + mType + " " + this);
			if (mParts.Count == 1 && (mParts[0]).Type == MaximumEntropyParser.TokenNode)
			{
				//System.Console.Error.WriteLine(this + " " + mParseProbability);
				return System.Math.Log(mProbability);
			}
			else
			{
				if (mParts.Count == 0)
				{
					throw new ParseException("Parse.GetTagSequenceProbability(): Wrong base case!");
					//return 0.0;
				}
				else
				{
					double sum = 0.0;
					foreach (Parse oChildParse in mParts)
					{
						sum += oChildParse.GetTagSequenceProbability();
					}
					return sum;
				}
			}	
		}

		///<summary>
		///Inserts the specified constituent into this parse based on its text span.  This
		///method assumes that the specified constituent can be inserted into this parse.
		///</summary>
		///<param name="constituent">
		///The constituent to be inserted.
		///</param>
		public virtual void Insert(Parse constituent)
		{
			Util.Span constituentSpan = constituent.mSpan;
			if (mSpan.Contains(constituentSpan))
			{
				int currentPart;
				int partCount = mParts.Count;
				for (currentPart = 0; currentPart < partCount; currentPart++)
				{
					Parse subPart = mParts[currentPart];
					Util.Span subPartSpan = subPart.mSpan;
					if (subPartSpan.Start > constituentSpan.End)
					{
						break;
					}
					// constituent Contains subPart
					else if (constituentSpan.Contains(subPartSpan))
					{
						mParts.RemoveAt(currentPart);
						currentPart--;
						constituent.mParts.Add(subPart);
						subPart.Parent = constituent;
						partCount = mParts.Count;
					}
					else if (subPartSpan.Contains(constituentSpan)) 
					{
						//System.Console.WriteLine("Parse.insert:subPart contains con");
						subPart.Insert(constituent);
						return;
					}
				}
				mParts.Insert(currentPart, constituent);
				constituent.Parent = this;
			}
			else
			{
				throw new ParseException("Inserting constituent not contained in the sentence!");
			}
		}
		
		///<summary>
		///Displays this parse using Penn Treebank-style formatting.
		///</summary>
		public virtual string Show()
		{
			StringBuilder buffer = new StringBuilder();
			int start = mSpan.Start;
			if (mType != MaximumEntropyParser.TokenNode)
			{
				buffer.Append("(");
				buffer.Append(mType + " ");
			}
			
			foreach (Parse childParse in mParts)
			{
				Util.Span childSpan = childParse.mSpan;
				if (start < childSpan.Start)
				{
					//System.Console.Out.WriteLine("pre " + start + " " + childSpan.Start);
					buffer.Append(mText.Substring(start, (childSpan.Start) - (start)));
				}
				buffer.Append(childParse.Show());
				start = childSpan.End;
			}
			buffer.Append(mText.Substring(start, (mSpan.End) - (start)));
			if (mType != MaximumEntropyParser.TokenNode)
			{
				buffer.Append(")");
			}
			return buffer.ToString();
		}
	
		/// <summary>
		/// Computes the head parses for this parse and its sub-parses and stores this information
		/// in the parse data structure. 
		/// </summary>
		/// <param name="rules">
		/// The head rules which determine how the head of the parse is computed.
		/// </param>
		public virtual void UpdateHeads(IHeadRules rules)
		{
			if (mParts != null && mParts.Count != 0)
			{
				for (int currentPart = 0, partCount = mParts.Count; currentPart < partCount; currentPart++)
				{
					Parse currentParse = mParts[currentPart];
					currentParse.UpdateHeads(rules);
				}
				mHead = rules.GetHead(mParts.ToArray(), mType);
				if (mHead == null)
				{
					mHead = this;
				}
			}
			else
			{
				mHead = this;
			}
		}
		
		/// <summary>
		/// Returns the parse nodes which are children of this node and which are pos tags.
		/// </summary>
		/// <returns>
		/// the parse nodes which are children of this node and which are pos tags.
		/// </returns>
		public virtual Parse[] GetTagNodes()
		{
            List<Parse> tags = new List<Parse>();
            List<Parse> nodes = new List<Parse>(mParts);
			while (nodes.Count != 0)
			{
				Parse currentParse = nodes[0];
				nodes.RemoveAt(0);
				if (currentParse.IsPosTag)
				{
					tags.Add(currentParse);
				}
				else
				{
					nodes.InsertRange(0, currentParse.GetChildren());
				}
			}
			return tags.ToArray();	
		}

		/// <summary>
		/// Returns the deepest shared parent of this node and the specified node. 
		/// If the nodes are identical then their parent is returned.  
		/// If one node is the parent of the other then the parent node is returned.
		/// </summary>
		/// <param name="node">
		/// The node from which parents are compared to this node's parents.
		/// </param>
		/// <returns>
		/// the deepest shared parent of this node and the specified node.
		/// </returns>
		public virtual Parse GetCommonParent(Parse node)
		{
			if (this == node)
			{
				return this.Parent;
			}
            Util.HashSet<Parse> parents = new Util.HashSet<Parse>();
			Parse parentParse = this;
			while (parentParse != null)
			{
				parents.Add(parentParse);
				parentParse = parentParse.Parent;
			}
			while (node != null)
			{
				if (parents.Contains(node))
				{
					return node;
				}
				node = node.Parent;
			}
			return null;
		}
	
		protected internal void UpdateChildParents()
		{
			foreach (Parse childParse in mParts)
			{
				childParse.Parent = this;
				childParse.UpdateChildParents();
			}
		}

		#region static methods used to create a Parse from a Penn Treebank parse string

		/// <summary>
		/// The pattern used to find the base constituent label of a Penn Treebank labeled constituent.
		/// </summary>
		private static Regex mTypePattern = new Regex("^([^ =-]+)");

		/// <summary>
		/// The pattern used to identify tokens in Penn Treebank labeled constituents.
		/// </summary>
		private static Regex mTokenPattern = new Regex("^[^ ()]+ ([^ ()]+)\\s*\\)");		

		private static string GetType(string rest)
		{
			if (rest.StartsWith("-LCB-"))
			{
				return "-LCB-";
			}
			else if (rest.StartsWith("-RCB-"))
			{
				return "-RCB-";
			}
			else if (rest.StartsWith("-LRB-"))
			{
				return "-LRB-";
			}
			else if (rest.StartsWith("-RRB-"))
			{
				return "-RRB-";
			}
			else
			{
				MatchCollection typeMatches = mTypePattern.Matches(rest);
				if (typeMatches.Count > 0)
				{
                    return typeMatches[0].Groups[1].Value;
				}
			}
			return null;
		}
		
		private static string GetToken(string rest)
		{
			MatchCollection tokenMatches = mTokenPattern.Matches(rest);
			if (tokenMatches.Count > 0)
			{
				return tokenMatches[0].Groups[1].Value;
			}
			return null;
//			int start = rest.IndexOf(" ");
//			if (start > -1)
//			{
//				int end = rest.IndexOfAny(new char[] {'(', ')'}, start); 
//				if  ((end > -1) && (end - start > 1))
//				{
//					return rest.Substring(start + 1, end - start - 1);
//				}
//			}
//			return null;
		}
		
		/// <summary>
		/// Generates a Parse structure from the specified tree-bank style parse string. 
		/// </summary>
		/// <param name="parse">
		/// A tree-bank style parse string.
		/// </param>
		/// <returns>
		/// a Parse structure for the specified tree-bank style parse string.
		/// </returns>
		public static Parse FromParseString(string parse)
		{
			StringBuilder textBuffer = new StringBuilder();
			int offset = 0;

            Stack<Util.Pair<string, int>> parseStack = new Stack<Util.Pair<string, int>>();

            List<Util.Pair<string, Util.Span>> consitutents = new List<Util.Pair<string, Util.Span>>();
			for (int currentChar = 0, charCount = parse.Length; currentChar < charCount; currentChar++)
			{
				char c = parse[currentChar];
				if (c == '(')
				{
					string rest = parse.Substring(currentChar + 1);
					string type = GetType(rest);
					if (type == null)
					{
						throw new ParseException("null type for: " + rest);
					}
					string token = GetToken(rest);
					parseStack.Push(new Util.Pair<string, int>(type, offset));
					if ((object) token != null && type != "-NONE-")
					{
						consitutents.Add(new Util.Pair<string, Util.Span>(MaximumEntropyParser.TokenNode, new Util.Span(offset, offset + token.Length)));
						textBuffer.Append(token).Append(" ");
						offset += token.Length + 1;
					}
				}
				else if (c == ')')
				{
					Util.Pair<string, int> parts = parseStack.Pop();
					string type = parts.FirstValue;
					if (type != "-NONE-")
					{
						int start = parts.SecondValue;
						consitutents.Add(new Util.Pair<string, Util.Span>(parts.FirstValue, new Util.Span(start, offset - 1)));
					}
				}
			}
			string text = textBuffer.ToString();
			Parse rootParse = new Parse(text, new Util.Span(0, text.Length), MaximumEntropyParser.TopNode, 1);
			for (int currentConstituent = 0, constituentCount = consitutents.Count; currentConstituent < constituentCount; currentConstituent++)
			{
                Util.Pair<string, Util.Span> parts = consitutents[currentConstituent];
				string type = parts.FirstValue;
				if (type != MaximumEntropyParser.TopNode)
				{
					Parse newConstituent = new Parse(text, parts.SecondValue, type, 1);
                    rootParse.Insert(newConstituent);
				}
			}
			return rootParse;
		}

		#endregion		
	}
}
