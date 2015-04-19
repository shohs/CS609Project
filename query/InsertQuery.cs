using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  public class InsertQuery : IQuery
  {
    public InsertQuery(INode toInsert, string key, InsertQuery subQuery = null)
    {
      _toInsert = toInsert;
      _key = key;
      _subQuery = subQuery;
    }

    public virtual INode Execute(INode data)
    {
      if (data.Contains(_key))
      {
        if (_subQuery != null)
        {
          return _subQuery.Execute(data.GetSubNode(_key));
        }
        else
        {
          throw new ArgumentException("Insert overwrites existing data");
        }
      }
      else
      {
        if (data.GetType() != typeof(CollectionNode))
        {
          throw new ArgumentException("Attempting to insert into a primitive node");
        }

        CollectionNode cNode = (CollectionNode)data;

        // Create a new node
        if (_subQuery != null)
        {
          INode sub = new CollectionNode();
          cNode.InsertNode(_key, sub);
          return _subQuery.Execute(sub);
        }
        else
        {
          cNode.InsertNode(_key, _toInsert);
          return _toInsert;
        }
      }
    }

    string _key;
    INode _toInsert;
    InsertQuery _subQuery;
  }
}
