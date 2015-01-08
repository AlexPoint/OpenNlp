using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Ling
{
    /// <summary>
    /// Something that implements the <code>HasCategory</code> interface knows about categories.
    /// 
    /// @author Christopher Manning
    /// 
    /// Code...
    /// </summary>
    public interface IHasCategory
    {
        /// <summary>
        /// Returns the category value of the label (or null if none).
        /// </summary>
        string Category();

        /// <summary>
        /// Sets the category value for the label (if one is stored).
        /// </summary>
        void SetCategory(string category);

    }
}