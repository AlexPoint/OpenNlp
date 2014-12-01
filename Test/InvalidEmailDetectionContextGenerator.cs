using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpEntropy;

namespace Test
{
    public class InvalidEmailDetectionContextGenerator: IContextGenerator<string>
    {
        public string[] GetContext(string input)
        {
            var results = new List<string>();

            var parts = input.Split('@');
            var domain = parts.Last();
            var domainParts = domain.Split('.');

            results.Add("d=" + domainParts.First());
            results.Add("dExt=" + domainParts.Last());

            return results.ToArray();
        }
    }
}
