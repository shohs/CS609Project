using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs609.utilities
{
    public class JsonTrimmer
    {
        public static string TrimTail(string stringvalue)
        {
            stringvalue = stringvalue.TrimEnd(',');
            stringvalue += "}";
            return stringvalue;
        }
        
        public static void TrimTail(StringBuilder builder)
        {
            builder.Remove(builder.Length - 1, 1);
            builder.Append('}');
        }
    }
}
