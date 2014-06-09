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

//This file is based on the DefaultParse.java source file found in the
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
//GNU General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;

using NameFinder = OpenNLP.Tools.NameFind.EnglishNameFinder;
using Parse = OpenNLP.Tools.Parser.Parse;
using ParserME = OpenNLP.Tools.Parser.MaximumEntropyParser;
using Span = OpenNLP.Tools.Util.Span;
namespace OpenNLP.Tools.Coreference.Mention
{
	
	/// <summary>
    /// This class is a wrapper for {@link OpenNLP.Tools.Parser.Parse} mapping it to the API specified in
    /// {@link OpenNLP.Tools.Coreference.Mention.Parse}.
	/// This allows coreference to be done on the output of the parser.
	/// </summary>
	public class DefaultParse : AbstractParse
	{
		public override int SentenceNumber
		{
			get
			{
				return mSentenceNumber;
			}
		}

        public override List<IParse> NamedEntities
		{
			get
			{
                List<Parser.Parse> names = new List<Parser.Parse>();
				List<Parser.Parse> kids = new List<Parser.Parse>(mParse.GetChildren());

				while (kids.Count > 0)
				{
                    Parser.Parse currentParse = kids[0];
					kids.RemoveAt(0);
                    
					if (mEntitySet.Contains(currentParse.Type))
					{
						names.Add(currentParse);
					}
					else
					{
						kids.AddRange(currentParse.GetChildren());
					}
				}
                return CreateParses(names.ToArray());
			}	
		}

        public override List<IParse> Children
		{
			get
			{
                return CreateParses(mParse.GetChildren());
			}
		}

		public override List<IParse> SyntacticChildren
		{
			get
			{
                List<Parser.Parse> kids = new List<Parser.Parse>(mParse.GetChildren());
				for (int childIndex = 0; childIndex < kids.Count; childIndex++)
				{
                    Parser.Parse currentKid = kids[childIndex];
                    if (mEntitySet.Contains(currentKid.Type))
					{
                        //remove currentKid
						kids.RemoveAt(childIndex);

                        //and replace it with its children
                        kids.InsertRange(childIndex, currentKid.GetChildren());
                        
                        //set childIndex back by one so we process the parses we just added
						childIndex--;
					}
				}
                return CreateParses(kids.ToArray());
			}
			
		}
        public override List<IParse> Tokens
		{
			get
			{
                List<Parser.Parse> tokens = new List<Parser.Parse>();
				List<Parser.Parse> kids = new List<Parser.Parse>(mParse.GetChildren());
				while (kids.Count > 0)
				{
                    OpenNLP.Tools.Parser.Parse currentParse = kids[0];
					kids.RemoveAt(0);
                    
					if (currentParse.IsPosTag)
					{
						tokens.Add(currentParse);
					}
					else
					{
                        kids.AddRange(currentParse.GetChildren());
					}
				}
                return CreateParses(tokens.ToArray());
			}
		}

		public override string SyntacticType
		{
			get
			{
				if (mEntitySet.Contains(mParse.Type))
				{
					return null;
				}
				else
				{
					return mParse.Type;
				}
			}
		}

		public override string EntityType
		{
			get
			{
				if (mEntitySet.Contains(mParse.Type))
				{
					return mParse.Type;
				}
				else
				{
					return null;
				}
			}
		}

		public override bool ParentNac
		{
			get
			{
				Parser.Parse parent = mParse.Parent;
				while (parent != null)
				{
					if (parent.Type == "NAC")
					{
						return true;
					}
					parent = parent.Parent;
				}
				return false;
			}
		}

		public override IParse Parent
		{
			get
			{
                Parser.Parse parent = mParse.Parent;
				if (parent == null)
				{
					return null;
				}
				else
				{
					return new DefaultParse(parent, mSentenceNumber);
				}
			}
		}

		public override bool IsNamedEntity
		{
			get
			{
				if (mEntitySet.Contains(mParse.Type))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public override bool IsNounPhrase
		{
			get
			{
				return mParse.Type == "NP";
			}
		}

		public override bool IsSentence
		{
			get
			{
				return mParse.Type == ParserME.TopNode;
			}
		}

		public override bool IsToken
		{
			get
			{
				return mParse.IsPosTag;
			}
		}

		public override int EntityId
		{
			get
			{
				return -1;
			}
		}

		public override Span Span
		{
			get
			{
				return mParse.Span;
			}
		}

		public override IParse PreviousToken
		{
			get
			{
                Parser.Parse parent = mParse.Parent;
                Parser.Parse node = mParse;
				int index = -1;
				//find parent with previous children
				while (parent != null && index < 0)
				{
					index = parent.IndexOf(node) - 1;
					if (index < 0)
					{
						node = parent;
						parent = parent.Parent;
					}
				}
				//find right-most child which is a token
				if (index < 0)
				{
					return null;
				}
				else
				{
                    Parser.Parse currentParse = parent.GetChildren()[index];
					while (!currentParse.IsPosTag)
					{
                        Parser.Parse[] kids = currentParse.GetChildren();
						currentParse = kids[kids.Length - 1];
					}
					return new DefaultParse(currentParse, mSentenceNumber);
				}
			}
		}

		public override IParse NextToken
		{
			get
			{
                Parser.Parse parent = mParse.Parent;
                Parser.Parse node = mParse;
				int index = -1;
				//find parent with subsequent children
				while (parent != null)
				{
					index = parent.IndexOf(node) + 1;
					if (index == parent.ChildCount)
					{
						node = parent;
						parent = parent.Parent;
					}
					else
					{
						break;
					}
				}
				//find left-most child which is a token
				if (parent == null)
				{
					return null;
				}
				else
				{
                    Parser.Parse currentParse = parent.GetChildren()[index];
					while (!currentParse.IsPosTag)
					{
						currentParse = currentParse.GetChildren()[0];
					}
					return new DefaultParse(currentParse, mSentenceNumber);
				}
			}
		}

        public virtual Parser.Parse Parse
		{
			get
			{
				return mParse;
			}
		}

        private Parser.Parse mParse;
		private int mSentenceNumber;
		
        private static Util.Set<string> mEntitySet = new Util.HashSet<string>(new List<string>(NameFinder.NameTypes));

        public DefaultParse(Parser.Parse parse, int sentenceNumber)
		{
			mParse = parse;
			mSentenceNumber = sentenceNumber;
		}

        private List<IParse> CreateParses(Parser.Parse[] parses)
		{
            List<IParse> newParses = new List<IParse>(parses.Length);
			for (int parseIndex = 0, parseCount = parses.Length; parseIndex < parseCount; parseIndex++)
			{
				newParses.Add(new DefaultParse(parses[parseIndex], mSentenceNumber));
			}
			return newParses;
		}
		
		public override int CompareTo(object obj)
		{
			if (obj == this)
			{
				return 0;
			}
			DefaultParse p = (DefaultParse) obj;
			if (mSentenceNumber < p.mSentenceNumber)
			{
				return -1;
			}
			else if (mSentenceNumber > p.mSentenceNumber)
			{
				return 1;
			}
			else
			{
				return mParse.Span.CompareTo(p.Span);
			}
		}
		
		public override string ToString()
		{
			return mParse.ToString();
		}
		
		public  override bool Equals(object obj)
		{
			return (this.mParse == ((DefaultParse) obj).mParse);
		}
		
		public override int GetHashCode()
		{
			return (mParse.GetHashCode());
		}
		
	}
}