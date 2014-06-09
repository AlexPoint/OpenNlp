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

//This file is based on the DiscourseEntity.java source file found in the
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

namespace OpenNLP.Tools.Coreference
{
	/// <summary>
    /// Represents an entity in a discourse model.
    /// </summary>
	public class DiscourseEntity : DiscourseElement
	{
        private string mCategory;
        private OpenNLP.Tools.Util.Set<string> mSynsets;
        private Similarity.GenderEnum mGender;
        private double mGenderProbability;
        private Similarity.NumberEnum mNumber;
        private double mNumberProbability;
		
		/// <summary>
        /// The semantic category of this entity.  This field is used to associated named-entity categories with an entity.
        /// </summary>
		public virtual string Category
		{
			get
			{
				return mCategory;
			}
			set
			{
				mCategory = value;
			}
		}

		/// <summary>
        /// The set of synsets associated with this entity.
        /// </summary>
		public virtual Util.Set<string> Synsets
		{
			get
			{
				return mSynsets;
			}	
		}

		/// <summary>
        /// The gender associated with this entity.
        /// </summary>
		public virtual Similarity.GenderEnum Gender
		{
			get
			{
				return mGender;
			}
			set
			{
				mGender = value;
			}
		}

		/// <summary>
        /// The probability for the gender associated with this entity.
        /// </summary>
		public virtual double GenderProbability
		{
			get
			{
				return mGenderProbability;
			}
			set
			{
				mGenderProbability = value;
			}	
		}

		// <summary>
        /// The number associated with this entity.
        /// </summary>
        public virtual Similarity.NumberEnum Number
		{
			get
			{
				return mNumber;
			}
			set
			{
				mNumber = value;
			}	
		}

		/// <summary>
        /// The probability for the number associated with this entity.
        /// </summary>
		public virtual double NumberProbability
		{
			get
			{
				return mNumberProbability;
			}
			set
			{
				mNumberProbability = value;
			}
		}
		
		/// <summary>
        /// Creates a new entity based on the specified mention and its specified gender and number properties.
        /// </summary>
		/// <param name="mention">
        /// The first mention of this entity.
		/// </param>
		/// <param name="gender">
        /// The gender of this entity.
		/// </param>
		/// <param name="genderProbability">
        /// The probability that the specified gender is correct.
		/// </param>
		/// <param name="number">
        /// The number for this entity.
		/// </param>
		/// <param name="numberProbability">
        /// The probability that the specified number is correct.
		/// </param>
        public DiscourseEntity(Mention.MentionContext mention, Similarity.GenderEnum gender, double genderProbability, Similarity.NumberEnum number, double numberProbability) : base(mention)
		{
			mGender = gender;
			mGenderProbability = genderProbability;
			mNumber = number;
			mNumberProbability = numberProbability;
		}
		
		/// <summary>
        /// Creates a new entity based on the specified mention.
        /// </summary>
		/// <param name="mention">
        /// The first mention of this entity.
		/// </param>
        public DiscourseEntity(Mention.MentionContext mention) : base(mention)
		{
            mGender = Similarity.GenderEnum.Unknown;
            mNumber = Similarity.NumberEnum.Unknown;
		}
	}
}