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
    public QueryParser(string queryString)
    {
      _query = queryString.Trim();
    }

    public Query ParseQuery()
    {
      if (MatchKeyword("select "))
      {
        string collectionList = ParseCollectionList();
        // collectionList = collectionList.Replace(" ", string.Empty);
        if (collectionList.Length == 0)
        {
          throw new ArgumentException("Select query does not specify a field");
        }
        string[] keys = collectionList.Split('.');
        SelectQuery query = null;
        for (int i = keys.Length - 1; i >= 0; i--)
        {
          query = new SelectQuery(keys[i], query);
        }
        return query;
      }
      else if (MatchKeyword("insert "))
      {
        return null;
      }
      else
      {
        return null;
      }
      /*
      string[] keywords = Regex.Split(_query, "\\s+");

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
      else if (keywords[0].ToLower().Equals("insert"))
      {
        return null;
      }
      else
      {
        return null;
      }
      */
    }

    private bool MatchKeyword(string keyword)
    {
      if (CheckKeyword(keyword))
      {
        AdvanceCursor(keyword.Length);
        return true;
      }
      return false;
    }

    private bool CheckKeyword(string keyword)
    {
      return _query.Substring(position, keyword.Length).ToLower().Equals(keyword);
    }

    private void AdvanceCursor(int characters)
    {
      position += characters;
      ConsumeWhiteSpace();
    }

    private void ConsumeWhiteSpace()
    {
      position = ConsumeWhiteSpace(position);
    }

    private int ConsumeWhiteSpace(int cursor)
    {
      while (cursor < _query.Length && char.IsWhiteSpace(_query[cursor]))
      {
        ++cursor;
      }
      return cursor;
    }

    private string ParseJSONString()
    {
      return null;
    }

    private string ParseCollectionList()
    {
      StringBuilder collectionList = new StringBuilder();
      int endPosition = position;
      bool isDone = false;

      while (!isDone)
      {
        while (endPosition < _query.Length && (char.IsLetterOrDigit(_query[endPosition]) || _query[endPosition] == '*'))
        {
          collectionList.Append(_query[endPosition++]);
        }
        endPosition = ConsumeWhiteSpace(endPosition);
        if (endPosition >= _query.Length || _query[endPosition] != '.')
        {
          isDone = true;
        }
        else
        {
          ++endPosition;
          collectionList.Append('.');
        }
      }

      position = endPosition;
      return collectionList.ToString();
    }

    private string _query;
    private int position = 0;
  }
}
