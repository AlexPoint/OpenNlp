using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Tokenize
{
    /// <summary>
    /// Licensed to the Apache Software Foundation (ASF) under one or more
    /// contributor license agreements.  See the NOTICE file distributed with
    /// this work for additional information regarding copyright ownership.
    /// The ASF licenses this file to You under the Apache License, Version 2.0
    /// (the "License"); you may not use this file except in compliance with
    /// the License. You may obtain a copy of the License at
    /// http://www.apache.org/licenses/LICENSE-2.0
    /// Unless required by applicable law or agreed to in writing, software
    /// distributed under the License is distributed on an "AS IS" BASIS,
    /// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    /// See the License for the specific language governing permissions and
    /// limitations under the License.
    /// </summary>
    public interface IDetokenizer
    {
        /// <summary>
        /// Find the detokenization operations corresponding to the input tokens
        /// </summary>
        /// <param name="tokens">the tokens to detokenize</param>
        /// <returns>the merge operations to detokenize the input tokens</returns>
        DetokenizationOperation[] GetDetokenizationOperations(string[] tokens);

        /// <summary>
        /// Detokenize the input tokens into a string. Tokens which
        /// are connected without a space inbetween can be separated by
        /// a split marker.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="splitMarker">the split marker or null</param>
        /// <returns></returns>
        string Detokenize(string[] tokens, string splitMarker = "");

    }

    /// <summary>
    /// This enum contains an operation for every token to merge the
    /// tokens together to their detokenized form.
    /// </summary>
    public enum DetokenizationOperation
    {
        /// <summary>
        /// The current token should be attached to the begin token on the right side.
        /// </summary>
        MERGE_TO_RIGHT,

        /// <summary>
        /// The current token should be attached to the string on the left side.
        /// </summary>
        MERGE_TO_LEFT,

        /// <summary>
        /// The current token should be attached to the string on the left side, as
        /// well as to the begin token on the right side.
        /// </summary>
        MERGE_BOTH,

        /// <summary>
        /// The current token should be attached to the string on the left and the right sides
        /// only if both are words (with letters)
        /// </summary>
        MERGE_BOTH_IF_SURROUNDED_BY_WORDS,

        /// <summary>
        /// Attaches the token token to the right token on first occurrence, and
        /// to the token on the left side on the second occurrence.
        /// </summary>
        RIGHT_LEFT_MATCHING,

        /// <summary>
        /// Do not perform a merge operation for this token, but is possible that another
        /// token can be attached to the left or right side of this one.
        /// </summary>
        NO_OPERATION
    }

}
