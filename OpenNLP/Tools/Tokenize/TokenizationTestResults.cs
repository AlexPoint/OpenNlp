using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Tokenize
{
    /// <summary>
    /// Results of a tokenization test.
    /// </summary>
    public class TokenizationTestResults
    {
        /// <summary>
        /// Nb of words correctly tokenized (excluding whitespace tokenization)
        /// </summary>
        public int NbOfCorrectTokenizations { get; set; }
        /// <summary>
        /// Nb of incorrect tokens (word not tokenized or wrongly tokenized)
        /// </summary>
        public int NbOfIncorrectTokenizations { get; set; }

        /// <summary>
        /// Computes the ratio of correct tokenization (excluding whitespaces)
        /// over all tokenizations.
        /// </summary>
        public float GetAccuracy()
        {
            return (float) (this.NbOfCorrectTokenizations )/
                   (this.NbOfCorrectTokenizations + this.NbOfIncorrectTokenizations);
        }
    }
}
