using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using cs609.data;


namespace cs609.utilities
{
    public class DataWriter
    {
        private string _storeName;

        public DataWriter(string storeName)
        {
            _storeName = storeName;
        }

   
        //static string RetrieveData(Document document)
        //{
        //    string content;
        //    var filePath = @"C:\temp\" + document.StoreName + ".dat";
        //    using (var binReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        //    {
        //        content = binReader.ReadString();
        //    }

        //    return content;

        //}
        public void CreateDocument(CollectionNode data)
        {
            var document = new Document()
            {
                StoreName = _storeName,
                Node = data
            };
            //write document contents to file
            WriteToFile(document);
        }

        public void WriteToFile(Document document)
        {
            try
            {
                var pathString = _storeName + ".dat";
                using (var binWriter =
                    new StreamWriter(File.Open(pathString, FileMode.Create)))
                    {
                        binWriter.Write(document.Node.ConvertToJson());
                    }
                Console.WriteLine("Data Written!");
                Console.WriteLine();
            }
            catch (IOException ioexp)
            {
                Console.WriteLine("Error: {0}", ioexp.Message);
            }
        }
    }
}
