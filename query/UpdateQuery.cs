using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs609.data;
using cs609.utilities;

namespace cs609.query
{
    public class UpdateQuery : Query
    {
        public UpdateQuery(INode toUpdate, string key, UpdateQuery subQuery = null)
        {
            _toUpdate = toUpdate;
            _key = key;
            _subQuery = subQuery;
        }

        public override INode Execute(INode data)
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
                        var item = new LogItem()
                        {
                            TransactionType = CommandType,
                            Command = Command,
                            StoreName = "cs609",
                            DocumentKey = _key,
                            NewValue = _toUpdate.ConvertToJson(),
                            CurrentValue = cNode.GetSubNode(_key).ConvertToJson(),
                            Committed = false,
                            DateCreated =  DateTime.Now
                        };
                        Logger.LogTransaction(item);
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
