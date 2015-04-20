using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using cs609.data;
using cs609.query;
using cs609.utilities;

namespace cs609
{
    public class Interface
    {
        public static void main()
        {
            string command;
            string databaseName = "cs609";
            var loader = new DataLoader(databaseName + ".dat");
            var collection = loader.LoadDataNodes();

            //if time we can implement a choice of databases
            do
            {
                Console.Write(databaseName + " > ");
                command = Console.ReadLine();

                switch (command.ToLower())
                {
                    case "print":
                        collection.Print(5);
                        break;
                    case "get":
                         Console.Write("Which item would you like to retrieve?");
                    var key = Console.ReadLine();
                    var result = collection.GetSubNode(key);
                    result.Print(2);
                        break;
                    case "exit":
                        break;
                    case "help":
                    case "?":
                        Console.WriteLine("You may enter the following Commands:");
                        Console.WriteLine("print - prints a formatted view of all data");
                        Console.WriteLine("get - this is a dummy command for now");
                        Console.WriteLine("exit - this ends the program");
                        Console.WriteLine("help - displays a list of commands");
                        Console.WriteLine("You may also query the database.");
                        break;
                    default:
                        ProcessQuery(command);

                        break;
                }

            } while (!command.Equals("exit"));
        }

        private static void ProcessQuery(string command)
        {
            if()

            Query query = new QueryParser("select *.*.first").ParseQuery();


        }


    }
}
