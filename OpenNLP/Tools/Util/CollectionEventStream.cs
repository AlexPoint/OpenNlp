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

//This file is based on the CollectionEventStream.java source file found in the
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
//GNU General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections;

namespace OpenNLP.Tools.Util
{
	/// <summary>
    /// Creates an event stream out of a collection of events.
    /// </summary>
    public class CollectionEventReader : SharpEntropy.ITrainingEventReader
	{
		private IEnumerator mCollection;
		
		public CollectionEventReader(ICollection c)
		{
			mCollection = c.GetEnumerator();
		}

        #region ITrainingEventReader Members

        public virtual bool HasNext()
        {
            return mCollection.MoveNext();
        }

        public SharpEntropy.TrainingEvent ReadNextEvent()
        {
            return (SharpEntropy.TrainingEvent)mCollection.Current;
        }

        #endregion
    }
}