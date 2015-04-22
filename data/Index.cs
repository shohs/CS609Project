using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.query;

namespace cs609.data
{
  public class Index
  {
    public static IDictionary<string, Index> LoadIndexes(Database db, INode root)
    {
      var list = new List<string>(); // get this from some stored state
      var dictionary = new Dictionary<string, Index>();

      foreach (var index in list)
      {
        dictionary[index] = new Index(db, root, index);
      }

      return dictionary;
    }

    public Index(Database db, INode root, string field)
    {
      _tree = new BPlusTree();

      string[] fields = field.Split('.');
      string superfield = "";
      int i;
      for (i = fields.Length - 1; i >= 0; i--)
      {
        if (fields[i].Equals("*"))
        {
          break;
        }
      }
      if (i < 0) throw new ArgumentException("Indexes must contain a wildcard");

      for (int j = 0; j <= i; j++)
      {
        superfield += fields[j] + ".";
      }
      superfield = superfield.Substring(0, superfield.Length - 1);

      IQuery query = new QueryParser("select " + superfield + ";").ParseQuery(db);
      INode result = query.Execute(root);
      // result.Print(0);

      IDictionary<string, INode> subnodes = result.GetAllSubNodes();
      if (subnodes != null)
      {
        foreach (KeyValuePair<string, INode> pair in subnodes)
        {
          INode node = pair.Value;
          string key = pair.Key;

          INode curNode = node;
          for (int j = i + 1; j < fields.Length; j++)
          {
            curNode = curNode.GetSubNode(fields[j]);
          }

          _tree.Insert(key, (IComparable)curNode.GetData(), node);
        }
      }
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

      public void Insert(string key, IComparable value, INode node)
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
        int index = leaf.Values.BinarySearch(value);
        if (index < 0)
        {
          index = ~index;
          leaf.Values.Insert(index, value);
          leaf.Children.Insert(index, new List<INode>());
          leaf.ChildrenKeys.Insert(index, new List<string>());
        }
        else
        {
          leaf.Children[index] = new List<INode>();
          leaf.ChildrenKeys[index] = new List<string>();
        }
        leaf.Children[index].Add(node);
        leaf.ChildrenKeys[index].Add(key);

        if (leaf.Children.Count >= BranchingFactor)
        {
          List<IComparable> leftVals = (List<IComparable>)leaf.Values.Take(leaf.Values.Count / 2);
          List<IComparable> rightVals = (List<IComparable>)leaf.Values.Skip(leaf.Values.Count / 2);

          List<List<INode>> leftList = (List<List<INode>>)leaf.Children.Take(leaf.Children.Count / 2);
          List<List<INode>> rightList = (List<List<INode>>)leaf.Children.Skip(leaf.Children.Count / 2);

          List<List<string>> leftListKeys = (List<List<string>>)leaf.ChildrenKeys.Take(leaf.Children.Count / 2);
          List<List<string>> rightListKeys = (List<List<string>>)leaf.ChildrenKeys.Skip(leaf.Children.Count / 2);

          BPlusLeaf newLeaf = new BPlusLeaf();
          leaf.Values = leftVals;
          leaf.Children = leftList;
          leaf.ChildrenKeys = leftListKeys;

          newLeaf.Values = rightVals;
          newLeaf.Children = rightList;
          newLeaf.ChildrenKeys = rightListKeys;

          newLeaf.Next = leaf.Next;
          leaf.Next = newLeaf;

          if (leaf.Parent == null)
          {
            BPlusInternalNode parent = new BPlusInternalNode();
            parent.Values[0] = newLeaf.Values[0];
            parent.Children[0] = leaf;
            parent.Children[1] = newLeaf;
            leaf.Parent = parent;
            newLeaf.Parent = parent;
            _root = parent;
          }
          else
          {
            int parentIndex = leaf.Parent.Values.BinarySearch(newLeaf.Values[0]);
            if (parentIndex < 0)
            {
              parentIndex = ~parentIndex;
            }
            BPlusInternalNode inode = (BPlusInternalNode)leaf.Parent;
            inode.Values.Insert(parentIndex, value);
            inode.Children.Insert(parentIndex + 1, newLeaf);
            newLeaf.Parent = inode;

            // Recursively split until the tree is not overloaded
            while (inode.Size >= BranchingFactor)
            {
              List<BPlusNode> leftNodes = (List<BPlusNode>)inode.Children.Take(inode.Children.Count / 2);
              List<BPlusNode> rightNodes = (List<BPlusNode>)inode.Children.Skip(inode.Children.Count / 2);

              leftVals = (List<IComparable>)inode.Values.Take(inode.Values.Count / 2);
              rightVals = (List<IComparable>)inode.Values.Skip(inode.Values.Count / 2);

              BPlusInternalNode sibling = new BPlusInternalNode();
              sibling.Values = rightVals;
              sibling.Children = rightNodes;

              inode.Values = leftVals;
              inode.Children = leftNodes;

              if (inode.Parent == null)
              {
                BPlusInternalNode parent = new BPlusInternalNode();
                parent.Values[0] = sibling.Values[0];
                parent.Children[0] = inode;
                parent.Children[1] = sibling;
                inode.Parent = parent;
                sibling.Parent = parent;
                inode = parent;
                _root = parent;
              }
              else
              {
                parentIndex = inode.Parent.Values.BinarySearch(newLeaf.Values[0]);
                if (parentIndex < 0)
                {
                  parentIndex = ~parentIndex;
                }
                
                inode.Parent.Values.Insert(parentIndex, value);
                ((BPlusInternalNode)inode.Parent).Children.Insert(parentIndex + 1, newLeaf);
                sibling.Parent = inode.Parent;
                inode = (BPlusInternalNode)inode.Parent;
              }

            }
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
          int i;
          BPlusInternalNode inode = (BPlusInternalNode)curNode;
          for (i = 0; i < inode.Values.Count; i++)
          {
            if (value.CompareTo(inode.Values[i]) >= 0)
            {
              curNode = inode.Children[i];
              break;
            }
          }

          if (i == inode.Values.Count)
          {
            curNode = inode.Children[i];
          }
        }

        CollectionNode collection = new CollectionNode();
        BPlusLeaf leaf = (BPlusLeaf)curNode;
        for (int i = 0; i < leaf.Values.Count; i++)
        {
          if (value.CompareTo(leaf.Values[i]) == 0)
          {
            for (int j = 0; j < leaf.Children[i].Count; j++)
            {
              collection.SetNode(leaf.ChildrenKeys[i][j], leaf.Children[i][j]);
            }
            return collection;
          }
          else if (value.CompareTo(leaf.Values[i]) < 0)
          {
            // We've already passed the portion where it would be equal, 
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
              for (int j = 0; j < leaf.Children[i].Count; j++)
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
        int Size { get; }
        BPlusNode Parent { get; set; }
        List<IComparable> Values { get; set; }
      }

      private class BPlusInternalNode : BPlusNode
      {
        public int Size { get { return Children.Count; } }
        public bool IsLeaf { get { return false; } }
        public BPlusNode Parent { get; set; }

        public List<BPlusNode> Children = new List<BPlusNode>(BranchingFactor);
        public List<IComparable> Values { get { return _values; } set { _values = value; } }

        private List<IComparable> _values = new List<IComparable>(BranchingFactor - 1);
      }

      private class BPlusLeaf : BPlusNode
      {
        public BPlusLeaf Next = null;
        public int Size { get { return Children.Count; } }
        public bool IsLeaf { get { return true; } }
        public BPlusNode Parent { get; set; }

        public List<List<INode>> Children = new List<List<INode>>(BranchingFactor - 1);
        public List<List<string>> ChildrenKeys = new List<List<string>>(BranchingFactor - 1);
        public List<IComparable> Values { get { return _values; } set { _values = value; } }

        private List<IComparable> _values = new List<IComparable>(BranchingFactor - 1);
      }
    }
  }
}
