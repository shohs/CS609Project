using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cs609.utilities;
using cs609.query;

namespace cs609.data
{
  class Database
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
      // This should be done in batches
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

    public string DatabaseName { get; private set; }
    private INode _collection;
    private DataLoader _loader;
    private IDictionary<string, Index> indices;
  }
}
