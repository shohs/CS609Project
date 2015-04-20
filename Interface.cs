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
                    case "1":
                        _collection.Print(5);
                        break;
                    case "checkpoint":
                    case "2":
                        var writer = new DataWriter(DatabaseName + ".dat");
                        writer.WriteToFile(_collection.ConvertToJson());
                        Logger.WriteToFile(DatabaseName);
                        break;
                    case "rollback":
                    case "3":
                        _collection = _loader.LoadDataNodes();
                        break;
                    case "help":
                    case "?":
                    case "4":
                        Console.WriteLine("You may enter the following Commands:");
                        Console.WriteLine("1) print - prints a formatted view of all data");
                        Console.WriteLine("2) checkpoint - creates a checkpoint and forces the log to write to disk");
                        Console.WriteLine("3) rollback - rolls the data back to its state at the last checkpoint");
                        Console.WriteLine("4) help - displays a list of commands");
                        Console.WriteLine("5) exit - this ends the program");
                        Console.WriteLine("You may also query the database.");
                        break;
                    case "exit":
                    case "5":
                        _command = "exit";
                        Logger.WriteToFile(DatabaseName);
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
