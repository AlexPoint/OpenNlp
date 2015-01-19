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

//This file is based on the SDEventStream.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

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
using System.Text;
using SharpEntropy;

namespace OpenNLP.Tools.SentenceDetect
{
	/// <summary>
	/// An implementation of ITrainingEventReader which assumes that it is receiving
	/// its data as one (valid) sentence per token. 
	/// The default DataReader to use with this class is PlainTextByLineDataReader, 
	/// but you can provide other types of ITrainingDataReaders if you wish 
	/// to receive data from sources other than plain text files; 
	/// however, be sure that each token your ITrainingDataReader returns is a valid sentence.
	/// </summary>
	public class SentenceDetectionEventReader : ITrainingEventReader
	{
		private readonly ITrainingDataReader<string> _dataReader;
		private string _next;
		private SentenceDetectionEvent _head, _tail;
        private readonly IContextGenerator<Tuple<StringBuilder, int>> _contextGenerator;
		private readonly StringBuilder _buffer = new StringBuilder();
		private readonly IEndOfSentenceScanner _scanner;
		
		/// <summary>
		/// Creates a new <code>SentenceDetectionEventReader</code> instance.
		/// A DefaultEndOfSentenceScanner is used to locate sentence endings.
		/// </summary>
		/// <param name="dataReader">a <code>ITrainingDataReader</code> value
		/// </param>
		public SentenceDetectionEventReader(ITrainingDataReader<string> dataReader): 
            this(dataReader, new DefaultEndOfSentenceScanner(), new SentenceDetectionContextGenerator(DefaultEndOfSentenceScanner.GetDefaultEndOfSentenceCharacters().ToArray())){}
		
		/// <summary>
		/// Class constructor which uses the EndOfSentenceScanner to locate
		/// sentence endings.
		/// </summary>
		public SentenceDetectionEventReader(ITrainingDataReader<string> dataReader, IEndOfSentenceScanner scanner) : 
            this(dataReader, scanner, new SentenceDetectionContextGenerator(scanner.GetPotentialEndOfSentenceCharacters().ToArray())){}

        public SentenceDetectionEventReader(ITrainingDataReader<string> dataReader, IEndOfSentenceScanner scanner, 
            IContextGenerator<Tuple<StringBuilder, int>> contextGenerator)
		{
			_dataReader = dataReader;
			_scanner = scanner;
			_contextGenerator = contextGenerator;
			if (_dataReader.HasNext())
			{
				string current = _dataReader.NextToken();
				if (_dataReader.HasNext())
				{
					_next = _dataReader.NextToken();
				}
				AddNewEvents(current);
			}
		}
		
		public virtual TrainingEvent ReadNextEvent()
		{
			SentenceDetectionEvent topEvent = _head;
			_head = _head.NextEvent;
			if (_head == null)
			{
				_tail = null;
			}
			return topEvent;
		}
		
		private void AddNewEvents(string token)
		{
			StringBuilder buffer = _buffer;
			buffer.Append(token.Trim());
            
            // only one sentence per token, so the end of sentence if at the very end of the token
            int sentenceEndPosition = buffer.Length - 1;

			//add following word to stringbuilder
			if (_next != null && token.Length > 0)
			{
				int positionAfterFirstWordInNext = _next.IndexOf(" ");
				if (positionAfterFirstWordInNext != - 1)
				{
					// should maybe changes this so that it usually adds a space
					// before the next sentence, but sometimes leaves no space.
					buffer.Append(" ");
					buffer.Append(_next.Substring(0, positionAfterFirstWordInNext));
				}
			}

            foreach (var candidate in _scanner.GetPositions(buffer))
			{
                var pair = new Tuple<StringBuilder, int>(buffer, candidate);
				string type = (candidate == sentenceEndPosition) ? "T" : "F";
				var sentenceEvent = new SentenceDetectionEvent(type, _contextGenerator.GetContext(pair));

                if (_tail != null)
				{
					_tail.NextEvent = sentenceEvent;
					_tail = sentenceEvent;
				}
                else if (_head == null)
				{
					_head = sentenceEvent;
				}
				else if (_head.NextEvent == null)
				{
					_head.NextEvent = _tail = sentenceEvent;
				}
			}
			
			buffer.Length = 0;
		}
		
		public virtual bool HasNext()
		{
			if (_head != null)
			{
				return true;
			}

            while (_head == null && _next != null)
			{
				string current = _next;
				_next = _dataReader.HasNext() ? _dataReader.NextToken() : null;
				AddNewEvents(current);
			}
			return (_head != null);
		}
		
//		[STAThread]
//		public static void  Main(System.string[] args)
//		{
//			EventStream es = new SDEventStream(new PlainTextByLineDataStream(new System.IO.StreamReader(System.Console.In)));
//			while (es.hasNext())
//			{
//				System.Console.Out.WriteLine(es.nextEvent());
//			}
//		}
	}
}
