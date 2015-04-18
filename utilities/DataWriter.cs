﻿using System;
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
            string content;
            var filePath = _fileName + ".dat";
            using (var reader = new StreamReader(File.Open(filePath, FileMode.Open)))
            {
                content = reader.ReadToEnd();
            }
            return content;
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
                var pathString = _fileName + ".dat";
                using (var writer =
                    new StreamWriter(File.Open(pathString, FileMode.Create)))
                    {
                        writer.Write(content);
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
