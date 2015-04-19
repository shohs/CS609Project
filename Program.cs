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
      //string command;

      INode root = DataReader.ParseJSONString("{ \"test\" : \"value\", \"test2\" : \"value 2\", \"subcollection\" : { \"subkey1\" : \"subval1\" } }");
      root.Print(0);

      Console.WriteLine("Query result");
      Query query = QueryParser.ParseQuery("select subcollection.subkey1");
      query.Execute(root).Print(0);

      //do
      //{
      //  Console.Write("cs609 > ");
      //  command = Console.ReadLine();
        
      //  if (command.StartsWith("insert"))
      //  {

      //  }

      //} while (!command.Equals("exit"));

      Console.WriteLine("~>~>~>~>~>~>~>~>~>~>~>~>~>~>~>>~>~");
      var checker = new Check();
      checker.Test();
      checker.LoadData();
      Console.ReadLine();
    }
  }
}
