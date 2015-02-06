using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEntropy;

namespace OpenNLP.Tools.SentenceDetect
{
    public class MultipleFilesPlainTextByLineDataReader: ITrainingDataReader<string>
    {
        private readonly List<StreamReader> _dataReaders;
        private int currentDataReaderIndex;
		private string _nextLine;
		
		/// <summary>
		/// Creates a training data reader for reading text lines from a file or other text stream
		/// </summary>
		/// <param name="dataReaders">StreamReaders containing the source of the training data</param>
        public MultipleFilesPlainTextByLineDataReader(List<StreamReader> dataReaders)
		{
			_dataReaders = dataReaders;
		    currentDataReaderIndex = 0;
			_nextLine = GetNextLine();
		}

        public string GetNextLine()
        {
            if (currentDataReaderIndex >= _dataReaders.Count)
            {
                return null;
            }
            else
            {
                var line = _dataReaders[currentDataReaderIndex].ReadLine();
                if (line == null)
                {
                    currentDataReaderIndex++;
                    return GetNextLine();
                }
                else
                {
                    return line;
                }
            }
        }
		
		/// <summary>Gets the next text line from the training data</summary>
		/// <returns>Next text line from the training data</returns>
		public virtual string NextToken()
		{
			string currentLine = _nextLine;
			_nextLine = GetNextLine();
			return currentLine;
		}
		
		/// <summary>Checks if there is any more training data</summary>
		/// <returns>true if there is more training data to be read</returns>
		public virtual bool HasNext()
		{
			return (_nextLine != null);
		}
    }
}
