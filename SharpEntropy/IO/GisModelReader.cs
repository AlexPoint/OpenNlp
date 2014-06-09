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

//This file is based on the GISModelReader.java source file found in the
//original java implementation of MaxEnt.  That source file contains the following header:

// Copyright (C) 2001 Jason Baldridge and Gann Bierner
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;

namespace SharpEntropy.IO
{
	/// <summary>
	/// Abstract parent class for readers of GIS models.
	/// </summary>
	/// <author>
	/// Jason Baldridge
	/// </author>
	/// <author>
	/// Richard J. Northedge
	/// </author>
	/// <version>
	/// based on GISModelReader.java, $Revision: 1.5 $, $Date: 2004/06/11 20:51:36 $
	/// </version>
	public abstract class GisModelReader : IGisModelReader
	{
		private char[] mSpaces;
	
		private int mCorrectionConstant;
		private double mCorrectionParameter;
		private string[] mOutcomeLabels;
		private int[][] mOutcomePatterns;
		private int mPredicateCount;
		private Dictionary<string, PatternedPredicate> mPredicates;

		/// <summary>
		/// The number of predicates contained in the model.
		/// </summary>
		protected int PredicateCount
		{
			get
			{
				return mPredicateCount;
			}
		}

		#region read data from the model file
		/// <summary>
		/// Retrieve a model from disk.
		/// 
		/// <p>This method delegates to worker methods for each part of this 
		/// sequence.  If you are creating a reader that conforms largely to this
		/// sequence but varies at one or more points, override the relevant worker
		/// method(s) to achieve the required format.</p>
		/// 
		/// <p>If you are creating a reader for a format which does not follow this
		/// sequence at all, override this method and ignore the
		/// other ReadX methods provided in this abstract class.</p>
		/// </summary>
		/// <remarks>
		/// Thie method assumes that models are saved in the
		/// following sequence:
		/// 
		/// <p>GIS (model type identifier)</p>
		/// <p>1. the correction constant (int)</p>
		/// <p>2. the correction constant parameter (double)</p>
		/// <p>3. outcomes</p>
		/// <p>3a. number of outcomes (int)</p>
		/// <p>3b. outcome names (string array - length specified in 3a)</p>
		/// <p>4. predicates</p>
		/// <p>4a. outcome patterns</p>
		/// <p>4ai. number of outcome patterns (int)</p>
		/// <p>4aii. outcome pattern values (each stored in a space delimited string)</p>
		/// <p>4b. predicate labels</p>
		/// <p>4bi. number of predicates (int)</p>
		/// <p>4bii. predicate names (string array - length specified in 4bi)</p>
		/// <p>4c. predicate parameters (double values)</p>
		/// </remarks>
		protected virtual void ReadModel()
		{
			mSpaces = new Char[] {' '}; //cached constant to improve performance
			CheckModelType();
			mCorrectionConstant = ReadCorrectionConstant();
			mCorrectionParameter = ReadCorrectionParameter();
			mOutcomeLabels = ReadOutcomes();
			ReadPredicates(out mOutcomePatterns, out mPredicates);
		}
	
		/// <summary>
		/// Checks the model file being read from begins with the sequence of characters
		/// "GIS".
		/// </summary>
		protected virtual void CheckModelType()
		{
			string modelType = ReadString();
			if (modelType != "GIS") 
			{
				throw new ApplicationException("Error: attempting to load a " + modelType + " model as a GIS model." + " You should expect problems.");
			}
		}

		/// <summary>
		/// Reads the correction constant from the model file.
		/// </summary>
		protected virtual int ReadCorrectionConstant()
		{
			return ReadInt32();
		}
	
		/// <summary>
		/// Reads the correction constant parameter from the model file.
		/// </summary>
		protected virtual double ReadCorrectionParameter()
		{
			return ReadDouble();
		}
		
		/// <summary>
		/// Reads the outcome names from the model file.
		/// </summary>
		protected virtual string[] ReadOutcomes()
		{
			int outcomeCount = ReadInt32();
			string[] outcomeLabels = new string[outcomeCount];
			for (int currentLabel = 0; currentLabel < outcomeCount; currentLabel++)
			{
				outcomeLabels[currentLabel] = ReadString();
			}
			return outcomeLabels;
		}

		/// <summary>
		/// Reads the predicate information from the model file, placing the data in two
		/// structures - an array of outcome patterns, and a Dictionary of predicates
		/// keyed by predicate name.
		/// </summary>
        protected virtual void ReadPredicates(out int[][] outcomePatterns, out Dictionary<string, PatternedPredicate> predicates)
		{
			outcomePatterns = ReadOutcomePatterns();
			string[] asPredicateLabels = ReadPredicateLabels();
			predicates = ReadParameters(outcomePatterns, asPredicateLabels);
		}

