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

    static void Main(string[] args)
    {
      string command;

      // INode root = DataReader.ParseJSONString("{ \"test\" : \"value\", \"test2\" : \"value 2\", \"subcollection\" : { \"subkey1\" : \"subval1\" } }");
      INode root = DataReader.ParseJSONString(
        "{" +
          "\"students\" : {" +
            "\"shohs\"  : { \"first\" : \"Steven\", \"last\" : \"Hohs\",  \"code\" : 1 }," +
            "\"jsmith\" : { \"first\" : \"Joe\",    \"last\" : \"Smith\", \"code\" : 2 }," +
            "\"fname\"  : { \"first\" : \"Fake\",   \"last\" : \"Name\",  \"code\" : 3 }," +
            "\"fname2\" : { \"first\" : \"Fake\",   \"last\" : \"Name\",  \"code\" : 4 }" +
          "}," +
          "\"faculty\" : {" +
            "\"sbinc\" : { \"first\" : \"ShouldntBe\", \"last\" : \"Included\"}" +
          "}" +
        "}"
      );
      root.Print(0);

      Console.WriteLine("\nQuery result");

      // IQuery query = new QueryParser("select *.*.first").ParseQuery();
      Query query = new QueryParser("insert { \"first\" : \"Joe\", \"last\" : \"Schmoe\" } into students.jschmoe").ParseQuery();
      INode result = query.Execute(root);

      if (result != null)
      {
        result.Print(0);
      }

      Console.WriteLine("\nQuery result");
      Query query2 = new QueryParser("delete students.* where students.*.first < \"George\"").ParseQuery();
      result = query2.Execute(root);
      Query query3 = new QueryParser("update students.jschmoe.first value Joel").ParseQuery();
      query3.Execute(root);



      if (result != null)
      {
        result.Print(0);
      }

      Console.WriteLine("\nDatabase Contents");
      root.Print(0);

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
