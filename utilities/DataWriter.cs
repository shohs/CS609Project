using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using cs609.data;


namespace cs609.utilities
{
    public class DataWriter
    {
        private string _fileName;

        public DataWriter(string fileName)
        {
            _fileName = fileName;
        }
        
        public void CreateDocument(CollectionNode data)
        {
            var document = new Document()
            {
                StoreName = _fileName,
                Node = data
            };
            WriteToFile(document.Node.ConvertToJson());
        }

        public void WriteToFile(string content)
        {
            try
            {
                using (var writer = new StreamWriter(File.Open(_fileName, FileMode.Create)))
                {
                    writer.Write(content);
                }
            }
            catch (IOException ioexp)
            {
                Console.WriteLine("Error: {0}", ioexp.Message);
            }
        }
      
}
}
