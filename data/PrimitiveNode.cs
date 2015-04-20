using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs609.data
{

  public class PrimitiveNode<T> : INode
  {
    public PrimitiveNode() { }

    public PrimitiveNode(T value)
    {
      Value = value;
    }

    public T Value { set; get; }

    public virtual bool Contains(string key)
    {
      return false;
    }

    public virtual INode GetSubNode(string key)
    {
      return null;
    }

    public virtual IDictionary<string, INode> GetAllSubNodes()
    {
      return null;
    }

    public virtual object GetData()
    {
      return Value;
    }

    public virtual void Print(int indent)
    {
      Console.WriteLine(Value);
    }

    public virtual string ConvertToJson()
    {
        return "\"" + Value.ToString() + "\"";
    }
  }
}
