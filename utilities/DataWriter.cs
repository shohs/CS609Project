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


        public string RetrieveData()
        {
            try
            {
                string content;
                var filePath = _fileName;

                using (var reader = new StreamReader(File.Open(filePath, FileMode.Open)))
                {
                    content = reader.ReadToEnd();
                }
                return content;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return String.Empty;
            }

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

                var writer = GetWriter();
                using (writer)
                {
                    writer.Write(content);
                }
            }
            catch (IOException ioexp)
            {
                Console.WriteLine("Error: {0}", ioexp.Message);
            }
        }
        public StreamWriter GetWriter()
        {
            StreamWriter writer;
            var type = _fileName.Substring(_fileName.LastIndexOf('.') + 1);
            switch (type)
            {
                case "log":
                    writer = new StreamWriter(File.Open(_fileName, FileMode.Append));
                    break;
                case "dat":
                    writer = new StreamWriter(File.Open(_fileName, FileMode.Open));
                    break;
                default:
                    writer = new StreamWriter(File.Open(_fileName, FileMode.Open));
                    break;
            }
            return writer;
        }
}
}
