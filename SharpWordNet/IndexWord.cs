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

using System;

namespace SharpWordNet
{
	/// <summary>
	/// Summary description for IndexWord.
	/// </summary>
	public class IndexWord
	{
		private string mLemma;
		private string mPartOfSpeech;
		private string[] mRelationTypes = null;
		private int mTagSenseCount = 0;
		private int[] mSynsetOffsets = null;
			
		public int[] SynsetOffsets
		{
			get
			{
				return mSynsetOffsets;
			}
		}

        public string Lemma
        {
            get
            {
                return mLemma;
            }
        }

        public int SenseCount
        {
            get
            {
                if (mSynsetOffsets == null)
                {
                    return 0;
                }
                else
                {
                    return mSynsetOffsets.Length;
                }
            }
        }

		public int TagSenseCount
		{
			get
			{
				return mTagSenseCount;
			}
		}

		public string[] RelationTypes
		{
			get
			{
				return mRelationTypes;
			}
		}

		public IndexWord(string lemma, string partOfSpeech, string[] relationTypes, int[] synsetOffsets, int tagSenseCount)
		{
            mLemma = lemma;
            mPartOfSpeech = partOfSpeech;
            mRelationTypes = relationTypes;
            mSynsetOffsets = synsetOffsets;
            mTagSenseCount = tagSenseCount;
		}
	}
}
