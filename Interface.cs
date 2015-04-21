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

        private Database _db;

        public void Start()
        {
            _db = new Database("MordorDb");

            //if time we can implement a choice of databases
            do
            {
                Console.Write(_db.DatabaseName + " > ");
                _command = Console.ReadLine();

                switch (_command.ToLower())
                {
                    case "print":
                    case "1":
                        _db.Print();
                        break;
                    case "checkpoint":
                    case "2":
                        _db.Checkpoint();
                        break;
                    case "rollback":
                    case "3":
                        _db.Rollback();
                        break;
                    case "help":
                    case "?":
                    case "4":
                        Console.WriteLine("You may enter the following Commands:");
                        Console.WriteLine("1) print - prints a formatted view of all data");
                        Console.WriteLine("2) checkpoint - creates a checkpoint and forces the log to write to disk");
                        Console.WriteLine("3) rollback - rolls the data back to its state at the last checkpoint");
                        Console.WriteLine("4) help - displays a list of commands");
                        Console.WriteLine("5) clear - clear the terminal");
                        Console.WriteLine("6) exit - this ends the program");
                        Console.WriteLine("You may also query the database.");
                        break;

                    case "clear":
                    case "5":
                        Console.Clear();
                        break;

                    case "exit":
                    case "6":
                        _command = "exit";
                        // Checkpoint should save the data
                        _db.Checkpoint();
                        break;

                    default:
                        // Queries must end in a semicolon, so parse until one is encountered
                        while (!_command.Contains(';'))
                        {
                          Console.Write(_db.DatabaseName + " | ");
                          _command += Console.ReadLine();
                        }
                        int index = _command.IndexOf(';');
                        _command = _command.Substring(0, index + 1);

                        INode result = ProcessQuery();
                        if (result != null)
                        {
                            result.Print(0);
                        }
                        break;
                }

            } while (!_command.Equals("exit"));
        }

        private INode ProcessQuery()
        {
            try
            {
                var query = new QueryParser(_db, _command).ParseQuery();
                INode result = _db.ExecuteQuery(query);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
