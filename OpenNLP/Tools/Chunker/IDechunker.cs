using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Chunker
{
    interface IDechunker
    {
        /// <summary>
        /// Find the dechunker operations corresponding to the input chunks
        /// </summary>
        /// <param name="chunks">the chunks to glue back together</param>
        /// <returns>the merge operations to detokenize the input chunks</returns>
        DechunkOperation[] GetDechunkerOperations(string[] chunks);

        /// <summary>
        /// Detokenize the input chunks into a string. Tokens which
        /// are connected without a space inbetween can be separated by
        /// a split marker.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="splitMarker">the split marker or null</param>
        /// <returns></returns>
        string Dechunk(string[] chunks, string splitMarker = "");
    }

    /// <summary>
    /// This enum contains an operation for every token to merge the
    /// tokens together to their detokenized form.
    /// </summary>
    public enum DechunkOperation
    {
        /// <summary>
        /// The current chunk should be attached to the begin token on the right side.
        /// </summary>
        MERGE_TO_RIGHT,

        /// <summary>
        /// The current chunk should be attached to the string on the left side.
        /// </summary>
        MERGE_TO_LEFT,

        /// <summary>
        /// The current chunk should be attached to the string on the left side, as
        /// well as to the begin token on the right side.
        /// </summary>
        MERGE_BOTH,

        /// <summary>
        /// The current chunk should be attached to the string on the left and the right sides
        /// only if both are words (with letters)
        /// </summary>
        MERGE_BOTH_IF_SURROUNDED_BY_WORDS,

        /// <summary>
        /// Attaches the chunk to the right token on first occurrence, and
        /// to the token on the left side on the second occurrence.
        /// </summary>
        RIGHT_LEFT_MATCHING,

        /// <summary>
        /// Do not perform a merge operation for this chunk, but is possible that another
        /// token can be attached to the left or right side of this one.
        /// </summary>
        NO_OPERATION
    }
}
