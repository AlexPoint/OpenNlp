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

//This file is based on the TokEventCollector.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2000 Jason Baldridge and Gann Bierner
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
using System.Collections.Generic;
using System.IO;

namespace OpenNLP.Tools.Tokenize
{
	/// <summary>
	/// Generate event contexts for maxent decisions for tokenization detection.
	/// This is currently not used.
	/// </summary>
	public class TokenEventReader : SharpEntropy.ITrainingEventReader
	{
        private static readonly SharpEntropy.IContextGenerator<Util.Pair<string, int>> mContextGenerator = new TokenContextGenerator();
		private StreamReader mStreamReader;
        private List<SharpEntropy.TrainingEvent> mEventList = new List<SharpEntropy.TrainingEvent>();
		private int mCurrentEvent = 0;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public TokenEventReader(StreamReader dataReader)
		{
			mStreamReader = dataReader;
			string nextLine = mStreamReader.ReadLine();
			if (nextLine != null)
			{
				AddEvents(nextLine);
			}
		}
		
		private void AddEvents(string line)
		{
			string[] spacedTokens = line.Split(' ');
			for (int currentToken = 0; currentToken < spacedTokens.Length; currentToken++)
			{
				string buffer = spacedTokens[currentToken];
				if (MaximumEntropyTokenizer.AlphaNumeric.IsMatch(buffer))
				{
					int lastIndex = buffer.Length - 1;
					for (int index = 0; index < buffer.Length; index++)
					{
                        string[] context = mContextGenerator.GetContext(new Util.Pair<string, int>(buffer, index));
						if (index == lastIndex)
						{
							mEventList.Add(new SharpEntropy.TrainingEvent("T", context));
						}
						else
						{
							mEventList.Add(new SharpEntropy.TrainingEvent("F", context));
						}
					}
				}
			}
		}

		public virtual bool HasNext()
		{
			return (mCurrentEvent < mEventList.Count);
		}
		
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			SharpEntropy.TrainingEvent trainingEvent = mEventList[mCurrentEvent];
			mCurrentEvent++;
			if (mEventList.Count == mCurrentEvent)
			{
				mCurrentEvent = 0;
				mEventList.Clear();
				string nextLine = mStreamReader.ReadLine();
				if (nextLine != null)
				{
					AddEvents(nextLine);
				}
			}
			return trainingEvent;
		}
	}
}
