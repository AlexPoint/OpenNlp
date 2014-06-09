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

//This file is based on the POSDictionaryWriter.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2004 Thomas Morton
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
using System.IO;

namespace OpenNLP.Tools.PosTagger
{
	/// <summary>
	/// Class that helps generate part-of-speech lookup list files.
	/// </summary>
	public class PosLookupListWriter
	{		
		private string mDictionaryFile;
		private Dictionary<string, Util.Set<string>> mDictionary;
		private Dictionary<string, int> mWordCounts;
		
		/// <summary>
		/// Creates a new part-of-speech lookup list, specifying the location to write it to.
		/// </summary>
		/// <param name="file">
		/// File to write the new list to.
		/// </param>
		public PosLookupListWriter(string file)
		{
			mDictionaryFile = file;
            mDictionary = new Dictionary<string, Util.Set<string>>();
            mWordCounts = new Dictionary<string, int>();
		}
		
		/// <summary>
		/// Adds an entry to the lookup list in memory, ready for writing to file.
		/// </summary>
		/// <param name="word">
		/// The word for which an entry should be added.
		/// </param>
		/// <param name="tag">
		/// The tag that should be marked as valid for this word.
		/// </param>
		public virtual void AddEntry(string word, string tag)
		{
            Util.Set<string> tags;
            if (mDictionary.ContainsKey(word))
            {
                tags = mDictionary[word];
            }
            else
            {
                tags = new Util.Set<string>();
                mDictionary.Add(word, tags);
            }
			tags.Add(tag);
			
			if (!(mWordCounts.ContainsKey(word)))
			{
				mWordCounts.Add(word, 1);
            }
			else
			{
				mWordCounts[word]++;
			}
		}
		
		/// <summary>
		/// Write the lookup list entries to file with a default cutoff of 5.
		/// </summary>
		public virtual void Write()
		{
			Write(5);
		}
		
		/// <summary>
		/// Write the lookup list entries to file.
		/// </summary>
		/// <param name="cutoff">
		/// The number of times a word must have been added to the lookup list for it to be considered important
		/// enough to write to file.
		/// </param>
		public virtual void Write(int cutoff)
		{
			using (StreamWriter writer = new StreamWriter(mDictionaryFile))
			{
                foreach (string word in mDictionary.Keys)
                {
                    if (mWordCounts[word] > cutoff)
                    {
                        writer.Write(word);
                        Util.Set<string> tags = mDictionary[word];
                        foreach (string tag in tags)
                        {
                            writer.Write(" ");
                            writer.Write(tag);
                        }
                        writer.Write(System.Environment.NewLine);
                    }
                }
				writer.Close();
			}
		}
	}
}
