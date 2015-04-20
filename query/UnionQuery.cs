using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  public class UnionQuery : IQuery
  {
    public UnionQuery(IQuery left, IQuery right)
    {
      _left = left;
      _right = right;
    }

    public virtual INode Execute(INode data)
    {
      INode leftResult = _left.Execute(data);
      INode rightResult = _right.Execute(data);
      // Union the two datasets - there's probably a better way to do this
      CollectionNode union = new CollectionNode();
      IDictionary<string, INode> subnodes = leftResult.GetAllSubNodes();
      if (subnodes != null && subnodes.Count > 0)
      {
        foreach (KeyValuePair<string, INode> pair in subnodes)
        {
          union.SetNode(pair.Key, pair.Value);
        }
      }

      subnodes = leftResult.GetAllSubNodes();
      if (subnodes != null && subnodes.Count > 0)
      {
        foreach (KeyValuePair<string, INode> pair in subnodes)
        {
          // This should overwrite duplicates
          union.SetNode(pair.Key, pair.Value);
        }
      }

      return union;
    }

    private IQuery _left;
    private IQuery _right;
  }
}
