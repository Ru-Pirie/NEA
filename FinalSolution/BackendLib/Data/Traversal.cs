using BackendLib.Datatypes;
using System;
using System.Collections.Generic;

namespace BackendLib.Data
{
    public class Traversal<T>
    {
        private Graph<T> _graph;

        public Traversal(Graph<T> graph)
        {
            _graph = graph;
        }

        public T[] DFS(T start)
        {
            List<T> path = new List<T>();
            Datatypes.Stack<T> stack = new Datatypes.Stack<T>();
            Dictionary<T, bool> visited = new Dictionary<T, bool>();
            foreach (T s in _graph.GetAllNodes()) visited.Add(s, false);

            // Kick Start
            stack.Push(start);

            while (!stack.IsEmpty())
            {
                T node = stack.Pop();
                path.Add(node);
                visited[node] = true;

                List<T> connections = _graph.GetNode(node);

                connections.Reverse();

                foreach (T s in connections)
                {
                    if (visited[s] == false && !stack.Contains(s))
                    {
                        stack.Push(s);
                    }
                }
            }


            return path.ToArray();
        }

        public T[] BFS(T start)
        {
            List<T> path = new List<T>();
            Datatypes.Queue<T> queue = new Datatypes.Queue<T>();
            Dictionary<T, bool> visited = new Dictionary<T, bool>();
            foreach (T s in _graph.GetAllNodes()) visited.Add(s, false);

            // Kick Start
            queue.Enqueue(start);

            while (!queue.IsEmpty())
            {

                T node = queue.Dequeue();
                path.Add(node);
                visited[node] = true;

                List<T> connections = _graph.GetNode(node);

                connections.Reverse();

                foreach (T s in connections)
                {
                    if (visited[s] == false && !queue.Contains(s))
                    {
                        queue.Enqueue(s);
                    }
                }
            }

            return path.ToArray();
        }

        public Dictionary<T, T> AStar(T start, T goal, Func<T, T, int> weightFunction)
        {
            Dictionary<T, int> dist = new Dictionary<T, int>();
            Dictionary<T, T> prev = new Dictionary<T, T>();
            dist.Add(start, 0);

            MinPriorityQueue<T> queue = new MinPriorityQueue<T>();

            T[] nodes = _graph.GetAllNodes();
            foreach (T node in nodes)
            {
                if (_graph.GetNode(node).Count > 0)
                {
                    if (!Equals(start, node)) dist.Add(node, int.MaxValue);
                    queue.Enqueue(node, dist[node]);
                }
            }

            while (queue.Size > 0)
            {
                T minVertex = queue.Dequeue();

                if (Equals(minVertex, goal)) return prev;

                List<T> adjacent = _graph.GetNode(minVertex);

                foreach (var neighbor in adjacent)
                {
                    if (queue.Contains(neighbor))
                    {
                        int alternateWeight = dist[minVertex] + weightFunction(goal, neighbor);
                        if (alternateWeight < dist[neighbor])
                        {
                            dist[neighbor] = alternateWeight;
                            if (prev.ContainsKey(neighbor)) prev[neighbor] = minVertex;
                            else prev.Add(neighbor, minVertex);
                            queue.ChangePriority(neighbor, alternateWeight);
                        }
                    }
                }
            }

            return new Dictionary<T, T>();
        }

        public Dictionary<T, T> Dijkstra(T start, T goal, bool endOnFind)
        {
            Dictionary<T, int> dist = new Dictionary<T, int>();
            Dictionary<T, T> prev = new Dictionary<T, T>();
            dist.Add(start, 0);

            MinPriorityQueue<T> queue = new MinPriorityQueue<T>();

            T[] nodes = _graph.GetAllNodes();
            foreach (T node in nodes)
            {
                if (_graph.GetNode(node).Count > 0)
                {
                    if (!Equals(start, node)) dist.Add(node, int.MaxValue);
                    queue.Enqueue(node, dist[node]);
                }
            }

            while (queue.Size > 0)
            {
                T minVertex = queue.Dequeue();

                if (Equals(minVertex, goal) && endOnFind) return prev;

                List<T> adjacent = _graph.GetNode(minVertex);

                foreach (var neighbor in adjacent)
                {

                    if (queue.Contains(neighbor))
                    {
                        int alternateWeight = dist[minVertex] + 1;
                        if (alternateWeight < dist[neighbor])
                        {
                            dist[neighbor] = alternateWeight;
                            if (prev.ContainsKey(neighbor)) prev[neighbor] = minVertex;
                            else prev.Add(neighbor, minVertex);
                            queue.ChangePriority(neighbor, alternateWeight);
                        }
                    }
                }
            }

            return prev;
        }
    }
}