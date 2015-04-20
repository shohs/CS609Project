using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using cs609.data;
using cs609.utilities;

namespace cs609.query
{
  public class QueryParser
  {
    public QueryParser(string queryString)
    {
      _query = queryString.Trim();
    }

    public IQuery ParseQuery()
    {
      if (MatchKeyword("select "))
      {
        string collectionList = ParseCollectionList();
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
        string argument;
        argument = ParseJSONString();

        if (!MatchKeyword("into "))
        {
          throw new ArgumentException("No \"into\" clause provided in insert");
        }

        string collectionList = ParseCollectionList();
        if (collectionList.Length == 0)
        {
          throw new ArgumentException("Insert query does not specify an insert location");
        }
        string[] keys = collectionList.Split('.');

        INode toInsert = DataReader.ParseJSONString(argument);
        InsertQuery query = null;
        for (int i = keys.Length - 1; i >= 0; i--)
        {
          query = new InsertQuery(toInsert, keys[i], query);
        }
        return query;
      }
      else if (MatchKeyword("update "))
      {
          string argument;
          string collectionList = ParseCollectionList();
          if (collectionList.Length == 0)
          {
              throw new ArgumentException("Update query does not specify a field");
          }
          if (!MatchKeyword("value "))
          {
              throw new ArgumentException("No \"value\" clause provided in update");
          }
          argument = ParseJSONString();
          string[] keys = collectionList.Split('.');
          UpdateQuery query = null;
          INode toUpdate;
          if (argument.Contains('{'))
          {
             toUpdate = DataReader.ParseJSONString(argument);
          }
          else
          {
              toUpdate = new PrimitiveNode<string>(argument);
          }
          
          for (int i = keys.Length - 1; i >= 0; i--)
          {
              query = new UpdateQuery(toUpdate, keys[i], query);
          }
          return query;
      }
      else
      {
        return null;
      }
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
      if (position + keyword.Length >= _query.Length)
      {
        return false;
      }
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
      int endPosition;
      if (_query[position] == '{')
      {
        endPosition = position + 1;
        int braceCount = 1;
        while (braceCount != 0 && endPosition < _query.Length)
        {
          if (_query[endPosition] == '{')
          {
            ++braceCount;
          }
          else if (_query[endPosition] == '}')
          {
            --braceCount;
          }
          ++endPosition;
        }
      }
      else
      {
        endPosition = position;
        while (endPosition < _query.Length && (char.IsLetterOrDigit(_query[endPosition]) || _query[endPosition] == '"'))
        {
          ++endPosition;
        }
      }
      string result = _query.Substring(position, endPosition - position);
      position = endPosition;
      ConsumeWhiteSpace();
      return result;
    }

    private string ParseCollectionList()
    {
      StringBuilder collectionList = new StringBuilder();
      int endPosition = position;
      bool isDone = false;

      while (!isDone)
      {
        endPosition = ConsumeWhiteSpace(endPosition);
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
