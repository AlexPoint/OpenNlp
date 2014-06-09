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

//This file is based on the ChunkerEventStream.java source file found in the
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

namespace OpenNLP.Tools.Chunker
{
	/// <summary> 
	/// Class for creating an event reader out of data files for training a chunker. 
	/// </summary>
	public class ChunkerEventReader : SharpEntropy.ITrainingEventReader
	{
		private IChunkerContextGenerator mContextGenerator;
        private SharpEntropy.ITrainingDataReader<string> mDataReader;
		private SharpEntropy.TrainingEvent[] mEvents;
		private int mEventIndex;
		
		/// <summary>
		/// Creates a new event reader based on the specified data reader.
		/// </summary>
		/// <param name="dataReader">
		/// The data reader for this event reader.
		/// </param>
        public ChunkerEventReader(SharpEntropy.ITrainingDataReader<string> dataReader)
            : this(dataReader, new DefaultChunkerContextGenerator())
		{
		}
		
		/// <summary>
		/// Creates a new event reader based on the specified data reader using the specified context generator.
		/// </summary>
		/// <param name="dataReader">
		/// The data reader for this event reader.
		/// </param>
		/// <param name="contextGenerator">
		/// The context generator which should be used in the creation of events for this event reader.
		/// </param>
        public ChunkerEventReader(SharpEntropy.ITrainingDataReader<string> dataReader, IChunkerContextGenerator contextGenerator)
		{
			mContextGenerator = contextGenerator;
			mDataReader = dataReader;
			mEventIndex = 0;
			if (dataReader.HasNext())
			{
				AddNewEvents();
			}
			else
			{
				mEvents = new SharpEntropy.TrainingEvent[0];
			}
		}
		
		/// <summary> 
		/// Returns the next TrainingEvent object held in this TrainingEventReader.
		/// </summary>
		/// <returns>
		/// the TrainingEvent object which is next in this TrainingEventReader
		/// </returns>
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			if (mEventIndex == mEvents.Length)
			{
				AddNewEvents();
				mEventIndex = 0;
			}
			return ((SharpEntropy.TrainingEvent) mEvents[mEventIndex++]);
		}
		
		/// <summary> 
		/// Test whether there are any TrainingEvents remaining in this TrainingEventReader.
		/// </summary>
		/// <returns>
		/// true if this TrainingEventReader has more TrainingEvents
		/// </returns>
		public virtual bool HasNext()
		{
			return (mEventIndex < mEvents.Length || mDataReader.HasNext());
		}
		
		private void AddNewEvents()
		{
            List<string> tokenList = new List<string>();
            List<string> tagList = new List<string>();
            List<string> predicateList = new List<string>();
			for (string line = mDataReader.NextToken(); line.Length > 0; line = mDataReader.NextToken())
			{
				string[] parts = line.Split(' ');
				if (parts.Length != 3) 
				{
					//skip this line; it is in error
				}
				else 
				{
					tokenList.Add(parts[0]);
					tagList.Add(parts[1]);
					predicateList.Add(parts[2]);
				}
			}
			mEvents = new SharpEntropy.TrainingEvent[tokenList.Count];
			object[] tokens = tokenList.ToArray();
			string[] tags = tagList.ToArray();
			string[] predicates = predicateList.ToArray();

			for (int eventIndex = 0, eventCount = mEvents.Length; eventIndex < eventCount; eventIndex++)
			{
				mEvents[eventIndex] = new SharpEntropy.TrainingEvent(predicates[eventIndex], mContextGenerator.GetContext(eventIndex, tokens, tags, predicates));
			}
		}
	}
}
