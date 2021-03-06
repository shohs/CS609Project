﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using cs609.utilities;

namespace cs609.data
{
  public class CollectionNode : INode
  {
    public CollectionNode()
    {
      _collection = new Dictionary<string, INode>();
    }

    public virtual void InsertNode(string key, INode node)
    {
      if (!_collection.Keys.Contains(key))
      {
        _collection[key] = node;
      }
      else
      {
        throw new ArgumentException("Key is already in collection");
      }
    }

    public virtual void Insert<T>(string key, T value)
    {
      InsertNode(key, new PrimitiveNode<T>(value));
    }

    public virtual void UpdateNode(string key, INode node)
    {
      if (_collection.Keys.Contains(key))
      {
        _collection[key] = node;
      }
      else
      {
        throw new ArgumentException("Key did not exist in collection");
      }
    }

    public virtual void Update<T>(string key, T value)
    {
      UpdateNode(key, new PrimitiveNode<T>(value));
    }

    public virtual void SetNode(string key, INode node)
    {
      _collection[key] = node;
    }

    public virtual void Set<T>(string key, T value)
    {
      SetNode(key, new PrimitiveNode<T>(value));
    }

    public virtual bool Contains(string key)
    {
      return _collection.Keys.Contains(key);
    }

    public virtual INode GetSubNode(string key)
    {
      return _collection[key];
    }

    public virtual void DeleteSubNode(string key)
    {
      _collection.Remove(key);
    }

    public virtual void DeleteAllSubNodes()
    {
      _collection.Clear();
    }

    public virtual IDictionary<string, INode> GetAllSubNodes()
    {
      return _collection;
    }

    public virtual object GetData()
    {
      return null;
    }

    public virtual void Print(int indent)
    {
      string indentString = new String(' ', indent);
      Console.WriteLine("{");
      foreach (KeyValuePair<string, INode> pair in _collection)
      {
        Console.Write(indentString + "  " + pair.Key + " : ");
        pair.Value.Print(indent + 2);
      }
      Console.WriteLine(indentString + "}");
    }

    public virtual string ConvertToJson()
    {
        var jSonString = new StringBuilder();
        jSonString.Append("{");
        foreach (KeyValuePair<string, INode> pair in _collection)
        {
            jSonString.Append("\"");
            jSonString.Append(pair.Key);
            jSonString.Append("\" : ");
            jSonString.Append(pair.Value.ConvertToJson());
            jSonString.Append(",");
        }

        JsonTrimmer.TrimTail(jSonString);
        return jSonString.ToString();
    }
    private IDictionary<string, INode> _collection;
  }
}
