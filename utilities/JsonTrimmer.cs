using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs609.utilities
{
    public class JsonTrimmer
    {
        public static string TrimTail(string originalContent)
        {
            var stringvalue = originalContent.ToString();
            stringvalue = stringvalue.TrimEnd(',');
            stringvalue += "}";
            return stringvalue;
        }
    }
}
