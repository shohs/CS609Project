using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using cs609.data;
using cs609.utilities;

namespace cs609
{
    public class Check
    {
        public static void Test()
        {
            var writer = new DataWriter("cs609");

            var root = new CollectionNode();
            var sub = new CollectionNode();
            sub.Insert("test", "hi");
            sub.Insert("test2", 6);

            var sub2 = new CollectionNode();
            sub2.Insert("test", 7);
            sub2.Insert("test2", 9);

            sub.InsertNode("sub2!!!!!", sub2);

            root.InsertNode("sub", sub);

            writer.CreateDocument(root);
               
        }

        public static INode LoadData()
        {   
            
            try
            {
                var writer = new DataWriter("cs609.dat");
                var savedData = DataReader.ParseJSONString(writer.RetrieveData());
                return savedData;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
           
        }

        public static void BeginProgram()
        {
            string command;
            string databaseName = "CS609";

            do
            {
                var collection =  LoadData();
                Console.Write(databaseName + ">");
                command = Console.ReadLine();
                if (command.Equals("print"))
                {
                   collection.Print(5);
                }
                if (command.Equals("get"))
                {
                    Console.Write("Which item would you like to retrieve?");
                    var key = Console.ReadLine();
                    var result = collection.GetSubNode(key);
                    result.Print(2);
                }

            } while (!command.Equals("exit"));
            
        }
        public static void CheckLog()
        {

            //This info comes from user when they issue a command but i dont know what thats going to look like yet
            var myEvent = new LogItem()
            {
                TransactionType = "Insert",
                Command = "Insert CS609.sub",
                StoreName = "CS609",
                DocumentKey = "sub2",
                NewValue = "greetings",
                Committed = false
            };
            var myEvent1 = new LogItem()
            {
                TransactionType = "Update",
                Command = "Update CS609.sub",
                StoreName = "CS609",
                DocumentKey = "sub2",
                CurrentValue = "greetings",
                NewValue = "goodbye",
                Committed = false
            };
            var myEvent2 = new LogItem()
            {
                TransactionType = "Update",
                Command = "Update CS609.sub",
                StoreName = "CS609",
                DocumentKey = "sub2",
                CurrentValue = "goodbye",
                NewValue = "easy cowboy",
                Committed = false
            };

            
            //also i would imagine the n number of transactions should be configurable but im setting it manually
            Logger.TransactionLimit = 2;

            Logger.LogTransaction(myEvent);
            Logger.LogTransaction(myEvent1);
            Logger.LogTransaction(myEvent2);




        }
    }

     
}
