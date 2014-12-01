using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.SentenceDetect;
using SharpEntropy;

namespace Test
{
    public class MaximumEntropyInvalidEmailDetector
    {
        // Properties ---------------

        /// <summary>
        /// The maximum entropy model to use to evaluate contexts.
        /// </summary>
        private readonly IMaximumEntropyModel _model;

        /// <summary>
        /// The feature context generator.
        /// </summary>
        private readonly IContextGenerator<string> _contextGenerator;
        
        /// <summary>
        /// The list of probabilities associated with each decision
        /// </summary>
        private readonly double _invalidEmailProbability;


        // Constructors ------------------

		/// <summary>
		/// Constructor which takes a IMaximumEntropyModel and calls the three-arg
		/// constructor with that model, a SentenceDetectionContextGenerator, and the
		/// default end of sentence scanner.
		/// </summary>
		/// <param name="model">
		/// The MaxentModel which this SentenceDetectorME will use to
		/// evaluate end-of-sentence decisions.
		/// </param>
		public MaximumEntropyInvalidEmailDetector(IMaximumEntropyModel model)
		{
            _invalidEmailProbability = 50;
		    _contextGenerator = new InvalidEmailDetectionContextGenerator();
		    _model = model;
		}
		
		

        // Methods ----------------
		
		/// <summary>
		/// Returns the probability associated with the most recent
		/// calls to Detect().
		/// </summary>
		/// <returns>
		/// Probability for the last email tested to be invalid.
		/// </returns>
		public virtual double GetProbability()
		{
            return _invalidEmailProbability;
		}

		/// <summary> 
		/// Tests the probabilty of an email to be invalid.
		/// </summary>
		/// <param name="email">
		/// The email to be processed.
		/// </param>
		/// <returns>   
		/// A string array containing individual sentences as elements.
		/// </returns>
		public virtual double Detect(string email)
		{
            var context = _contextGenerator.GetContext(email);
            double[] probabilities = _model.Evaluate(context);

		    return probabilities.First();
		}
		
		
        // Utilities ----------------------------
		
		
		public static GisModel TrainModel(string inFile, int iterations, int cut)
		{
		    return TrainModel(new List<string>() {inFile}, iterations, cut);
		}

        public static GisModel TrainModel(IEnumerable<string> files, int iterations, int cut)
        {
            var trainer = new GisTrainer();

            foreach (var file in files)
            {
                using (var streamReader = new StreamReader(file))
                {
                    ITrainingDataReader<string> dataReader = new PlainTextByLineDataReader(streamReader);
                    ITrainingEventReader eventReader = new InvalidEmailDetectionEventReader(dataReader);

                    trainer.TrainModel(eventReader, iterations, cut);
                }
            }

            return new GisModel(trainer);
        }
    }
}
