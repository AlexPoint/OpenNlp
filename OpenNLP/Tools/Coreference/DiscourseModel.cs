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

//This file is based on the DiscourseModel.java source file found in the
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

namespace OpenNLP.Tools.Coreference
{
	/// <summary>
    /// Represents the elements which are part of a discourse.
    /// </summary>
	public class DiscourseModel
	{
        private List<DiscourseEntity> mEntities;
        private int mNextEntityId = 1;

		/// <summary>
        /// The number of entities in this discourse model
        /// </summary>
		public virtual int EntityCount
		{
			get
			{
				return mEntities.Count;
			}	
		}

		/// <summary>
        /// The entities in the discourse model
        /// </summary>
		public virtual DiscourseEntity[] Entities
		{
			get
			{
                return mEntities.ToArray();
			}
		}

		/// <summary> 
		/// Creates a new discourse model.
		/// </summary>
		public DiscourseModel()
		{
            mEntities = new List<DiscourseEntity>();
		}
		
		/// <summary>
        /// Indicates that the specified entity has been mentioned.</summary>
        /// <param name="entity">
        /// The entity which has been mentioned.
		/// </param>
		public virtual void MentionEntity(DiscourseEntity entity)
		{
			bool isContained;
			isContained = mEntities.Contains(entity);
			mEntities.Remove(entity);
			if (isContained)
			{
				mEntities.Insert(0, entity);
			}
			else
			{
				throw new InvalidOperationException("DiscourseModel.MentionEntity: failed to remove " + entity);
			}
		}
		
		/// <summary>
        /// Returns the entity at the specified index.
        /// </summary>
		/// <param name="index">
        /// the index of the entity to be returned.
		/// </param>
		/// <returns>
        /// the entity at the specified index.
		/// </returns>
		public virtual DiscourseEntity GetEntity(int index)
		{
			return mEntities[index];
		}
		
		/// <summary>
        /// Adds the specified entity to this discourse model.
        /// </summary>
        /// <param name="entity">
        /// the entity to be added to the model. 
		/// </param>
		public virtual void AddEntity(DiscourseEntity entity)
		{
			entity.Id = mNextEntityId;
			mNextEntityId++;
			mEntities.Insert(0, entity);
		}
		
		/// <summary>
        /// Merges the specified entities into a single entity with the specified confidence.
        /// </summary>
        /// <param name="firstEntity">
        /// The first entity. 
		/// </param>
        /// <param name="secondEntity">
        /// The second entity.
		/// </param>
		/// <param name="confidence">
        /// The confidence.
		/// </param>
		public virtual void MergeEntities(DiscourseEntity firstEntity, DiscourseEntity secondEntity, float confidence)
		{
			foreach (Mention.MentionContext mc in secondEntity.Mentions)
            {
				firstEntity.AddMention(mc);
			}
			mEntities.Remove(secondEntity);
		}
		
		/// <summary>
        /// Removes all elements from this discourse model.
        /// </summary>
		public virtual void Clear()
		{
			mEntities.Clear();
		}
	}
}