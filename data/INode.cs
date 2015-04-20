using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs609.data
{
  public interface INode
  {
    bool Contains(string key);
    INode GetSubNode(string key);
    void DeleteSubNode(string key);
    void DeleteAllSubNodes();
    IDictionary<string, INode> GetAllSubNodes();
    object GetData();
    void Print(int indent);
    string ConvertToJson();
  }
}
