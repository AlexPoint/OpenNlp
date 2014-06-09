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

//This file is based on the DiscourseElement.java source file found in the
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
using System.Text;
using System.Collections.Generic;

namespace OpenNLP.Tools.Coreference
{
	/// <summary>
    /// Represents an item in which can be put into the discourse model.  Object which are
	/// to be placed in the discourse model should extend this class.
	/// </summary>
	/// <seealso cref="OpenNLP.Tools.Coreference.DiscourseModel">
	/// </seealso>
	public abstract class DiscourseElement
	{
        private List<Mention.MentionContext> mExtents;
        private int mElementId = -1;
        private Mention.MentionContext mLastExtent;

		/// <summary>
        /// An iterator over the mentions which iteratates through them based on which were most recently mentioned.
        /// </summary>
        public virtual IEnumerable<Mention.MentionContext> RecentMentions
        {
            get
            {
                for (int currentMention = mExtents.Count - 1; currentMention >= 0; currentMention--)
                {
                    yield return mExtents[currentMention];
                }
            }
        }

		/// <summary>
        /// An iterator over the mentions which iteratates through them based on their occurance in the document.
        /// </summary>
        public virtual IEnumerable<Mention.MentionContext> Mentions
        {
            get
            {
                for (int currentMention = 0; currentMention < mExtents.Count; currentMention++)
                {
                    yield return mExtents[currentMention];
                }
            }
        }

		/// <summary>
        /// The number of mentions in this element.
        /// </summary>
		public virtual int MentionCount
		{
			get
			{
				return mExtents.Count;
			}
		}

		/// <summary>
        /// The last mention for this element.  For appositives this will be the
		/// first part of the appositive.
		/// </summary>
        public virtual Mention.MentionContext LastExtent
		{
			get
			{
				return mLastExtent;
			}	
		}
		
        /// <summary>
        /// The id associated with this element.
        /// </summary>
		public virtual int Id
		{
			get
			{
				return mElementId;
			}
			set
			{
				mElementId = value;
			}
		}

		/// <summary>
        /// Creates a new discourse element which contains the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The mention which begins this discourse element.
		/// </param>
        protected DiscourseElement(Mention.MentionContext mention)
		{
			mExtents = new List<Mention.MentionContext>(1);
			mLastExtent = mention;
			mExtents.Add(mention);
		}
		
		/// <summary>
        /// Adds the specified mention to this discourse element.
        /// </summary>
		/// <param name="mention">
        /// The mention to be added.
		/// </param>
        public virtual void AddMention(Mention.MentionContext mention)
		{
			mExtents.Add(mention);
			mLastExtent = mention;
		}
		
		public override string ToString()
		{
            StringBuilder buffer = new StringBuilder();
            buffer.Append("[ ").Append(mExtents[0].ToText()); //.append("<").append(ex.getHeadText()).append(">");
            for (int currentExtent = 1; currentExtent < mExtents.Count; currentExtent++)
            {
                buffer.Append(", ").Append(mExtents[currentExtent].ToText());
            }
            buffer.Append(" ]");
            return buffer.ToString();
		}
	}
}