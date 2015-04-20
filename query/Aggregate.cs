using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  public delegate double AggregateFunction(INode collection);

  public class AggregateFunctions
  {
    private static double _max(INode collection)
    {
      IDictionary<string, INode> subnodes = collection.GetAllSubNodes();
      double max = double.NegativeInfinity;
      if (subnodes == null || subnodes.Count == 0) return max;

      foreach (KeyValuePair<string, INode> pair in subnodes)
      {
        object val = pair.Value.GetData();
        if (val == null) continue;
        if (val.GetType() == typeof(double))
        {
          double dval = (double)val;
          if (dval > max)
          {
            max = dval;
          }
        }
        else if (val.GetType() == typeof(int))
        {
          int ival = (int)val;
          if (ival > max)
          {
            max = ival;
          }
        }
      }
      return max;
    }

    private static double _min(INode collection)
    {
      IDictionary<string, INode> subnodes = collection.GetAllSubNodes();
      double min = double.PositiveInfinity;
      if (subnodes == null || subnodes.Count == 0) return min;

      foreach (KeyValuePair<string, INode> pair in subnodes)
      {
        object val = pair.Value.GetData();
        if (val == null) continue;
        if (val.GetType() == typeof(double))
        {
          double dval = (double)val;
          if (dval < min)
          {
            min = dval;
          }
        }
        else if (val.GetType() == typeof(int))
        {
          int ival = (int)val;
          if (ival < min)
          {
            min = ival;
          }
        }
      }
      return min;
    }

    private static double _sum(INode collection)
    {
      IDictionary<string, INode> subnodes = collection.GetAllSubNodes();
      double sum = 0;
      if (subnodes == null || subnodes.Count == 0) return sum;

      foreach (KeyValuePair<string, INode> pair in subnodes)
      {
        object val = pair.Value.GetData();
        if (val == null) continue;

        if (val.GetType() == typeof(double))
        {
          sum += (double)val;
        }
        else if (val.GetType() == typeof(int)) 
        {
          sum += (int)val;
        }
      }
      return sum;
    }

    // This should be int...
    private static double _count(INode collection)
    {
      IDictionary<string, INode> subnodes = collection.GetAllSubNodes();
      if (subnodes == null) return 0;
      return subnodes.Count;
    }

    private static double _average(INode collection)
    {
      double count = _count(collection);
      if (count == 0) return 0;
      double sum = _sum(collection);
      return sum / count;
    }

    public static readonly AggregateFunction Min = _min;
    public static readonly AggregateFunction Max = _max;
    public static readonly AggregateFunction Sum = _sum;
    public static readonly AggregateFunction Count = _count;
    public static readonly AggregateFunction Average = _average;
  }

  public class Aggregate : Query
  {
    public Aggregate(IQuery subquery, AggregateFunction agr)
    {
      _subquery = subquery;
      _agr = agr;
    }

    public override INode Execute(INode data)
    {
      INode sub = _subquery.Execute(data);
      double result;
      if (sub == null)
      {
        result = 0;
      }
      else
      {
        result = _agr(sub);
      }
      return new PrimitiveNode<double>(result);
    }

    private IQuery _subquery;
    private AggregateFunction _agr;
  }
}
