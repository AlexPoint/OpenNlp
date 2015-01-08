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

//This file is based on the MaxentCompatibilityModel.java source file found in the
//original java implementation of OpenNLP.  

using System;

namespace OpenNLP.Tools.Coreference.Similarity
{
	public class MaximumEntropyCompatibilityModel
	{
		private double mMinimumGenderProbability = 0.66;
		private double mMinimumNumberProbability = 0.66;
		
		private static ITestGenderModel mGenderModel;
		private static ITestNumberModel mNumberModel;
		
		private bool mDebugOn = false;
		
		public MaximumEntropyCompatibilityModel(string coreferencePath)
		{
			mGenderModel = GenderModel.TestModel(coreferencePath + "/gen");
			mNumberModel = NumberModel.TestModel(coreferencePath + "/num");
		}
		
		public virtual Gender ComputeGender(Context context)
		{
			Gender gender;
			double[] genderDistribution = mGenderModel.GenderDistribution(context);
			if (mDebugOn)
			{
				Console.Error.WriteLine("MaxentCompatibilityModel.computeGender: " + context.ToString() + " m=" + genderDistribution[mGenderModel.MaleIndex] + " f=" + genderDistribution[mGenderModel.FemaleIndex] + " n=" + genderDistribution[mGenderModel.NeuterIndex]);
			}
			if (mGenderModel.MaleIndex >= 0 && genderDistribution[mGenderModel.MaleIndex] > mMinimumGenderProbability)
			{
				gender = new Gender(GenderEnum.Male, genderDistribution[mGenderModel.MaleIndex]);
			}
			else if (mGenderModel.FemaleIndex >= 0 && genderDistribution[mGenderModel.FemaleIndex] > mMinimumGenderProbability)
			{
				gender = new Gender(GenderEnum.Female, genderDistribution[mGenderModel.FemaleIndex]);
			}
			else if (mGenderModel.NeuterIndex >= 0 && genderDistribution[mGenderModel.NeuterIndex] > mMinimumGenderProbability)
			{
				gender = new Gender(GenderEnum.Neuter, genderDistribution[mGenderModel.NeuterIndex]);
			}
			else
			{
				gender = new Gender(GenderEnum.Unknown, mMinimumGenderProbability);
			}
			return gender;
		}
		
		public virtual Number ComputeNumber(Context context)
		{
			double[] numberDistribution = mNumberModel.NumberDistribution(context);
			Number number;
			if (numberDistribution[mNumberModel.SingularIndex] > mMinimumNumberProbability)
			{
				number = new Number(NumberEnum.Singular, numberDistribution[mNumberModel.SingularIndex]);
			}
			else if (numberDistribution[mNumberModel.PluralIndex] > mMinimumNumberProbability)
			{
				number = new Number(NumberEnum.Plural, numberDistribution[mNumberModel.PluralIndex]);
			}
			else
			{
				number = new Number(NumberEnum.Unknown, mMinimumNumberProbability);
			}
			return number;
		}
	}
}