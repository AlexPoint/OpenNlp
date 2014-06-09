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

//This file is based on the TokenizerME.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

// Copyright (C) 2002 Jason Baldridge, Gann Bierner, and Tom Morton
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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenNLP.Tools.Tokenize
{
	/// <summary>
	/// A Tokenizer for converting raw text into separated tokens.  It uses
	/// Maximum Entropy to make its decisions.  The features are loosely
	/// based on Jeff Reynar's UPenn thesis "Topic Segmentation:
	/// Algorithms and Applications.", which is available from his
	/// homepage: http://www.cis.upenn.edu/~jcreynar.
	/// </summary>
	public class MaximumEntropyTokenizer : ITokenizer
	{
		/// <summary>
		/// the maximum entropy model to use to evaluate contexts.
		/// </summary>
		private SharpEntropy.IMaximumEntropyModel mModel;
		
		/// <summary>
		/// The context generator.
		/// </summary>
        private SharpEntropy.IContextGenerator<Util.Pair<string, int>> mContextGenerator;
		
		internal static Regex AlphaNumeric = new Regex("^[A-Za-z0-9]+$");
		
		/// <summary>
		/// optimization flag to skip alpha numeric tokens for further
		/// tokenization 
		/// </summary>
		private bool mAlphaNumericOptimization;
		
		/// <summary>
		/// list of probabilities for each token returned from call to
		/// Tokenize() 
		/// </summary>
		private List<double> mTokenProbabilities;
		private List<Util.Span> mNewTokens;

		/// <summary>
		/// Used to have the tokenizer ignore tokens which only contain alpha-numeric characters.
		/// </summary>
		virtual public bool AlphaNumericOptimization
		{
			get
			{
				return mAlphaNumericOptimization;
			}
			set
			{
				mAlphaNumericOptimization = value;
			}
		}
		
		/// <summary>
		/// Class constructor which takes the string locations of the
		/// information which the maxent model needs.
		/// </summary>
		public MaximumEntropyTokenizer(SharpEntropy.IMaximumEntropyModel model)
		{
			mContextGenerator = new TokenContextGenerator();
			mAlphaNumericOptimization = false;
			mModel = model;
            mNewTokens = new List<Util.Span>();
			mTokenProbabilities = new List<double>(50);
		}
		
		/// <summary> 
		/// Tokenizes the string.
		/// </summary>
		/// <param name="input">
		/// The string to be tokenized.
		/// </param>
		/// <returns>
		/// A span array containing individual tokens as elements.
		/// </returns>
		public virtual Util.Span[] TokenizePositions(string input)
		{
			Util.Span[] tokens = Split(input);
			mNewTokens.Clear();
			mTokenProbabilities.Clear();
			
			for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
			{
				Util.Span tokenSpan = tokens[currentToken];
				string token = input.Substring(tokenSpan.Start, (tokenSpan.End) - (tokenSpan.Start));
				// Can't tokenize single characters
				if (token.Length < 2)
				{
					mNewTokens.Add(tokenSpan);
					mTokenProbabilities.Add(1.0);
				}
				else if (AlphaNumericOptimization && AlphaNumeric.IsMatch(token))
				{
					mNewTokens.Add(tokenSpan);
					mTokenProbabilities.Add(1.0);
				}
				else
				{
					int startPosition = tokenSpan.Start;
					int endPosition = tokenSpan.End;
					int originalStart = tokenSpan.Start;
					double tokenProbability = 1.0;
					for (int currentPosition = originalStart + 1; currentPosition < endPosition; currentPosition++)
					{
						double[] probabilities = mModel.Evaluate(mContextGenerator.GetContext(new Util.Pair<string, int>(token, currentPosition - originalStart)));
						string bestOutcome = mModel.GetBestOutcome(probabilities);
						
						tokenProbability *= probabilities[mModel.GetOutcomeIndex(bestOutcome)];
						if (bestOutcome == TokenContextGenerator.SplitIndicator)
						{
							mNewTokens.Add(new Util.Span(startPosition, currentPosition));
							mTokenProbabilities.Add(tokenProbability);
							startPosition = currentPosition;
							tokenProbability = 1.0;
						}
					}
					mNewTokens.Add(new Util.Span(startPosition, endPosition));
					mTokenProbabilities.Add(tokenProbability);
				}
			}
			
			return mNewTokens.ToArray();
		}
		
		/// <summary> 
		/// Tokenize a string.
		/// </summary>
		/// <param name="input">
		/// The string to be tokenized.
		/// </param>
		/// <returns>   
		/// A string array containing individual tokens as elements.
		/// </returns>
		public virtual string[] Tokenize(string input)
		{
			Util.Span[] tokenSpans = TokenizePositions(input);
			string[] tokens = new string[tokenSpans.Length];
			for (int currentToken = 0, tokenCount = tokens.Length; currentToken < tokenCount; currentToken++)
			{
				tokens[currentToken] = input.Substring(tokenSpans[currentToken].Start, (tokenSpans[currentToken].End) - (tokenSpans[currentToken].Start));
			}
			return tokens;
		}
		
		/// <summary>
		/// Constructs a list of Span objects, one for each whitespace
		/// delimited token. Token strings can be constructed form these
		/// spans as follows: input.Substring(span.Start, span.End);
		/// </summary>
		/// <param name="input">
		/// string to tokenize.
		/// </param>
		/// <returns> 
		/// Array of spans.
		/// </returns>
		internal static Util.Span[] Split(string input)
		{
			int tokenStart = - 1;
            List<Util.Span> tokens = new List<Util.Span>();
			bool isInToken = false;
			
			//gather up potential tokens
			int endPosition = input.Length;
			for (int currentChar = 0; currentChar < endPosition; currentChar++)
			{
				if (System.Char.IsWhiteSpace(input[currentChar]))
				{
					if (isInToken)
					{
						tokens.Add(new Util.Span(tokenStart, currentChar));
						isInToken = false;
						tokenStart = - 1;
					}
				}
				else
				{
					if (!isInToken)
					{
						tokenStart = currentChar;
						isInToken = true;
					}
				}
			}
			if (isInToken)
			{
				tokens.Add(new Util.Span(tokenStart, endPosition));
			}
			return tokens.ToArray();
		}
		
		/// <summary>
		/// Returns the probabilities associated with the most recent
		/// calls to Tokenize() or TokenizePositions().
		/// </summary>
		/// <returns>
		/// probability for each token returned for the most recent
		/// call to tokenize.  If not applicable an empty array is
		/// returned.
		/// </returns>
		public virtual double[] GetTokenProbabilities()
		{
            return mTokenProbabilities.ToArray();
		}

		public static void Train(SharpEntropy.ITrainingEventReader eventReader, string outputFilename)
		{
			SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer(0.1);
			trainer.TrainModel(100, new SharpEntropy.TwoPassDataIndexer(eventReader, 5));
			SharpEntropy.GisModel tokenizeModel = new SharpEntropy.GisModel(trainer);
			new SharpEntropy.IO.BinaryGisModelWriter().Persist(tokenizeModel, outputFilename);
		}
		
		public static void Train(string input, string output)
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(new System.IO.FileInfo(input).FullName);
			SharpEntropy.ITrainingEventReader eventReader = new TokenEventReader(dataReader);
			Train(eventReader, output);
		}		
	}
}
