using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.SentenceDetect;
using SharpEntropy;

namespace Test
{
    public class InvalidEmailDetectionEventReader: ITrainingEventReader
    {
        private readonly ITrainingDataReader<string> _dataReader;
        private readonly IContextGenerator<string> _contextGenerator;
		
		/// <summary>
		/// Creates a new <code>SentenceDetectionEventReader</code> instance.  A
		/// DefaultEndOfSentenceScanner is used to locate sentence endings.
		/// </summary>
		/// <param name="dataReader">a <code>ITrainingDataReader</code> value
		/// </param>
		public InvalidEmailDetectionEventReader(ITrainingDataReader<string> dataReader): 
            this(dataReader, new InvalidEmailDetectionContextGenerator()){}
		

        public InvalidEmailDetectionEventReader(ITrainingDataReader<string> dataReader, 
            IContextGenerator<string> contextGenerator)
		{
			_dataReader = dataReader;
			_contextGenerator = contextGenerator;
		}
		
		public virtual TrainingEvent ReadNextEvent()
		{
		    var nextToken = _dataReader.NextToken();
            // split on tab
            var parts = nextToken.Split('\t');
            var isInvalid = parts.Last() == "1";
            var type = isInvalid ? "INV" : "OK";
            var email = parts.First();
            var nextEvent = new InvalidEmailDetectionEvent(type, _contextGenerator.GetContext(email));

		    return nextEvent;
		}
		
		public virtual bool HasNext()
		{
		    return _dataReader.HasNext();
		}
		
    }
}
