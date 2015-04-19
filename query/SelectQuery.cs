using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  public class SelectQuery : IQuery
  {
    public SelectQuery(string key, SelectQuery subQuery = null)
    {
      _key = key;
      _subQuery = subQuery;
    }

    public virtual INode Execute(INode data)
    {
      if (_key.Equals("*"))
      {
        IDictionary<string, INode> subnodes = data.GetAllSubNodes();
        if (subnodes == null) return null;

        CollectionNode collection = new CollectionNode();
        if (_subQuery == null) return data;

        foreach (KeyValuePair<string, INode> pair in subnodes)
        {
          collection.SetNode(pair.Key, _subQuery.Execute(pair.Value));
        }
        return collection;
      }
      else if (data.Contains(_key))
      {
        INode subnode = data.GetSubNode(_key);
        if (subnode == null || _subQuery == null) return subnode;
        // TODO: Make a new collection node
        return _subQuery.Execute(subnode);
      }
      else
      {
        return null;
      }
    }

    string _key;
    SelectQuery _subQuery;
    // Conditions
  }
}
