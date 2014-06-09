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

//This file is based on the POSEventCollector.java and POSEventStream.java source files found in the
//original java implementation of OpenNLP.  Those source files contain the following headers:

// Copyright (C) 2003 Tom Morton
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

// Copyright (C) 2002 Jason Baldridge and Gann Bierner
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
using System.Collections;
using System.Collections.Generic;

namespace OpenNLP.Tools.PosTagger
{
	/// <summary> 
	/// An event generator for the maxent POS Tagger.
	/// </summary>
	public class PosEventReader : SharpEntropy.ITrainingEventReader
	{
		private System.IO.TextReader mTextReader;
		private IPosContextGenerator mContextGenerator;
        private List<SharpEntropy.TrainingEvent> mEventList = new List<SharpEntropy.TrainingEvent>();
		private int mCurrentEvent = 0;
		
		public PosEventReader(System.IO.TextReader data) : this(data, new DefaultPosContextGenerator())
		{
		}

		public PosEventReader(System.IO.TextReader data, IPosContextGenerator contextGenerator)
		{
			mContextGenerator = contextGenerator;
			mTextReader = data;
			string nextLine = mTextReader.ReadLine();
			if (nextLine != null)
			{
				AddEvents(nextLine);
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
				string nextLine = mTextReader.ReadLine();
				if (nextLine != null)
				{
					AddEvents(nextLine);
				}
			}
			return trainingEvent;
		}

		private void AddEvents(string line)
		{
			Util.Pair<ArrayList, ArrayList>  linePair = ConvertAnnotatedString(line);
			ArrayList tokens = linePair.FirstValue;
			ArrayList outcomes = linePair.SecondValue;
            List<string> tags = new List<string>();
					
			for (int currentToken = 0; currentToken < tokens.Count; currentToken++)
			{
				string[] context = mContextGenerator.GetContext(currentToken, tokens.ToArray(), tags.ToArray(), null);
				SharpEntropy.TrainingEvent posTrainingEvent = new SharpEntropy.TrainingEvent((string) outcomes[currentToken], context);
				tags.Add((string)outcomes[currentToken]);
				mEventList.Add(posTrainingEvent);
			}
		}

		private static Util.Pair<string, string> Split(string input)
		{
			int splitPosition = input.LastIndexOf("_");
			if (splitPosition == -1)
			{
				System.Console.Out.WriteLine("There is a problem in your training data: " + input + " does not conform to the format WORD_TAG.");
                return new Util.Pair<string, string>(input, "UNKNOWN");
			}
            return new Util.Pair<string, string>(input.Substring(0, (splitPosition) - (0)), input.Substring(splitPosition + 1));
		}
		
		public static Util.Pair<ArrayList, ArrayList> ConvertAnnotatedString(string input)
		{
			ArrayList tokens = new ArrayList();
			ArrayList outcomes = new ArrayList();
			Util.StringTokenizer tokenizer = new Util.StringTokenizer(input);
			string token = tokenizer.NextToken();
			while (token != null)
			{
				Util.Pair<string, string> linePair = Split(token);
				tokens.Add(linePair.FirstValue);
				outcomes.Add(linePair.SecondValue);
				token = tokenizer.NextToken();
			}
			return new Util.Pair<ArrayList, ArrayList>(tokens, outcomes);
		}
			
//		[STAThread]
//		public static void Main(string[] args)
//		{
//			string sData = "the_DT stories_NNS about_IN well-heeled_JJ communities_NNS and_CC developers_NNS";
//			EventCollector oEventCollector = new PosEventCollector(new System.IO.StringReader(sData), new DefaultPosContextGenerator());
//			Event[] aoEvents = oEventCollector.GetEvents();
//			for (int iCurrentEvent = 0; iCurrentEvent < aoEvents.length; iCurrentEvent++)
//			{
//				System.Console.Out.WriteLine(aoEvents[iCurrentEvent].GetOutcome());
//			}
//		}
	}
}
