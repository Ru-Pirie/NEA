using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendLib.Exceptions;

namespace BackendLib.Datatypes
{
    public class Graph<TKey, TValue>
    {
        public Dictionary<TKey, List<TValue>> _data = new Dictionary<TKey, List<TValue>>();

        public Graph(Dictionary<TKey, List<TValue>> graph)
        {
            _data = graph;
        }

        public void AddNode(TKey key)
        {
            if (_data.ContainsKey(key)) throw new GraphException($"Cannot add {key}, node already exists.");
            _data.Add(key, new List<TValue>());
        }

        public void RemoveNode(TKey key)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Cannot remove {key}, node does not exist.");
            _data.Remove(key);
        }

        public void AddConnection(TKey key, TValue value)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Cannot add connection {value} to {key} original node does not exist.");
            if (_data[key].Contains(value)) throw new GraphException($"Cannot add connection {value} to {key} connection already exists.");
            _data[key].Add(value);
        }

        public List<TValue> GetNode(TKey key)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Node {key} does not exist.");
            return _data[key];
        }

        public TValue[] GetAllNodes() => _data.Keys.ToArray();

        public void Clear() => _data.Clear();
    }
}
