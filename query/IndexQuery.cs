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
    public IndexQuery(Index index, IComparable min, IComparable max, bool minEqual, bool maxEqual, IQuery subQuery = null)
    {
      _index = index;
      _min = min;
      _max = max;
      _minEqual = minEqual;
      _maxEqual = maxEqual;
      _subQuery = subQuery;
    }

    public override INode Execute(INode data)
    {
      Console.WriteLine("Using an index:");
      INode collection;
      if (_min.Equals(_max))
      {
        collection = _index.GetSingleNode(_min);
      }
      else
      {
        collection = _index.GetNodeRange(_min, _max, _minEqual, _maxEqual);
      }

      if (collection == null || _subQuery == null) return collection;
      return _subQuery.Execute(collection);
    }

    private IQuery _subQuery;
    private IComparable _min, _max;
    private bool _minEqual, _maxEqual;
    private Index _index;
  }
}
