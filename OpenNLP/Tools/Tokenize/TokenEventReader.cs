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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace OpenNLP.Tools.Tokenize
{
	/// <summary>
	/// Generate event contexts for maxent decisions for tokenization detection.
	/// </summary>
	public class TokenEventReader : SharpEntropy.ITrainingEventReader
	{
        private static readonly SharpEntropy.IContextGenerator<Tuple<string, int>> ContextGenerator = new TokenContextGenerator();
		private readonly StreamReader _streamReader;
        private readonly List<SharpEntropy.TrainingEvent> _eventList = new List<SharpEntropy.TrainingEvent>();
		private int _currentEvent = 0;
	    private readonly char _tokenSeparator;
		
        // Constructors ---------------

		public TokenEventReader(StreamReader dataReader, char tokenSeparator)
		{
		    _tokenSeparator = tokenSeparator;
			_streamReader = dataReader;
			string nextLine = _streamReader.ReadLine();
			if (nextLine != null)
			{
				AddEvents(nextLine);
			}
		}

        // Methods --------------------
		
		private void AddEvents(string line)
		{
		    string[] wordsWithSeparatorToken = line.Split(' ');
		    foreach (string wordWithSeparatorToken in wordsWithSeparatorToken)
		    {
		        var parts = wordWithSeparatorToken.Split(_tokenSeparator);
		        var indicesOfSeparators = new List<int>();
		        for (var i = 1; i < parts.Length; i++)
		        {
		            var indexOfSeparator = parts.Where((p, index) => index < i).Sum(p => p.Length);
                    indicesOfSeparators.Add(indexOfSeparator);
		        }

		        var word = string.Join("", parts);
		        for (int index = 0; index < word.Length; index++)
		        {
		            string[] context = ContextGenerator.GetContext(new Tuple<string, int>(word, index));

		            var outcome = indicesOfSeparators.Contains(index) ? "T" : "F";
                    var trainingEvent = new SharpEntropy.TrainingEvent(outcome, context);
		            _eventList.Add(trainingEvent);
		        }
		    }
		}

	    public virtual bool HasNext()
		{
			return (_currentEvent < _eventList.Count);
		}
		
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			SharpEntropy.TrainingEvent trainingEvent = _eventList[_currentEvent];
			_currentEvent++;
			if (_eventList.Count == _currentEvent)
			{
				_currentEvent = 0;
				_eventList.Clear();
				string nextLine = _streamReader.ReadLine();
				if (nextLine != null)
				{
					AddEvents(nextLine);
				}
			}
			return trainingEvent;
		}
	}
}
