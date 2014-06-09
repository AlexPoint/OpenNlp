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

namespace OpenNLP.Tools.SentenceDetect
{
	/// <summary>
	/// An implementation of ITrainingEventReader which assumes that it is receiving
	/// its data as one (valid) sentence per token.  The default DataReader
	/// to use with this class is PlainTextByLineDataReader, but you can
	/// provide other types of ITrainingDataReaders if you wish to receive data from
	/// sources other than plain text files; however, be sure that each
	/// token your ITrainingDataReader returns is a valid sentence.
	/// </summary>
	public class SentenceDetectionEventReader : SharpEntropy.ITrainingEventReader
	{
		private SharpEntropy.ITrainingDataReader<string> mDataReader;
		private string mNext;
		private SentenceDetectionEvent mHead, mTail;
        private SharpEntropy.IContextGenerator<Util.Pair<System.Text.StringBuilder, int>> mContextGenerator;
		private System.Text.StringBuilder mBuffer = new System.Text.StringBuilder();
		private IEndOfSentenceScanner mScanner;
		
		/// <summary>
		/// Creates a new <code>SentenceDetectionEventReader</code> instance.  A
		/// DefaultEndOfSentenceScanner is used to locate sentence endings.
		/// </summary>
		/// <param name="dataReader">a <code>ITrainingDataReader</code> value
		/// </param>
		public SentenceDetectionEventReader(SharpEntropy.ITrainingDataReader<string> dataReader) : this(dataReader, new DefaultEndOfSentenceScanner(), new SentenceDetectionContextGenerator(DefaultEndOfSentenceScanner.GetEndOfSentenceCharacters()))
		{
		}
		
		/// <summary>
		/// Class constructor which uses the EndOfSentenceScanner to locate
		/// sentence endings.
		/// </summary>
		public SentenceDetectionEventReader(SharpEntropy.ITrainingDataReader<string> dataReader, IEndOfSentenceScanner scanner) : this(dataReader, scanner, new SentenceDetectionContextGenerator(DefaultEndOfSentenceScanner.GetEndOfSentenceCharacters()))
		{
		}

        public SentenceDetectionEventReader(SharpEntropy.ITrainingDataReader<string> dataReader, IEndOfSentenceScanner scanner, SharpEntropy.IContextGenerator<Util.Pair<System.Text.StringBuilder, int>> contextGenerator)
		{
			mDataReader = dataReader;
			mScanner = scanner;
			mContextGenerator = contextGenerator;
			if (mDataReader.HasNext())
			{
				string current = mDataReader.NextToken();
				if (mDataReader.HasNext())
				{
					mNext = mDataReader.NextToken();
				}
				AddNewEvents(current);
			}
		}
		
		public virtual SharpEntropy.TrainingEvent ReadNextEvent()
		{
			SentenceDetectionEvent topEvent = mHead;
			mHead = mHead.NextEvent;
			if (null == mHead)
			{
				mTail = null;
			}
			return topEvent;
		}
		
		private void AddNewEvents(string token)
		{
			System.Text.StringBuilder buffer = mBuffer;
			buffer.Append(token.Trim());
			int sentenceEndPosition = buffer.Length - 1;
			//add following word to stringbuilder
			if (mNext != null && token.Length > 0)
			{
				int positionAfterFirstWordInNext = mNext.IndexOf(" ");
				if (positionAfterFirstWordInNext != - 1)
				{
					// should maybe changes this so that it usually adds a space
					// before the next sentence, but sometimes leaves no space.
					buffer.Append(" ");
					buffer.Append(mNext.Substring(0, (positionAfterFirstWordInNext) - (0)));
				}
			}
			
			for (System.Collections.IEnumerator iterator = mScanner.GetPositions(buffer).GetEnumerator(); iterator.MoveNext(); )
			{
				int candidate = (int) iterator.Current;
                Util.Pair<System.Text.StringBuilder, int> pair = new Util.Pair<System.Text.StringBuilder, int>(buffer, candidate);
				string type = (candidate == sentenceEndPosition) ? "T" : "F";
				SentenceDetectionEvent sentenceEvent = new SentenceDetectionEvent(type, mContextGenerator.GetContext(pair));
				
				if (null != mTail)
				{
					mTail.NextEvent = sentenceEvent;
					mTail = sentenceEvent;
				}
				else if (null == mHead)
				{
					mHead = sentenceEvent;
				}
				else if (null == mHead.NextEvent)
				{
					mHead.NextEvent = mTail = sentenceEvent;
				}
			}
			
			buffer.Length = 0;
		}
		
		public virtual bool HasNext()
		{
			if (null != mHead)
			{
				return true;
			}
			
			while (null == mHead && (object) mNext != null)
			{
				string current = mNext;
				if (mDataReader.HasNext())
				{
					mNext = (mDataReader.NextToken());
				}
				else
				{
					mNext = null;
				}
				AddNewEvents(current);
			}
			return (null != mHead);
		}
		
//		[STAThread]
//		public static void  Main(System.String[] args)
//		{
//			EventStream es = new SDEventStream(new PlainTextByLineDataStream(new System.IO.StreamReader(System.Console.In)));
//			while (es.hasNext())
//			{
//				System.Console.Out.WriteLine(es.nextEvent());
//			}
//		}
	}
}
