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

//This file is based on the EnglishPOSTaggerME.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2004 Jason Baldridge, Gann Bierner, and Tom Morton
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

namespace OpenNLP.Tools.PosTagger
{
	/// <summary>
	/// A part of speech tagger that uses a model trained on English data from the
	/// Wall Street Journal and the Brown corpus.  The latest model created
	/// achieved >96% accuracy on unseen data.
	/// </summary>	
	public class EnglishMaximumEntropyPosTagger : MaximumEntropyPosTagger
	{
		
        // Constructors ----------

		public EnglishMaximumEntropyPosTagger(string modelFile, PosLookupList dictionary) : 
            base(GetModel(modelFile), new DefaultPosContextGenerator(), dictionary){ }
		
		public EnglishMaximumEntropyPosTagger(string modelFile, string dictionary) : 
            base(GetModel(modelFile), new DefaultPosContextGenerator(), new PosLookupList(dictionary)){ }

		public EnglishMaximumEntropyPosTagger(string modelFile) : 
            base(GetModel(modelFile), new DefaultPosContextGenerator()){ }
		
        
        // Utilities ---------

		private static SharpEntropy.IMaximumEntropyModel GetModel(string name)
		{
			return new SharpEntropy.GisModel(new SharpEntropy.IO.BinaryGisModelReader(name));
		}
	}

}
