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

//This file is based on the Mention.java source file found in the
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

namespace OpenNLP.Tools.Coreference.Mention
{
	
	/// <summary>
    /// Data structure representation of a mention.
    /// </summary>
	public class Mention : System.IComparable
	{
        /// <summary>
        /// Represents the character offset for this extent.
        /// </summary>
        private Util.Span mSpan;

        /// <summary>
        /// A string representing the type of this extent.  This is helpful for determining
        /// which piece of code created a particular extent.
        /// </summary>
        private string mType;

        /// <summary>
        /// The entity id indicating which entity this extent belongs to.  This is only
        /// used when training a coreference classifier.
        /// </summary>
        private int mId;

        /// <summary>
        /// Represents the character offsets of the the head of this extent. 
        /// </summary>
        private Util.Span mHeadSpan;

        /// <summary>
        /// The parse node that this extent is based on. 
        /// </summary>
        private IParse mParse;

        /// <summary>
        /// A string representing the name type for this extent. 
        /// </summary>
        private string mNameType;

		/// <summary>
        /// Returns the character offsets for this extent.
        /// </summary>
        public virtual Util.Span Span
		{
			get
			{
				return mSpan;
			}
		}

		/// <summary>
        /// Returns the character offsets for the head of this extent.
        /// </summary>
        public virtual Util.Span HeadSpan
		{
			get
			{
				return mHeadSpan;
			}
		}

		/// <summary>
		/// The parse node that this extent is based on or null if the extent is newly created.
		/// </summary>
		public virtual IParse Parse
		{
			get
			{
				return mParse;
			}
			set
			{
				mParse = value;
			}
		}

		/// <summary>
        /// Returns the id associated with this mention.
        /// </summary>
		public virtual int Id
		{
			get
			{
				return mId;
			}
			set
			{
				mId = value;
			}
		}

        protected internal string Type
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
        /// The named-entity category associated with this mention.
        /// </summary>
        public string NameType
        {
            get
            {
                return mNameType;
            }
            protected internal set
            {
                mNameType = value;
            }
        }

        public Mention(Util.Span span, Util.Span headSpan, int entityId, IParse parse, string extentType)
		{
			mSpan = span;
			mHeadSpan = headSpan;
			mId = entityId;
			mType = extentType;
			mParse = parse;
		}

        public Mention(Util.Span span, Util.Span headSpan, int entityId, IParse parse, string extentType, string nameType)
		{
			mSpan = span;
			mHeadSpan = headSpan;
			mId = entityId;
			mType = extentType;
			mParse = parse;
			mNameType = nameType;
		}
		
		public Mention(Mention mention) : this(mention.mSpan, mention.mHeadSpan, mention.mId, mention.mParse, mention.mType, mention.mNameType)
		{
		}
		
		public virtual int CompareTo(object obj)
		{
			Mention e = (Mention) obj;
			return (mSpan.CompareTo(e.Span));
		}
		
		public override string ToString()
		{
			return "mention(span=" + mSpan + ",hs=" + mHeadSpan + ", type=" + mType + ", id=" + mId + " " + mParse + " )";
		}
	}
}