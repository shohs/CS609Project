using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;
using cs609.utilities;

namespace cs609
{
    public class Check
    {
        public void Test()
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

        public void LoadData()
        {
            var writer = new DataWriter("cs609");
            var savedData = DataReader.ParseJSONString(writer.RetrieveData());
            try
            {
                savedData.Print(5);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
           
        }
    }

     
}
