using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cs609.data;

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

      CollectionNode root = new CollectionNode();
      CollectionNode sub = new CollectionNode();
      sub.Insert("test", 4);
      sub.Insert("test2", 6);

      CollectionNode sub2 = new CollectionNode();
      sub2.Insert("test", 7);
      sub2.Insert("test2", 9);

      sub.InsertNode("sub2!!!!!", sub2);

      root.InsertNode("sub", sub);

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
