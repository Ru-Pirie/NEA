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
            if (_data.ContainsKey(key)) throw new GraphException($"Failed to add node {key} to the graph, the node already exists.");
            _data.Add(key, new List<T>());
        }

        public void RemoveNode(T key)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Failed to remove node {key} from the graph, the node does not exist.");
            _data.Remove(key);
        }

        public void AddConnection(T key, T value)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Cannot add connection between {value} and {key} the parent node does not exist in the graph.");
            if (_data[key].Contains(value)) throw new GraphException($"Cannot add connection between {value} and {key} the connection already exists.");
            _data[key].Add(value);
        }

        public List<T> GetNode(T key)
        {
            if (!_data.ContainsKey(key)) throw new GraphException($"Failed to get node {key} form graph because it does not exist.");
            return _data[key];
        }

        public T[] GetAllNodes() => _data.Keys.ToArray();

        public bool ContainsNode(T node) => _data.ContainsKey(node);

        public void Clear() => _data.Clear();
    }
}
