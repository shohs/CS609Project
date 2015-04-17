﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using cs609.data;

namespace cs609.utilities
{
    public class DataWriter
    {
        static void CreateStore(string storeName)
        {
            var pathString = @"C:\temp\" + storeName;
            System.IO.Directory.CreateDirectory(pathString);
        }

        static string RetrieveData(Document document)
        {
            string content;
            var filePath = @"C:\temp\" + document.StoreName + @"\" + document.Id + ".bin";
            using (var binReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                content = binReader.ReadString();
            }

            return content;

        }
        static void CreateDocument(CollectionNode data, string storeName)
        {
            var document = new Document()
            {
                Id = Guid.NewGuid(),
                StoreName = storeName,
                Node = data

            };
            //write document contents to file
            WriteToFile(document);
        }

        static void WriteToFile(Document document)
        {
            try
            {
                var filePath = @"C:\temp\" + document.StoreName + @"\" + document.Id + ".bin";

                using (var binWriter =
                    new BinaryWriter(File.Open(filePath, FileMode.Create)))
                {
                    binWriter.Write(document.Node.Print());
                }
                Console.WriteLine("Data Written!");
                Console.WriteLine();
            }
            catch (IOException ioexp)
            {
                Console.WriteLine("Error: {0}", ioexp.Message);
            }

        }

        //public virtual string ToString(int indent)
        //{
        //    var jSonString = new StringBuilder();
        //    string indentString = new String(' ', indent);
        //    jSonString.Append("{");
        //    foreach (KeyValuePair<string, INode> pair in _collection)
        //    {
        //        jSonString.Append(indentString + "  " + pair.Key + " : ");
        //        pair.Value.Print(indent + 2);
        //    }
        //    Console
        //        .WriteLine(indentString + "}");

        //}

    }
}