		/// <summary>
		/// Reads the outcome pattern information from the model file.
		/// </summary>
		protected virtual int[][] ReadOutcomePatterns()
		{
			//get the number of outcome patterns (that is, the number of unique combinations of outcomes in the model)
			int outcomePatternCount = ReadInt32();
			//initialize an array of outcome patterns.  Each outcome pattern is itself an array of integers
			int[][] outcomePatterns = new int[outcomePatternCount][];
			//for each outcome pattern
			for (int currentOutcomePattern = 0; currentOutcomePattern < outcomePatternCount; currentOutcomePattern++)
			{
				//read a space delimited string from the model file containing the information for the integer array.
				//The first value in the integer array is the number of predicates related to this outcome pattern; the
				//other values make up the outcome IDs for this pattern.
				string[] tokens = ReadString().Split(mSpaces);
				//convert this string to the array of integers required for the pattern
				int[] patternData = new int[tokens.Length];
				for (int currentPatternValue = 0; currentPatternValue < tokens.Length; currentPatternValue++) 
				{
					patternData[currentPatternValue] = int.Parse(tokens[currentPatternValue], System.Globalization.CultureInfo.InvariantCulture);
				}
				outcomePatterns[currentOutcomePattern] = patternData;
			}
			return outcomePatterns;
		}
	
		/// <summary>
		/// Reads the outcome labels from the model file.
		/// </summary>
		protected virtual string[] ReadPredicateLabels()
		{
			mPredicateCount = ReadInt32();
			string[] predicateLabels = new string[mPredicateCount];
			for (int currentPredicate = 0; currentPredicate < mPredicateCount; currentPredicate++)
			{
				predicateLabels[currentPredicate] = ReadString();
			}
			return predicateLabels;
		}

		/// <summary>
		/// Reads the predicate parameter information from the model file.
		/// </summary>
        protected virtual Dictionary<string, PatternedPredicate> ReadParameters(int[][] outcomePatterns, string[] predicateLabels)
		{
            Dictionary<string, PatternedPredicate> predicates = new Dictionary<string, PatternedPredicate>(predicateLabels.Length);
			int parameterIndex = 0;
	
			for (int currentOutcomePattern = 0; currentOutcomePattern < outcomePatterns.Length; currentOutcomePattern++)
			{
				for (int currentOutcomeInfo = 0; currentOutcomeInfo < outcomePatterns[currentOutcomePattern][0]; currentOutcomeInfo++)
				{
					double[] parameters = new double[outcomePatterns[currentOutcomePattern].Length - 1];
					for (int currentParameter = 0; currentParameter < outcomePatterns[currentOutcomePattern].Length - 1; currentParameter++)
					{
						parameters[currentParameter] = ReadDouble();
					}
					predicates.Add(predicateLabels[parameterIndex], new PatternedPredicate(currentOutcomePattern, parameters));
					parameterIndex++;
				}
			}
			return predicates;
		}

		/// <summary>
		/// Implement as needed for the format the model is stored in.
		/// </summary>
		protected abstract int ReadInt32();
			
		/// <summary>
		/// Implement as needed for the format the model is stored in.
		/// </summary>
		protected abstract double ReadDouble();
			
		/// <summary>
		/// Implement as needed for the format the model is stored in.
		/// </summary>
		protected abstract string ReadString();

#endregion

		#region implement IGisModelReader
		/// <summary>
		/// The model's correction constant.
		/// </summary>
		public int CorrectionConstant
		{
			get
			{
				return mCorrectionConstant;
			}
		}
	
		/// <summary>
		/// The model's correction constant parameter.
		/// </summary>
		public double CorrectionParameter
		{
			get
			{
				return mCorrectionParameter;
			}
		}
	
		/// <summary>
		/// Returns the labels for all the outcomes in the model.
		/// </summary>
		/// <returns>
		/// String array containing outcome labels.
		/// </returns>
		public string[] GetOutcomeLabels()
		{
			return mOutcomeLabels;
		}
	
		/// <summary>
		/// Returns the outcome patterns in the model.
		/// </summary>
		/// <returns>
		/// Array of integer arrays containing the information for
		/// each outcome pattern in the model.
		/// </returns>
		public int[][] GetOutcomePatterns()
		{
			return mOutcomePatterns;
		}

		/// <summary>
		/// Returns the predicates in the model.
		/// </summary>
		/// <returns>
		/// Dictionary containing PatternedPredicate objects keyed
		/// by predicate label.
		/// </returns>
        public Dictionary<string, PatternedPredicate> GetPredicates()
		{
			return mPredicates;
		}

		/// <summary>
		/// Returns model information for a predicate, given the predicate label.
		/// </summary>
		/// <param name="predicateLabel">
		/// The predicate label to fetch information for.
		/// </param>
		/// <param name="featureCounts">
		/// Array to be passed in to the method; it should have a length equal to the number of outcomes
		/// in the model.  The method increments the count of each outcome that is active in the specified
		/// predicate.
		/// </param>
		/// <param name="outcomeSums">
		/// Array to be passed in to the method; it should have a length equal to the number of outcomes
		/// in the model.  The method adds the parameter values for each of the active outcomes in the
		/// predicate.
		/// </param>
		public virtual void GetPredicateData(string predicateLabel, int[] featureCounts, double[] outcomeSums)
		{
            if (mPredicates.ContainsKey(predicateLabel))
            {
			    PatternedPredicate predicate = mPredicates[predicateLabel];
				int[] activeOutcomes = mOutcomePatterns[predicate.OutcomePattern];
					
				for (int currentActiveOutcome = 1; currentActiveOutcome < activeOutcomes.Length; currentActiveOutcome++)
				{
					int outcomeIndex = activeOutcomes[currentActiveOutcome];
					featureCounts[outcomeIndex]++;
					outcomeSums[outcomeIndex] += predicate.GetParameter(currentActiveOutcome - 1);
				}
			}
		}
	
#endregion
	
	}
}
