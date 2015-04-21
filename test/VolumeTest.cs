using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using cs609.data;
using cs609.query;

namespace cs609.test
{
  class VolumeTest
  {
    public static void RunTest()
    {
      // Setting this to 1750000 makes me run out of memory :(
      // It seems that the json strings are as much of a problem as the data itself
      const int numRecords = 1600000;
      Database db = new Database("volume");
      Stopwatch stopwatch = new Stopwatch();
      
      var builder = new StringBuilder("insert {");
      for (int i = 0; i < numRecords; i++)
      {
        builder.Append("\"record_" + i + "\" : { \"field\" : " + i + "}");
        if (i != numRecords - 1) builder.Append(',');
      }
      builder.Append("} into records;");

      stopwatch.Start();
      string bstring = builder.ToString();
      builder = null;
      var query = new QueryParser(db, bstring).ParseQuery();
      bstring = null;
      db.ExecuteQuery(query);

      GC.Collect();

      long millis = stopwatch.ElapsedMilliseconds;
      stopwatch.Stop();

      Console.WriteLine("Done inserting " + numRecords + " records in " + millis + " milliseconds");

      stopwatch.Reset();

      stopwatch.Start();
      query = new QueryParser(db, "select average records.*.field;").ParseQuery();
      INode results = db.ExecuteQuery(query);
      millis = stopwatch.ElapsedMilliseconds;
      stopwatch.Stop();

      Console.WriteLine("\nResult of average: ");
      results.Print(0);

      Console.WriteLine("Aggregate computed in " + millis + " milliseconds");

      db.Checkpoint();
      Console.WriteLine();
    }
  }
}
