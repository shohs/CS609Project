using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
    public abstract class Query : IQuery
    {
        public Commands CommandType { get; set; }
        public string Command { get; set; }
        public string Keys { get; set; }
        public string NewValue { get; set; }
        public abstract INode Execute(INode data);
    }
}
