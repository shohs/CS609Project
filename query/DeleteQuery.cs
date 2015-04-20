using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  class DeleteQuery : IQuery
  {
    public DeleteQuery(string key, DeleteQuery subQuery = null)
    {
      _key = key;
      _subQuery = subQuery;
      _filters = new List<IQueryFilter>();
    }

    public void AddFilter(IQueryFilter filter)
    {
      _filters.Add(filter);
    }

    public virtual INode Execute(INode data)
    {
      if (_key.Equals("*") || _filters.Count != 0)
      {
        IDictionary<string, INode> subnodes = data.GetAllSubNodes();
        if (subnodes == null) return null;

        CollectionNode collection = new CollectionNode();
        if (_subQuery == null && _filters.Count == 0)
        {
          foreach (KeyValuePair<string, INode> pair in subnodes)
          {
            collection.SetNode(pair.Key, pair.Value);
          }
          data.DeleteAllSubNodes();
          return collection;
        }

        IList<string> removedKeys = new List<string>();
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
              collection.SetNode(pair.Key, _subQuery.Execute(pair.Value));
            }
            else
            {
              removedKeys.Add(pair.Key);
              collection.SetNode(pair.Key, pair.Value);
            }
          }
        }

        foreach (string key in removedKeys)
        {
          data.DeleteSubNode(key);
        }

        return collection;

      }
      else if (data.Contains(_key))
      {
        INode subnode = data.GetSubNode(_key);
        if (_subQuery == null)
        {
          data.DeleteSubNode(_key);
          return subnode;
        }
        return _subQuery.Execute(subnode);
      }
      return null;
    }

    private string _key;
    private DeleteQuery _subQuery;
    private IList<IQueryFilter> _filters;
  }
}
