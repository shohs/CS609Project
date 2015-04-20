using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Util;
using cs609.query;
using System.Runtime.Serialization.Json;

namespace cs609.utilities
{
    public class Logger
    {
        public static int TransactionCount = 0;
       public static int TransactionLimit { get; set; }
       public static List<LogItem> Transactions = new List<LogItem>();
       public static void LogTransaction(LogItem logItem)
        {
            try
            { 
                Transactions.Add(logItem);
                TransactionCount += 1;
                if (TransactionCount > TransactionLimit)
                {
                    WriteToFile(logItem.StoreName);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not Log Event:" + e.Message);
            }
        }

        public static bool WriteToFile(string storeName)
        {
            string fileName = storeName + ".log";
            LoadTransactions(fileName);
            var writer = new DataWriter(fileName);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var logString = new StringBuilder();
            logString.Append("[");
            foreach (var transaction in Transactions)
            {
                try
                {
                    transaction.Committed = true;
                    logString.Append(serializer.Serialize(transaction));
                    logString.Append(",");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    transaction.Committed = false;
                }
            }
            var json = JsonTrimmer.TrimTail(logString.ToString());
            json = json.Trim('}');
            json = json + "}]";
            writer.WriteToFile(json);

            Transactions.Clear();
            TransactionCount = 0;
            return true;
        }

        private static void LoadTransactions(string fileName)
        {
            try
            {
                var reader = new DataLoader(fileName);
                var log = reader.RetrieveRawData();
                var serializer = new DataContractJsonSerializer(typeof(List<LogItem>));
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(log));
                var logitems = (List<LogItem>)serializer.ReadObject(stream);
                logitems.Reverse();
                foreach (var item in logitems)
                {
                    Transactions.Insert(0, item);
                } 

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }

    public class LogItem
    {
        public Commands TransactionType { get; set; }
        //public string Command { get; set; }
        public string StoreName { get; set; }
        public string DocumentKey { get; set; }
        public string CurrentValue { get; set; }
        public string NewValue { get; set; }
        public bool Committed { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
