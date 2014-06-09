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

//This file is based on the Sequence.java source file found in the
//original java implementation of OpenNLP.  That source file contains the following header:

//Copyright (C) 2003 Gann Bierner and Thomas Morton
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

namespace OpenNLP.Tools.Util
{
	/// <summary>Represents a weighted sequence of outcomes. </summary>
	public class Sequence : System.IComparable
	{
		private double mScore;
		private List<string> mOutcomes;
		private List<double> mProbabilities;

		/// <summary>
		/// Returns a list of outcomes for this sequence.
		/// </summary>
		/// <returns> a list of outcomes.
		/// </returns>
		public virtual List<string> Outcomes
		{
			get
			{
				return mOutcomes;
			}
			
		}

		private List<double> Probabilities
		{
			get
			{
				return mProbabilities;
			}
		}

		/// <summary>
		/// Creates a new sequence of outcomes.
		/// </summary>
		public Sequence()
		{
            mOutcomes = new List<string>(1);
            mProbabilities = new List<double>(1);
			mScore = 0;
		}
		
		/// <summary>
		/// Creates a new sequence of outcomes by cloning an existing sequence.
		/// </summary>
		/// <param name="sequenceToCopy">
		/// The sequence to create the clone from.
		/// </param>
		public Sequence(Sequence sequenceToCopy)
		{
			mOutcomes = new List<string>(sequenceToCopy.Outcomes.Count + 1);
			mOutcomes.AddRange(sequenceToCopy.Outcomes);
			
			mProbabilities = new List<double>(sequenceToCopy.Probabilities.Count + 1);
			mProbabilities.AddRange(sequenceToCopy.Probabilities);

			mScore = sequenceToCopy.Score;
		}
		
		/// <summary>
		/// Creates a new sequence of outcomes based on an existing sequence.
		/// </summary>
		/// <param name="sequenceToCopy">
		/// The sequence to base the new sequence on.
		/// </param>
		/// <param name="outcome">
		/// An additional outcome to add onto the sequence.
		/// </param>
		/// <param name="probability">
		/// An existing probability to add onto the sequence.
		/// </param>
		public Sequence(Sequence sequenceToCopy, string outcome, double probability)
		{
			mOutcomes = new List<string>(sequenceToCopy.Outcomes.Count + 1);
			mOutcomes.AddRange(sequenceToCopy.Outcomes);
			mOutcomes.Add(outcome);

			mProbabilities = new List<double>(sequenceToCopy.Probabilities.Count + 1);
			mProbabilities.AddRange(sequenceToCopy.Probabilities);
			mProbabilities.Add(probability);

			mScore = sequenceToCopy.Score + System.Math.Log(probability);
		}
		
		/// <summary>
		/// Creates a new sequence of outcomes based on a list of outcomes.
		/// Each is given a probability of 1.
		/// </summary>
		/// <param name="outcomes">
		/// List of outcomes to create the sequence from.
		/// </param>
		public Sequence(List<string> outcomes)
		{
			mOutcomes = outcomes;
            mProbabilities = new List<double>(mOutcomes.Count);
            for (int currentOutcome = 0; currentOutcome < mOutcomes.Count; currentOutcome++)
            {
                mProbabilities.Add(1);
            }
		}

        /// <summary>
        /// Compares two Sequence objects.
        /// </summary>
        /// <param name="o">
        /// Object to compare this Sequence to.
        /// </param>
        /// <returns>
        /// Value indicating which sequence is the larger.
        /// </returns>
        public virtual int CompareTo(object o)
		{
			Sequence sequence = (Sequence) o;

			if (mScore < sequence.Score)
			{
				return 1;
			}
			if (mScore > sequence.Score)
			{
				return -1;
			}
			return 0;
		}
		
		/// <summary>
		/// Tests for equality of Sequence objects.
		/// </summary>
		/// <param name="o">
		/// Object to compare this Sequence to.
		/// </param>
		/// <returns>
		/// True if the objects are equal; false otherwise.
		/// </returns>
		public override bool Equals (object o)
		{
			if (!(o is Sequence))
			{
				return false;
			}
			Sequence sequence = (Sequence) o;
			return mScore == sequence.Score;
		}  

		public override int GetHashCode ()
		{
			return mScore.GetHashCode();
		}  

		public static bool operator == (Sequence firstSequence, Sequence secondSequence)
		{
			return firstSequence.Score == secondSequence.Score;
		}  

		public static bool operator != (Sequence firstSequence, Sequence secondSequence)
		{
			return firstSequence.Score != secondSequence.Score;
		}  

		public static bool operator < (Sequence firstSequence, Sequence secondSequence)
		{
			return firstSequence.Score < secondSequence.Score;
		}  

		public static bool operator > (Sequence firstSequence, Sequence secondSequence)
		{
			return firstSequence.Score > secondSequence.Score;
		}  

		/// <summary>
		/// Adds an outcome and probability to this sequence.
		/// </summary>
		/// <param name="outcome">
		/// the outcome to be added.
		/// </param>
		/// <param name="probability">
		/// the probability associated with this outcome.
		/// </param>
		public virtual void Add(string outcome, double probability)
		{
			mOutcomes.Add(outcome);
			mProbabilities.Add(probability);
			mScore += System.Math.Log(probability);
		}
		
		/// <summary>
		/// Returns an array of probabilities associated with the outcomes of this sequence.
		/// </summary>
		/// <returns>
		/// an array of probabilities.
		/// </returns>
		public virtual double[] GetProbabilities()
		{
            return mProbabilities.ToArray();
		}
		
		/// <summary>
		/// Populates an array with the probabilities associated with the outcomes of this sequence.</summary>
		/// <param name="probabilities">
		/// a pre-allocated array to use to hold the values of the probabilities of the outcomes for this sequence.
		/// </param>
		public virtual void GetProbabilities(double[] probabilities)
		{
			for (int currentProbability = 0, probabilityCount = mProbabilities.Count; currentProbability < probabilityCount; currentProbability++)
			{
				probabilities[currentProbability] = mProbabilities[currentProbability];
			}
		}
		
		/// <summary>
		/// Returns the score of this sequence.
		/// </summary>
		/// <returns>
		/// The score of this sequence.
		/// </returns>
		public double Score
		{
			get
			{
				return mScore;
			}
		}

		public override string ToString()
		{
			return mScore + " " + mOutcomes.ToString();
		}
	}
}
