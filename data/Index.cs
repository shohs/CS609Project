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
      _tree = new BPlusTree();
    }

    public INode GetSingleNode(IComparable value)
    {
      return _tree.GetSingleNode(value);
    }

    public INode GetNodeRange(IComparable min, IComparable max, bool minEqual, bool maxEqual)
    {
      return _tree.GetNodeRange(min, max, minEqual, maxEqual);
    }

    private BPlusTree _tree;

    public class BPlusTree
    {
      public const int BranchingFactor = 50;
      private BPlusNode _root = new BPlusLeaf();

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

        BPlusLeaf leaf = (BPlusLeaf)curNode;
        if (leaf.Size == 0)
        {
          leaf.Values[0] = value;
          leaf.Children[0] = new List<INode>();
          leaf.Children[0].Add(node);
          leaf.ChildrenKeys[0] = new List<string>();
          leaf.ChildrenKeys[0].Add(key);
          leaf.Size++;
          return;
        }

        for (int i = 0; i < leaf.Size; i++)
        {
          if (leaf.Values[i].CompareTo(value) == 0)
          {
            leaf.Children[i].Add(node);
            return;
          }
        }
        
        if (curNode.Size == BranchingFactor)
        {
          // split the node
          BPlusLeaf newLeaf = new BPlusLeaf();
          int i;
          for (i = 0; i < curNode.Size - 1; i++)
          {
            if (value.CompareTo(curNode.Values[i]) < 0)
            {
              break;
            }
          }

          int halfway = BranchingFactor / 2;
          if (i < halfway)
          {
            for (int j = 0; j < halfway; j++)
            {
              newLeaf.Children[j] = leaf.Children[halfway + j - 1];
              leaf.Children[halfway + j - 1] = null;
              newLeaf.ChildrenKeys[j] = leaf.ChildrenKeys[halfway + j - 1];
              leaf.ChildrenKeys[halfway + j - 1] = null;
              newLeaf.Values[j] = curNode.Values[halfway + j - 1];
              leaf.Values[halfway + j - 1] = null;
            }

            for (int j = halfway - 1; j >= i; j--)
            {
              leaf.Children[j + 1] = leaf.Children[j];
              leaf.Values[j + 1] = leaf.Values[j];
            }
            leaf.Children[i] = new List<INode>();
            leaf.Children[i].Add(node);
            leaf.ChildrenKeys[i] = new List<string>();
            leaf.ChildrenKeys[i].Add(key);

            leaf.Size = halfway;
            newLeaf.Size = BranchingFactor - halfway + 1;
          }
          else
          {
            for (int j = halfway; j < BranchingFactor; j++)
            {
              newLeaf.Children[j - halfway] = leaf.Children[j];
              leaf.Children[j] = null;
              newLeaf.ChildrenKeys[j - halfway] = leaf.ChildrenKeys[j];
              leaf.ChildrenKeys[j] = null;
              newLeaf.Values[j - halfway] = curNode.Values[j];
              leaf.Values[j] = null;
            }

            for (int j = 0; j < BranchingFactor - halfway; i++)
            {
              if (newLeaf.Values[j].CompareTo(value) < 0)
              {
                for (int k = leaf.Size - 1; k >= i + 1; k--)
                {
                  newLeaf.Values[k + 1] = newLeaf.Values[k];
                  newLeaf.Children[k + 1] = newLeaf.Children[k];
                }
                newLeaf.Values[j + 1] = value;
                newLeaf.Children[j + 1] = new List<INode>();
                newLeaf.Children[j + 1].Add(node);
                newLeaf.ChildrenKeys[j + 1] = new List<string>();
                newLeaf.ChildrenKeys[j + 1].Add(key);
                return;
              }
            }
            newLeaf.Values[BranchingFactor - halfway] = value;
            newLeaf.Children[BranchingFactor - halfway] = new List<INode>();
            newLeaf.Children[BranchingFactor - halfway].Add(node);
            newLeaf.ChildrenKeys[BranchingFactor - halfway] = new List<string>();
            newLeaf.ChildrenKeys[BranchingFactor - halfway].Add(key);
            newLeaf.Size++;

          }

          if (curNode.Parent == null)
          {
            BPlusInternalNode parent = new BPlusInternalNode();
            parent.Size = 1;
            parent.Values[0] = newLeaf.Values[0];
            parent.Children[0] = leaf;
            parent.Children[1] = newLeaf;
            leaf.Parent = parent;
            newLeaf.Parent = parent;
          }
          else
          {

          }
        }
        else
        {
          for (int i = 0; i < leaf.Size; i++)
          {
            if (leaf.Values[i].CompareTo(value) < 0)
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

      public INode GetNodeRange(IComparable min, IComparable max, bool minEqual, bool maxEqual)
      {
        BPlusNode curNode = _root;

        while (!curNode.IsLeaf)
        {
          BPlusInternalNode iNode = (BPlusInternalNode)curNode;
          for (int i = 0; i < curNode.Size - 1; i++)
          {
            if (min.CompareTo(iNode.Values[i]) < 0)
            {
              curNode = iNode.Children[i];
              break;
            }
          }
          curNode = iNode.Children[iNode.Size - 1];
        }

        BPlusLeaf leaf = (BPlusLeaf)curNode;
        CollectionNode collection = new CollectionNode();
        while (leaf != null)
        {
          for (int i = 0; i < leaf.Size; i++)
          {
            if ((leaf.Values[i].CompareTo(min) > 0 || leaf.Values[i].CompareTo(min) == 0 && minEqual)
              && (leaf.Values[i].CompareTo(max) < 0 || leaf.Values[i].CompareTo(max) == 0 && maxEqual))
            {
              for (int j = 0; j < leaf.Children.Length; j++)
              {
                collection.SetNode(leaf.ChildrenKeys[i][j], leaf.Children[i][j]);
              }
            }
            else if (leaf.Values[i].CompareTo(max) > 0 || leaf.Values[i].CompareTo(max) == 0 && !maxEqual)
            {
              return collection;
            }
          }

          leaf = leaf.Next;
        }
        return collection;
      }

      private interface BPlusNode 
      {
        bool IsLeaf { get; }
        int Size { get; set; }
        BPlusNode Parent { get; set; }
        IComparable[] Values { get; }
      }

      private class BPlusInternalNode : BPlusNode
      {
        public int Size { get; set; }
        public bool IsLeaf { get { return false; } }
        public BPlusNode Parent { get; set; }
        public BPlusNode[] Children = new BPlusNode[BranchingFactor];
        public IComparable[] Values { get { return _values; } }
        private IComparable[] _values = new IComparable[BranchingFactor - 1];
      }

      private class BPlusLeaf : BPlusNode
      {
        public BPlusLeaf Next = null;
        public int Size { get; set; }
        public bool IsLeaf { get { return true; } }
        public BPlusNode Parent { get; set; }
        public IList<INode>[] Children = new List<INode>[BranchingFactor - 1];
        public IList<string>[] ChildrenKeys = new List<string>[BranchingFactor - 1];
        public IComparable[] Values { get { return _values; } }
        public IComparable[] _values = new IComparable[BranchingFactor - 1];
      }
    }
  }
}
