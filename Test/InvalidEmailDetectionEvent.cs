using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEntropy;

namespace Test
{
    public class InvalidEmailDetectionEvent:TrainingEvent
    {
        public InvalidEmailDetectionEvent(string outcome, string[] context) : base(outcome, context)
        {
        }
    }
}
