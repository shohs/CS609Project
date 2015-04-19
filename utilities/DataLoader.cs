using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.utilities
{
    public class DataLoader
    {
        private string _fileName;

        public  DataLoader(string fileName)
        {
              _fileName = fileName;
        }
        public INode LoadDataNodes()
        {
            try
            {
                var savedData = DataReader.ParseJSONString(RetrieveRawData());
                return savedData;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }

        }
        public string RetrieveRawData()
        {
            try
            {
                string content;
                using (var reader = new StreamReader(File.Open(_fileName, FileMode.Open)))
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
    }
}
