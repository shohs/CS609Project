using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
  public interface IQueryFilter
  {
    bool ShouldInclude(INode node);
  }

  public delegate bool ComparatorDelegate(IComparable left, IComparable right);

  public abstract class Comparators
  {
    public static readonly ComparatorDelegate LessThan = (left, right) => left.CompareTo(right) < 0;
    public static readonly ComparatorDelegate LessThanEq = (left, right) => left.CompareTo(right) <= 0;
    public static readonly ComparatorDelegate GreaterThan = (left, right) => left.CompareTo(right) > 0;
    public static readonly ComparatorDelegate GreaterThanEq = (left, right) => left.CompareTo(right) >= 0;
    public static readonly ComparatorDelegate Equal = (left, right) => left.Equals(right);
    public static readonly ComparatorDelegate NotEqual = (left, right) => !left.Equals(right);
  }

  public class ConstantComparisonFilter : IQueryFilter
  {
    public ConstantComparisonFilter(string[] subCollections, IComparable compareVal, ComparatorDelegate func)
    {
      _subCollections = subCollections;
      _compareVal = compareVal;
      _func = func;
    }

    public virtual bool ShouldInclude(INode node)
    {
      INode curNode = node;

      foreach (string key in _subCollections)
      {
        if (curNode.Contains(key))
        {
          curNode = curNode.GetSubNode(key);
        }
        else
        {
          return false;
        }
      }

      IComparable value = (IComparable)curNode.GetData();

      return _func(value, _compareVal);
    }

    private string[] _subCollections;
    private IComparable _compareVal;
    private ComparatorDelegate _func;
  }

  public class FieldComparisonFilter : IQueryFilter
  {
    public virtual bool ShouldInclude(INode node)
    {
      return false;
    }
  }
}
