﻿using System;
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
    public class UserInterface
    {
        private string _command;

        private Database _db;

        public void Start()
        {
            DisplayStartUpText();

            _db = new Database("MordorDB");

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
                        DisplayHelpText();
                        break;

                    case "clear":
                    case "5":
                        Console.Clear();
                        break;

                    case "exit":
                    case "6":
                        _command = Exit();
                        break;
                    case "query help":
                    case "7":
                        DisplayQueryHelpText();
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

        private string Exit()
        {
            Console.WriteLine("Do you want to save all changes before exiting?");
            var save = Console.ReadLine();
            do
            {
                switch (save)
                {
                    case "yes":
                        _db.Checkpoint();
                        return "exit";
                    case "no":
                        return "exit";
                    case "cancel":
                        return "";
                    default:
                        Console.WriteLine("Enter yes, no or cancel.");
                        save = Console.ReadLine();
                        break;
                }
            } while (save != "yes" && save != "no");
            return "";
        }
        private void DisplayStartUpText()
        {
            //Fancy Colors for extra points ;)
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(String.Format("MordorDB{0}", Environment.NewLine));
            Console.ResetColor();

            DisplayHelpText();
        }

        private void DisplayHelpText()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Available Commands:");
            Console.WriteLine("1) print - prints a formatted view of all data");
            Console.WriteLine("2) checkpoint - creates a checkpoint and forces the log to write to disk");
            Console.WriteLine("3) rollback - rolls the data back to its state at the last checkpoint");
            Console.WriteLine("4) help - displays a list of commands");
            Console.WriteLine("5) clear - clear the terminal");
            Console.WriteLine("6) exit - this ends the program");
            Console.WriteLine("7) query help - for help with the queries or you may enter your query here");
            Console.ResetColor();
        }

        private void DisplayQueryHelpText()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Available Queries:"); 
            Console.WriteLine("select - select employee.first");
            Console.WriteLine("insert - insert { “first” : “Joe”, “last” : “Schmoe”, “department“ :“Sales“ } into employees.jschmoe");
            Console.WriteLine("update - update employees.jschmoe.first value Joel");
            Console.WriteLine("delete - delete employees.* where employees.*.first < “Joel”");
            Console.ResetColor();
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
