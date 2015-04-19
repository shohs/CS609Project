using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace cs609.utilities
{
    public class Logger
    {
       public static int TransactionCount { get; set; }
       public static int TransactionLimit { get; set; }
       public static List<LogItem> Transactions { get; set; } 
       public static bool LogTransaction(LogItem dbEvent)
        {
            try
            { 
                Transactions.Add(dbEvent);
                return true;
            }
            catch (Exception)
            {
               return false;
            }
        }

        public static bool WriteToFile(string storeName)
        {
            //create a logitem
            var writer = new DataWriter(storeName + ".log");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            foreach (var transaction in Transactions)
            {
                try
                {
                    transaction.Committed = true;
                    writer.WriteToFile(serializer.Serialize(transaction));

                }
                catch (Exception e)
                {
                    transaction.Committed = false;
                }
                
            }

            Transactions.Clear();
            TransactionCount = 0;
            return true;
        }
    }

    public class LogItem
    {
        public string TransactionType { get; set; }
        public string Command { get; set; }
        public string StoreName { get; set; }
        public string DocumentKey { get; set; }
        public string CurrentValue { get; set; }
        public string NewValue { get; set; }
        public bool Committed { get; set; }
    }
}
