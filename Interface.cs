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
        private string _command;
        private const string DatabaseName = "cs609";
        private DataLoader _loader;
        private INode _collection;
        public void Start()
        {
            _loader = new DataLoader(DatabaseName + ".dat");
            _collection = _loader.LoadDataNodes();
            Logger.TransactionLimit = 2;

            //if time we can implement a choice of databases
            do
            {
                Console.Write(DatabaseName + " > ");
                _command = Console.ReadLine();

                switch (_command.ToLower())
                {
                    case "print":
                        _collection.Print(5);
                        break;
                    case "get":
                         Console.Write("Which item would you like to retrieve?");
                    var key = Console.ReadLine();
                    var result = _collection.GetSubNode(key);
                    result.Print(2);
                        break;
                    case "checkpoint":
                        Logger.WriteToFile(DatabaseName);
                        break;
                    case "exit":
                        Logger.WriteToFile(DatabaseName);
                        break;
                    case "help":
                    case "?":
                        Console.WriteLine("You may enter the following Commands:");
                        Console.WriteLine("print - prints a formatted view of all data");
                        Console.WriteLine("checkpoint - creates a checkpoint and forces the log to write to disk");
                        Console.WriteLine("exit - this ends the program");
                        Console.WriteLine("help - displays a list of commands");
                        Console.WriteLine("You may also query the database.");
                        break;
                    default:
                        ProcessQuery();
                        break;
                }

            } while (!_command.Equals("exit"));
        }

        private void ProcessQuery()
        {
            try
            {
                var query = new QueryParser(_command).ParseQuery();
                query.Execute(_collection);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
