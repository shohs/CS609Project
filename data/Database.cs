using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cs609.utilities;
using cs609.query;

namespace cs609.data
{
  public class Database
  {
    public Database(string name)
    {
      DatabaseName = name;
      _loader = new DataLoader(DatabaseName + ".dat");
      _collection = _loader.LoadDataNodes();
      if (_collection == null)
      {
        _collection = new CollectionNode();
      }
      _indices = new Dictionary<string, Index>();
      Logger.TransactionLimit = 2;
    }

    public INode ExecuteQuery(IQuery query)
    {
      return (query != null) ? query.Execute(_collection) : null;
    }

    public void Print()
    {
      _collection.Print(0);
    }

    public void Checkpoint()
    {
      // This could potentially benefit from being done in batches
      var writer = new DataWriter(DatabaseName + ".dat");
      writer.WriteToFile(_collection.ConvertToJson());
      Logger.WriteToFile(DatabaseName);
    }

    public void Rollback()
    {
      _collection = _loader.LoadDataNodes();
      if (_collection == null)
      {
        _collection = new CollectionNode();
      }
    }

    public void CreateIndex(string field)
    {
      _indices[field] = new Index(_collection, field);
    }


    public Index getIndex(string field)
    {
      if (_indices.Keys.Contains(field))
      {
        return _indices[field];
      }
      return null;
    }

    public string DatabaseName { get; private set; }
    private INode _collection;
    private DataLoader _loader;
    private IDictionary<string, Index> _indices;
  }
}
