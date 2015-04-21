using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  class IndexQuery : Query
  {
    public IndexQuery(string field, IComparable min, IComparable max, IQuery subQuery = null)
    {
      _index = null; // get this somehow
      _min = min;
      _max = max;
      _subQuery = subQuery;
    }

    public override INode Execute(INode data)
    {
      INode collection;
      if (_min.Equals(_max))
      {
        collection = _index.GetSingleNode(_min);
      }
      else
      {
        collection = _index.GetNodeRange(_min, _max);
      }

      if (collection == null || _subQuery == null) return collection;
      return _subQuery.Execute(collection);
    }

    private IQuery _subQuery;
    private IComparable _min, _max;
    private Index _index;
  }
}
