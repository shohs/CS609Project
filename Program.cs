using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cs609.data;
using cs609.utilities;
using cs609.query;

namespace cs609
{
  class Program
  {

    static void RecursiveInsert(string location)
    {

    }

    static void Main(string[] args)
    {
      string command;

      // INode root = DataReader.ParseJSONString("{ \"test\" : \"value\", \"test2\" : \"value 2\", \"subcollection\" : { \"subkey1\" : \"subval1\" } }");
      INode root = DataReader.ParseJSONString(
        "{" +
          "\"students\" : {" +
            "\"shohs\" : { \"first\" : \"Steven\", \"last\" : \"Hohs\"}," +
            "\"jsmith\" : { \"first\" : \"Joe\", \"last\" : \"Smith\"}," +
            "\"fname\" : { \"first\" : \"Fake\", \"last\" : \"Name\"}," +
            "\"fname2\" : { \"first\" : \"Fake\", \"last\" : \"Name\"}" +
          "}," +
          "\"faculty\" : {" +
            "\"sbinc\" : { \"first\" : \"ShouldntBe\", \"last\" : \"Included\"}" +
          "}" +
        "}"
      );
      root.Print(0);

      Console.WriteLine("\nQuery result");
      Query query = QueryParser.ParseQuery("select *.*.first");
      INode result = query.Execute(root);
      if (result != null)
      {
        result.Print(0);
      }

      do
      {
        Console.Write("cs609 > ");
        command = Console.ReadLine();
        
        if (command.StartsWith("insert"))
        {

        }

      } while (!command.Equals("exit"));
    }
  }
}
