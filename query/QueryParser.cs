using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace cs609.query
{
  public class QueryParser
  {
    public static Query ParseQuery(string queryString)
    {
      queryString = queryString.Trim();
      string[] keywords = Regex.Split(queryString, "\\s+");
      if (keywords[0].ToLower().Equals("select"))
      {
        if (keywords.Length < 2) throw new ArgumentException("Select query does not specify a field");
        string[] keys = keywords[1].Split('.');
        SelectQuery query = null;
        for (int i = keys.Length - 1; i >= 0; i--)
        {
          query = new SelectQuery(keys[i], query);
        }
        return query;
      }
      else
      {
        return null;
      }
    }
  }
}
