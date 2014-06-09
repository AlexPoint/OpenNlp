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

//This file is based on the NameFinderEventStream.java source file found in the
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

namespace OpenNLP.Tools.NameFind
{
	/// <summary>
	/// Class for creating a training event reader out of data files for training a name finder.
	/// </summary>
	public class NameFinderEventReader : SharpEntropy.ITrainingEventReader
	{
		private SharpEntropy.ITrainingDataReader<string> mDataReader;
		private SharpEntropy.TrainingEvent[] mEvents;
		private INameContextGenerator mContextGenerator;

		/// <summary>
		/// A mapping between tokens and the name tag assigned to them previously. 
		/// </summary>
		private Dictionary<string, string> mPreviousTags;

		/// <summary>
		/// The index into the array of events.
		/// </summary>
		private int mEventIndex;

		/// <summary>
		/// The last line read in from the data file.
		/// </summary>	
		private string mLine;
			
		/// <summary>
		/// Creates a new event reader based on the specified data reader.
		/// </summary>
		/// <param name="dataReader">
		/// The data stream for this event reader.
		/// </param>
		public NameFinderEventReader(SharpEntropy.ITrainingDataReader<string> dataReader) : this(dataReader, new DefaultNameContextGenerator())
		{
		}
		
		/// <summary>
		/// Creates a new event reader based on the specified data reader using the specified context generator.
		/// </summary>
		/// <param name="dataReader">
		/// The data reader for this event reader.
		/// </param>
		/// <param name="contextGenerator">
		/// The context generator which should be used in the creation of events for this event stream.
		/// </param>
		public NameFinderEventReader(SharpEntropy.ITrainingDataReader<string> dataReader, INameContextGenerator contextGenerator)
		{
			mDataReader = dataReader;
			mContextGenerator = contextGenerator;
			mEventIndex = 0;
            mPreviousTags = new Dictionary<string, string>();

			//prime events with first line of data stream.
			if (mDataReader.HasNext())
			{
				mLine = mDataReader.NextToken();
				if (mLine.Length == 0)
				{
					mPreviousTags.Clear();
				}
				else
				{
					AddEvents(mLine);
				}
			}
			else
			{
				mEvents = new SharpEntropy.TrainingEvent[0];
			}
		}
		
		/// <summary>
		/// Adds name events for the specified sentence.
		/// </summary>
		/// <param name="sentence">
		/// The sentence for which name events should be added.
		/// </param>
		private void AddEvents(string sentence)
		{
			string[] parts = sentence.Split(' ');
			string outcome = MaximumEntropyNameFinder.Other;
            List<string> tokens = new List<string>();
            List<string> outcomesList = new List<string>();
			for (int currentPart = 0, partCount = parts.Length; currentPart < partCount; currentPart++)
			{
				if (parts[currentPart] == "<START>")
				{
					outcome = MaximumEntropyNameFinder.Start;
				}
				else if (parts[currentPart] == "<END>")
				{
					outcome = MaximumEntropyNameFinder.Other;
				}
				else
				{
					//regular token
					tokens.Add(parts[currentPart]);
					outcomesList.Add(outcome);
					if (outcome == MaximumEntropyNameFinder.Start)
					{
						outcome = MaximumEntropyNameFinder.Continue;
					}
				}
			}
			mEvents = new SharpEntropy.TrainingEvent[tokens.Count];
			for (int currentToken = 0, tokenCount = tokens.Count; currentToken < tokenCount; currentToken++)
			{
				mEvents[currentToken] = new SharpEntropy.TrainingEvent(outcomesList[currentToken], mContextGenerator.GetContext(currentToken, tokens, outcomesList, mPreviousTags));
			}
			for (int currentToken = 0, tokenCount = tokens.Count; currentToken < tokenCount; currentToken++)
			{
                mPreviousTags[tokens[currentToken]] =  outcomesList[currentToken];
			}
		}
		
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			if (mEventIndex == mEvents.Length)
			{
                AddEvents(mLine);
                mEventIndex = 0;
                mLine = null;
			}
			return mEvents[mEventIndex++];
		}
		
		public virtual bool HasNext()
		{
			if (mEventIndex < mEvents.Length) 
			{
				return true;
			}
			else if (mLine != null) 
			{ // previous result has not been consumed
				return true;
			}
			//find next non-blank line
			while (mDataReader.HasNext()) 
			{
				mLine = mDataReader.NextToken();
				if (mLine.Length == 0) 
				{
					mPreviousTags.Clear();
				}
				else 
				{
					return true;
				}
			}
			return false;
		}
	}
}
