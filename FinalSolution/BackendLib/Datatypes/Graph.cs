using BackendLib.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace BackendLib.Datatypes
{
    public class Graph<T>
    {
        public Dictionary<T, List<T>> _data = new Dictionary<T, List<T>>();

        public Graph() { }

        public Graph(Dictionary<T, List<T>> graph)
        {
            _data = graph;
        }

        public void AddNode(T key)
        {
            if (_data.ContainsKey(key)) throw new GraphException($"Cannot add {key}, node already exists.");
            _data.Add(key, new List<T>());
        }

        public void RemoveNode(T key)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Cannot remove {key}, node does not exist.");
            _data.Remove(key);
        }

        public void AddConnection(T key, T value)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Cannot add connection {value} to {key} original node does not exist.");
            if (_data[key].Contains(value)) throw new GraphException($"Cannot add connection {value} to {key} connection already exists.");
            _data[key].Add(value);
        }

        public List<T> GetNode(T key)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Node {key} does not exist.");
            return _data[key];
        }

        public T[] GetAllNodes() => _data.Keys.ToArray();

        public bool ContainsNode(T node) => _data.ContainsKey(node);

        public void Clear() => _data.Clear();
    }
}
