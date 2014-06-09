using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace SharpEntropy.IO
{
	/// <summary>
	/// Summary description for SqliteGisModelReader.
	/// </summary>
	public class SqliteGisModelReader : IGisModelReader
	{
		private SQLiteConnection mDataConnection;
		private SQLiteCommand mGetParameterCommand;
		private IDbDataParameter mPredicateParameter;

		private int mCorrectionConstant;
		private double mCorrectionParameter;
		private string[] mOutcomeLabels;

		public SqliteGisModelReader(string fileName)
		{
			mDataConnection = new SQLiteConnection("Data Source=" + fileName + ";Compress=False;Synchronous=Off;UTF8Encoding=True;Version=3");
			mDataConnection.Open();

			SQLiteCommand getModelCommand = mDataConnection.CreateCommand();
			getModelCommand.CommandText = "SELECT CorrectionConstant, CorrectionParameter FROM Model";
			IDataReader reader = getModelCommand.ExecuteReader();
			while (reader.Read())
			{
				mCorrectionConstant = reader.GetInt32(0);
				mCorrectionParameter = reader.GetDouble(1);
			}
			reader.Close();

            List<string> outcomeLabels = new List<string>();

			getModelCommand.CommandText = "SELECT OutcomeLabel FROM Outcome ORDER BY OutcomeID";
			reader = getModelCommand.ExecuteReader();
			while (reader.Read())
			{
				outcomeLabels.Add(reader.GetString(0));
			}
			reader.Close();
			mOutcomeLabels =outcomeLabels.ToArray();

			mGetParameterCommand = mDataConnection.CreateCommand();
			mGetParameterCommand.CommandText = "SELECT PredicateParameter.OutcomeID, PredicateParameter.Parameter FROM PredicateParameter INNER JOIN Predicate ON PredicateParameter.PredicateID = Predicate.PredicateID WHERE Predicate.PredicateLabel = ?";
            mPredicateParameter = new SQLiteParameter();
			mPredicateParameter.DbType = DbType.String;
			mPredicateParameter.Size = 255;
            mGetParameterCommand.Parameters.Add(mPredicateParameter);
		}

		public int CorrectionConstant
		{
			get
			{
				return mCorrectionConstant;
			}
		}
	
		public double CorrectionParameter
		{
			get
			{
				return mCorrectionParameter;
			}
		}
	
		public string[] GetOutcomeLabels()
		{
			return mOutcomeLabels;
		}

		public int[][] GetOutcomePatterns()
		{
            throw new NotImplementedException();
		}

        public Dictionary<string, PatternedPredicate> GetPredicates()
		{
            throw new NotImplementedException();
		}

		public void GetPredicateData(string predicateLabel, int[] featureCounts, double[] outcomeSums)
		{
			mPredicateParameter.Value = predicateLabel;
			SQLiteDataReader getParameterReader = mGetParameterCommand.ExecuteReader();
			int outcomeId;
			double parameter;
			while (getParameterReader.Read())
			{
				outcomeId = getParameterReader.GetInt32(0);
				parameter = getParameterReader.GetDouble(1);
				featureCounts[outcomeId]++;
				outcomeSums[outcomeId] += parameter;
			}
			getParameterReader.Close();
		}
	}
}
