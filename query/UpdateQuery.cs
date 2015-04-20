using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;

namespace cs609.query
{
    public class UpdateQuery : IQuery
    {
        public UpdateQuery(INode toUpdate, string key, UpdateQuery subQuery = null)
        {
            _toUpdate = toUpdate;
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
                        cNode.UpdateNode(_key, _toUpdate);
                        return _toUpdate;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Can not locate key");
            }
        }

        string _key;
        INode _toUpdate;
        UpdateQuery _subQuery;
    }
}
