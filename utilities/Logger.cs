using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Util;

namespace cs609.utilities
{
    public class Logger
    {
        public static int TransactionCount = 0;
       public static int TransactionLimit { get; set; }
       public static List<LogItem> Transactions = new List<LogItem>();
       public static void LogTransaction(LogItem dbEvent)
        {
            try
            { 
                Transactions.Add(dbEvent);
                TransactionCount += 1;
                if (TransactionCount > TransactionLimit)
                {
                    WriteToFile(dbEvent.StoreName);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not Log Event:" + e.Message);
            }
        }

        public static bool WriteToFile(string storeName)
        {
            //create a logitem
            var writer = new DataWriter(storeName + ".log");
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var logString = new StringBuilder();
            logString.Append("{");
            foreach (var transaction in Transactions)
            {
                try
                {
                    transaction.Committed = true;
                    logString.Append("\"");
                    logString.Append(Guid.NewGuid());
                    logString.Append("\":");
                    logString.Append(serializer.Serialize(transaction));
                    logString.Append(",");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    transaction.Committed = false;
                }
            }
            writer.WriteToFile(JsonTrimmer.TrimTail(logString.ToString()));

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
