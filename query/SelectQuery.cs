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

    public bool CanUseIndex(Database db, string prefix)
    {
      if (_filters.Count > 2 || _filters.Count == 0)
      {
        return false;
      }
      else if (_filters.Count == 2)
      {
        if (_filters[0].Fields.Length != _filters[1].Fields.Length 
          || _filters[0].GetType() != typeof(ConstantComparisonFilter)
          || _filters[1].GetType() != typeof(ConstantComparisonFilter))
        {
          return false;
        }

        string fieldString = prefix;
        for (int i = 0; i < _filters[0].Fields.Length; i++)
        {
          fieldString += _filters[0].Fields[i] + ".";
          if (!_filters[0].Fields[i].Equals(_filters[1].Fields[i]))
          {
            return false;
          }
        }
        fieldString = fieldString.Substring(0, fieldString.Length - 1);
        return db.HasIndex(fieldString);
      }
      else
      {
        if ( _filters[0].GetType() != typeof(ConstantComparisonFilter))
        {
          return false;
        }

        string fieldString = prefix;
        for (int i = 0; i < _filters[0].Fields.Length; i++)
        {
          fieldString += _filters[0].Fields[i] + ".";
        }

        fieldString = fieldString.Substring(0, fieldString.Length - 1);

        return db.HasIndex(fieldString);
      }
    }

    public Query ToIndexQuery(Database db, string prefix)
    {
      if (_filters.Count == 0) return null;

      string fieldString = prefix;
      for (int i = 0; i < _filters[0].Fields.Length; i++)
      {
        fieldString += _filters[0].Fields[i] + ".";
      }

      fieldString = fieldString.Substring(0, fieldString.Length - 1);
      if (!db.HasIndex(fieldString))
      {
        return null;
      }

      if (_filters.Count == 2)
      {
        var filter0 = (ConstantComparisonFilter)_filters[0];
        var filter1 = (ConstantComparisonFilter)_filters[1];

        if (filter0.CompareValue.CompareTo(filter1.CompareValue) > 0)
        {
          var temp = filter1;
          filter1 = filter0;
          filter0 = temp;
        }

        bool lte = false, gte = false;
        if (filter0.Comparator.Equals(Comparators.LessThan))
        {
          lte = false;
        }
        else if (filter0.Comparator.Equals(Comparators.LessThanEq))
        {
          lte = true;
        }
        else
        {
          return null;
        }

        if (filter1.Comparator.Equals(Comparators.GreaterThan))
        {
          gte = false;
        }
        else if (filter1.Comparator.Equals(Comparators.GreaterThanEq))
        {
          gte = true;
        }
        else
        {
          return null;
        }

        Index index = db.GetIndex(fieldString);
        IndexQuery iquery = new IndexQuery(index, filter0.CompareValue, filter1.CompareValue, lte, gte, _subQuery);
        return iquery;
      }
      else if (_filters.Count == 1)
      {
        var filter = (ConstantComparisonFilter)_filters[0];
        Index index = db.GetIndex(fieldString);
        IComparable minValue, maxValue;
        if (filter.CompareValue.GetType() == typeof(double))
        {
          minValue = double.MinValue;
          maxValue = double.MaxValue;
        }
        else if (filter.CompareValue.GetType() == typeof(int))
        {
          minValue = int.MinValue;
          maxValue = int.MaxValue;
        }
        else
        {
          // This is probably wrong.
          minValue = "" + ((char)0);
          maxValue = "" + ((char)0xffff);
        }
        
          
        if (filter.Comparator.Equals(Comparators.LessThan))
        {
          return new IndexQuery(index, minValue, filter.CompareValue, true, false, _subQuery);
        }
        else if (filter.Comparator.Equals(Comparators.LessThanEq))
        {
          return new IndexQuery(index, minValue, filter.CompareValue, true, true, _subQuery);
        }
        else if (filter.Comparator.Equals(Comparators.GreaterThan))
        {
          return new IndexQuery(index, filter.CompareValue, maxValue, false, true, _subQuery);
        }
        else if (filter.Comparator.Equals(Comparators.GreaterThanEq))
        {
          return new IndexQuery(index, filter.CompareValue, maxValue, true, true, _subQuery);
        }
        else if (filter.Comparator.Equals(Comparators.Equal))
        {
          return new IndexQuery(index, filter.CompareValue, filter.CompareValue, true, true, _subQuery);
        }
        else
        {
          return null;
        }
      }
      else
      {
        return null;
      }
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
    public override IQuery SubQuery { get { return _subQuery; } }
    private SelectQuery _subQuery;
    private IList<IQueryFilter> _filters;
  }
}
