using System;
using System.Data.SQLite;
using System.IO;
using System.Data;

namespace SharpEntropy.IO
{
	/// <summary>
	/// Summary description for SqliteGisModelWriter.
	/// </summary>
	public class SqliteGisModelWriter : GisModelWriter
	{

		private SQLiteConnection mDataConnection;
		private SQLiteTransaction mDataTransaction;
		private SQLiteCommand mDataCommand;

		public SqliteGisModelWriter(){}

		/// <summary>
		/// Takes a GIS model and a file and writes the model to that file.
		/// </summary>
		/// <param name="model">
		/// The GisModel which is to be persisted.
		/// </param>
		/// <param name="fileName">
		/// The name of the file in which the model is to be persisted.
		/// </param>
		public void Persist(GisModel model, string fileName)
		{
			Initialize(model);
			PatternedPredicate[] predicates = GetPredicates();

			if (	File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			using (mDataConnection = new SQLiteConnection("Data Source=" + fileName + ";New=True;Compress=False;Synchronous=Off;UTF8Encoding=True;Version=3"))
			{
				mDataConnection.Open();
				mDataCommand = mDataConnection.CreateCommand();
				CreateDataStructures();

                using (mDataTransaction = mDataConnection.BeginTransaction())
                {
                    mDataCommand.Transaction = mDataTransaction;

                    CreateModel(model.CorrectionConstant, model.CorrectionParameter);
                    InsertOutcomes(model.GetOutcomeNames());
                    InsertPredicates(predicates);
                    InsertPredicateParameters(model.GetOutcomePatterns(), predicates);

                    mDataTransaction.Commit();
                }
				mDataConnection.Close();
			}
		}

		private void CreateDataStructures()
		{
			mDataCommand.CommandText = "CREATE TABLE Model(CorrectionConstant INTEGER NOT NULL, CorrectionParameter FLOAT NOT NULL)";
			mDataCommand.ExecuteNonQuery();

			mDataCommand.CommandText = "CREATE TABLE Outcome(OutcomeID INTEGER NOT NULL PRIMARY KEY UNIQUE, OutcomeLabel VARCHAR(255) NOT NULL)";
			mDataCommand.ExecuteNonQuery();

			mDataCommand.CommandText = "CREATE TABLE Predicate(PredicateID INTEGER NOT NULL PRIMARY KEY UNIQUE, PredicateLabel VARCHAR(255) NOT NULL)";
			mDataCommand.ExecuteNonQuery();

			mDataCommand.CommandText = "CREATE UNIQUE INDEX IX_PredicateLabel ON Predicate (PredicateLabel ASC)";
			mDataCommand.ExecuteNonQuery();

			mDataCommand.CommandText = "CREATE TABLE PredicateParameter(PredicateID INTEGER NOT NULL, OutcomeID INTEGER NOT NULL, Parameter FLOAT NOT NULL, PRIMARY KEY(PredicateID, OutcomeID))";
			mDataCommand.ExecuteNonQuery();
		}

		private void CreateModel(int correctionConstant, double correctionParameter)
		{
			mDataCommand.CommandText = "INSERT INTO Model values (?, ?)";
            mDataCommand.Parameters.Clear();

            var correctionConstantParameter = new SQLiteParameter
            {
                DbType = DbType.Int32, 
                Value = correctionConstant
            };
		    mDataCommand.Parameters.Add(correctionConstantParameter);

            var correctionParameterParameter = new SQLiteParameter
            {
                DbType = DbType.Double, 
                Value = correctionParameter
            };
		    mDataCommand.Parameters.Add(correctionParameterParameter);

			mDataCommand.ExecuteNonQuery();
		}

        private void InsertOutcomes(string[] outcomeLabels)
        {
            mDataCommand.CommandText = "INSERT INTO Outcome values (?, ?)";
            mDataCommand.Parameters.Clear();

            var idParameter = new SQLiteParameter {DbType = DbType.Int32};
            mDataCommand.Parameters.Add(idParameter);

            var labelParameter = new SQLiteParameter {DbType = DbType.String};
            mDataCommand.Parameters.Add(labelParameter);

            for (int currentOutcomeId = 0; currentOutcomeId < outcomeLabels.Length; currentOutcomeId++)
            {
                idParameter.Value = currentOutcomeId;
                labelParameter.Value = outcomeLabels[currentOutcomeId];
                mDataCommand.ExecuteNonQuery();
            }
        }

        private void InsertPredicates(PatternedPredicate[] predicates)
        {
            mDataCommand.CommandText = "INSERT INTO Predicate values (?, ?)";
            mDataCommand.Parameters.Clear();

            var idParameter = new SQLiteParameter {DbType = DbType.Int32};
            mDataCommand.Parameters.Add(idParameter);

            var nameParameter = new SQLiteParameter {DbType = DbType.String};
            mDataCommand.Parameters.Add(nameParameter);

            for (int currentPredicate = 0; currentPredicate < predicates.Length; currentPredicate++)
            {
                idParameter.Value = currentPredicate;
                nameParameter.Value = predicates[currentPredicate].Name;
                mDataCommand.ExecuteNonQuery();
            }
        }

        private void InsertPredicateParameters(int[][] outcomePatterns, PatternedPredicate[] predicates)
        {
            mDataCommand.CommandText = "INSERT INTO PredicateParameter values (?, ?, ?)";
            mDataCommand.Parameters.Clear();

            var predicateIdParameter = new SQLiteParameter {DbType = DbType.Int32};
            mDataCommand.Parameters.Add(predicateIdParameter);

            var outcomeIdParameter = new SQLiteParameter {DbType = DbType.Int32};
            mDataCommand.Parameters.Add(outcomeIdParameter);

            var predicateParameterParameter = new SQLiteParameter {DbType = DbType.Double};
            mDataCommand.Parameters.Add(predicateParameterParameter);

            for (int currentPredicate = 0; currentPredicate < predicates.Length; currentPredicate++)
            {
                predicateIdParameter.Value = currentPredicate;
                int[] currentOutcomePattern = outcomePatterns[predicates[currentPredicate].OutcomePattern];
                for (int currentParameter = 0; currentParameter < predicates[currentPredicate].ParameterCount; currentParameter++)
                {
                    outcomeIdParameter.Value = currentOutcomePattern[currentParameter + 1];
                    predicateParameterParameter.Value = predicates[currentPredicate].GetParameter(currentParameter);
                    mDataCommand.ExecuteNonQuery();
                }
            }
        }

		protected override void WriteString(string data)
        {
            throw new InvalidOperationException("This method is not necessary for use");
        }

		protected override void WriteInt32(int data)
        {
            throw new InvalidOperationException("This method is not necessary for use");
        }

        protected override void WriteDouble(double data)
        {
            throw new InvalidOperationException("This method is not necessary for use");
        }
	}
}
