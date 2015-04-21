using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cs609.data;
using cs609.utilities;
using cs609.query;
using cs609.test;

namespace cs609
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 0 && args[0].Equals("runtests"))
      {
        VolumeTest.RunTest();
      }
      else
      {
        var userInterface = new Interface();
        userInterface.Start();
      }
    }

  }
}
