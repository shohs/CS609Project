using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs609.data
{
  public class Index
  {
    public static IDictionary<string, Index> LoadIndexes(INode root)
    {
      var list = new List<string>(); // get this from some stored state
      var dictionary = new Dictionary<string, Index>();

      foreach (var index in list)
      {
        dictionary[index] = new Index(root, index);
      }

      return dictionary;
    }

    public Index(INode root, string field)
    {
      
    }

    public INode GetSingleNode(IComparable value)
    {
      CollectionNode collection = new CollectionNode();
      return collection;
    }

    public INode GetNodeRange(IComparable min, IComparable max)
    {
      CollectionNode collection = new CollectionNode();
      return collection;
    }

    private BPlusTree _tree;

    public class BPlusTree
    {
      public const int BranchingFactor = 50;
      private BPlusNode _root = new BPlusLeaf();
      private bool _rootIsLeaf = true;

      public void Insert(IComparable value, string key, INode node)
      {
        BPlusNode curNode = _root;

        while (!curNode.IsLeaf)
        {
          BPlusInternalNode iNode = (BPlusInternalNode)curNode;
          for (int i = 0; i < curNode.Size - 1; i++)
          {
            if (value.CompareTo(iNode.Values[i]) < 0)
            {
              curNode = iNode.Children[i];
              break;
            }
          }
          curNode = iNode.Children[iNode.Size - 1];
        }

        if (curNode.Size == BranchingFactor)
        {
          // split the node
        }
        else
        {
          BPlusLeaf leaf = (BPlusLeaf)curNode;
          if (leaf.Size == 0)
          {
            leaf.Values[0] = value;
            leaf.Children[0] = new List<INode>();
            leaf.Children[0].Add(node);
            leaf.ChildrenKeys[0] = new List<string>();
            leaf.ChildrenKeys[0].Add(key);
            leaf.Size++;
          }
          else
          {
            for (int i = 0; i < leaf.Size; i++)
            {
              if (leaf.Values[i].CompareTo(value) == 0)
              {
                leaf.Children[i].Add(node);
                return;
              } 
              else if (leaf.Values[i].CompareTo(value) < 0)
              {
                for (int j = leaf.Size - 1; j >= i + 1; j--)
                {
                  leaf.Values[j + 1] = leaf.Values[j];
                  leaf.Children[j + 1] = leaf.Children[j];
                }
                leaf.Values[i + 1] = value;
                leaf.Children[i + 1] = new List<INode>();
                leaf.Children[i + 1].Add(node);
                leaf.ChildrenKeys[i + 1] = new List<string>();
                leaf.ChildrenKeys[i + 1].Add(key);
                return;
              }
            }
            leaf.Values[leaf.Size] = value;
            leaf.Children[leaf.Size] = new List<INode>();
            leaf.Children[leaf.Size].Add(node);
            leaf.ChildrenKeys[leaf.Size] = new List<string>();
            leaf.ChildrenKeys[leaf.Size].Add(key);
            leaf.Size++;
          }
        }
      }

      public void Delete(IComparable value, INode node)
      {

      }

      public INode GetSingleNode(IComparable value)
      {
        BPlusNode curNode = _root;

        while (!curNode.IsLeaf)
        {
          BPlusInternalNode iNode = (BPlusInternalNode)curNode;
          for (int i = 0; i < curNode.Size - 1; i++)
          {
            if (value.CompareTo(iNode.Values[i]) < 0)
            {
              curNode = iNode.Children[i];
              break;
            }
          }
          curNode = iNode.Children[iNode.Size - 1];
        }

        BPlusLeaf leaf = (BPlusLeaf)curNode;
        CollectionNode collection = new CollectionNode();
        for (int i = 0; i < leaf.Size; i++)
        {
          if (leaf.Values[i].CompareTo(value) == 0)
          {
            for (int j = 0; j < leaf.Children.Length; j++)
            {
              collection.SetNode(leaf.ChildrenKeys[i][j], leaf.Children[i][j]);
            }
            return collection;
          }
          else if (leaf.Values[i].CompareTo(value) < 0)
          {
            return collection;
          }
        }
        return collection;
      }

      public INode GetNodeRange(IComparable min, IComparable max)
      {
        return null;
      }

      private interface BPlusNode {
        bool IsLeaf { get; }
        int Size { get; set; }
        IComparable[] Values { get; }
      }

      private class BPlusInternalNode : BPlusNode
      {
        public int Size { get; set; }
        public bool IsLeaf { get { return false; } }
        public BPlusNode Parent = null;
        public BPlusNode[] Children = new BPlusNode[BranchingFactor];
        public IComparable[] Values { get { return _values; } }
        private IComparable[] _values = new IComparable[BranchingFactor - 1];
      }

      private class BPlusLeaf : BPlusNode
      {
        public BPlusNode Next;
        public int Size { get; set; }
        public bool IsLeaf { get { return true; } }
        public BPlusNode Parent = null;
        public IList<INode>[] Children = new List<INode>[BranchingFactor - 1];
        public IList<string>[] ChildrenKeys = new List<string>[BranchingFactor - 1];
        public IComparable[] Values { get { return _values; } }
        public IComparable[] _values = new IComparable[BranchingFactor - 1];
      }
    }
  }
}
