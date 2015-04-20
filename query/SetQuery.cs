using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
    public class SetQuery : IQuery
    {
        public SetQuery(INode toSet, string key, SetQuery subQuery = null)
        {
            _toSet = toSet;
            _key = key;
            _subQuery = subQuery;
        }

        public virtual INode Execute(INode data)
        {
            if (data.Contains(_key))
            {
                if (_subQuery != null)
                {
                    return _subQuery.Execute(data.GetSubNode(_key));
                }
                else
                {
                    CollectionNode cNode = (CollectionNode)data;

                    // Create a new node
                    if (_subQuery != null)
                    {
                        INode sub = new CollectionNode();
                        cNode.UpdateNode(_key, sub);
                        return _subQuery.Execute(sub);
                    }
                    else
                    {
                        cNode.UpdateNode(_key, _toSet);
                        return _toSet;
                    }
                }
            }
            else
            {
                if (data.GetType() != typeof(CollectionNode))
                {
                    throw new ArgumentException("Attempting to insert into a primitive node");
                }

                CollectionNode cNode = (CollectionNode)data;

                // Create a new node
                if (_subQuery != null)
                {
                    INode sub = new CollectionNode();
                    cNode.InsertNode(_key, sub);
                    return _subQuery.Execute(sub);
                }
                else
                {
                    cNode.InsertNode(_key, _toSet);
                    return _toSet;
                }
            }
        }

        string _key;
        INode _toSet;
        SetQuery _subQuery;
    }
}
