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

//This file is based on the ReverseListIterator.java source file found in the
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
namespace OpenNLP.Tools.Util
{
	
	/// <summary> An iterator for a list which returns values in the opposite order as the typical list iterator.</summary>
    //public class ReverseListIterator : System.Collections.IEnumerator
    //{
    //    public virtual System.Object Current
    //    {
    //        get
    //        {
    //            return (list[index--]);
    //        }
			
    //    }
		
    //    private int index;
    //    private System.Collections.IList list;
		
    //    public ReverseListIterator(System.Collections.IList list)
    //    {
    //        index = list.Count - 1;
    //        this.list = list;
    //    }
		
    //    public virtual bool MoveNext()
    //    {
    //        return (index >= 0);
    //    }
		
    //    //UPGRADE_NOTE: The equivalent of method 'java.util.Iterator.remove' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
    //    public virtual void  remove()
    //    {
    //        throw (new System.NotSupportedException());
    //    }
    //    //UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
    //    virtual public void  Reset()
    //    {
    //    }
    //}
}