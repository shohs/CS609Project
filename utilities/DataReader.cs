using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using cs609.data;

namespace cs609.utilities
{
  public class DataReader
  {
    public static INode ParseJSONString(string json)
    {
      INode returnNode = null;
      using (var reader = new StringReader(json))
      {
        returnNode = Parse(reader);
      }
      return returnNode;
    }

    private static void ConsumeWhitespace(StringReader reader)
    {
      while (char.IsWhiteSpace((char)reader.Peek()))
      {
        reader.Read();
      }
    }

    private static void MatchCharacter(StringReader reader, char toMatch)
    {
      char c = (char)reader.Read();
      if (c != toMatch)
      {
        throw new ArgumentException("Expected " + toMatch + ", found " + c);
      }
    }

    private static string ReadToken(StringReader reader)
    {
      ConsumeWhitespace(reader);
      MatchCharacter(reader, '"');

      StringBuilder tokenBuilder = new StringBuilder();
      while (reader.Peek() != '"')
      {
        tokenBuilder.Append((char)reader.Read());
      }
      MatchCharacter(reader, '"');
      return tokenBuilder.ToString();
    }

    private static string ReadNumber(StringReader reader)
    {
      ConsumeWhitespace(reader);
      StringBuilder tokenBuilder = new StringBuilder();

      System.Globalization.NumberFormatInfo format = new System.Globalization.NumberFormatInfo();
      while (char.IsDigit((char)reader.Peek()) || format.NumberDecimalSeparator.Contains((char)reader.Peek()) || (char)reader.Peek() == '-')
      {
        tokenBuilder.Append((char)reader.Read());
      }
      return tokenBuilder.ToString();
    }

    private static INode Parse(StringReader reader)
    {
      ConsumeWhitespace(reader);
      if (reader.Peek() == '{')
      {
        return ParseCollection(reader);
      }
      else if (reader.Peek() == '"')
      {
        return ParsePrimitive(reader);
      }
      else
      {
        string num = ReadNumber(reader);
        int iNum;
        double dNum;

        if (int.TryParse(num, out iNum))
        {
          return new PrimitiveNode<int>(iNum);
        }
        else if (double.TryParse(num, out dNum))
        {
          return new PrimitiveNode<double>(dNum);
        }
        else
        {
          throw new ArgumentException("Invalid JSON string passed to DataReader");
        }
      }
    }

    private static INode ParseCollection(StringReader reader)
    {
      MatchCharacter(reader, '{');

      CollectionNode collection = new CollectionNode();

      ConsumeWhitespace(reader);
      if (reader.Peek() == '}')
      {
        return collection;
      }

      // TODO: Make this handle correctly without the repeated code
      string key = ReadToken(reader);

      ConsumeWhitespace(reader);
      MatchCharacter(reader, ':');

      INode value = Parse(reader);
      collection.SetNode(key, value);
      ConsumeWhitespace(reader);

      while (reader.Peek() == ',')
      {
        reader.Read();
        ConsumeWhitespace(reader);

        key = ReadToken(reader);

        ConsumeWhitespace(reader);
        MatchCharacter(reader, ':');
        
        value = Parse(reader);
        collection.SetNode(key, value);
        ConsumeWhitespace(reader);
      }

      ConsumeWhitespace(reader);
      MatchCharacter(reader, '}');

      return collection;
    }

    private static INode ParsePrimitive(StringReader reader)
    {
      return new PrimitiveNode<string>(ReadToken(reader));
    }
  }
}
