using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  public class SelectQuery : Query 
  {
    public SelectQuery(string key, SelectQuery subQuery = null)
    {
      _key = key;
      _subQuery = subQuery;
      _filters = new List<IQueryFilter>();
    }

    public void AddFilter(IQueryFilter filter)
    {
      _filters.Add(filter);
    }

    public override INode Execute(INode data)
    {
      if (_key.Equals("*") || _filters.Count != 0)
      {
        IDictionary<string, INode> subnodes = data.GetAllSubNodes();
        if (subnodes == null) return null;

        if (_subQuery == null && _filters.Count == 0)
        {
          return data;
        }

        CollectionNode collection = new CollectionNode();
        foreach (KeyValuePair<string, INode> pair in subnodes)
        {
          bool shouldInclude = true;
          foreach (IQueryFilter filter in _filters)
          {
            if (!filter.ShouldInclude(pair.Value))
            {
              shouldInclude = false;
              break;
            }
          }

          if (shouldInclude)
          {
            if (_subQuery != null)
            {
              INode subnode = _subQuery.Execute(pair.Value);
              if (subnode != null && (subnode.GetAllSubNodes() == null || subnode.GetAllSubNodes().Count > 0))
              { 
                collection.SetNode(pair.Key, _subQuery.Execute(pair.Value));
              }
            }
            else
            {
              collection.SetNode(pair.Key, pair.Value);
            }
          }
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

    private string _key;
    private SelectQuery _subQuery;
    private IList<IQueryFilter> _filters;
  }
}
