using BackendLib.Datatypes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

        public T[] AStar(T start, T goal)
        {
            return new T[1];
        }

        public T[] Dijkstra(T start, T goal, Func<T, int> weightFunction)
        {
            Dictionary<T, int> dist = new Dictionary<T, int>();
            Dictionary<T, T> prev = new Dictionary<T, T>();
            dist.Add(start, 0);

            PriorityQueue<T> queue = new PriorityQueue<T>();

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
                List<T> adjacent = _graph.GetNode(minVertex);

                foreach (var neighbor in adjacent)
                {
                    if (Equals(neighbor, new Structures.Cord { X = 510, Y = 264 })) Console.ReadLine();

                    // Issue seems to be that the pixel already appears before its a neighbor so its deleted so idk 
                    // how to fix this, its a bit of a future me issue

                    if (queue.Contains(neighbor))
                    {
                        int alternateWeight = dist[minVertex] + weightFunction(neighbor);
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

            List<T> sequence = new List<T>();
            T u = goal;
            while (u != null)
            {
                sequence.Insert(0, u);
                u = prev[u];
            }

            return sequence.ToArray();

            // Some form of dijkstra
            //List<T> path = new List<T>();
            //bool found = false;
            //while (pq.Size > 0 && !found)
            //{
            //    T minVertex = pq.Dequeue();
            //    Console.Title = $"{pq.Size} {minVertex}";

            //    List<T> adjacent = _graph.GetNode(minVertex);
            //    for (int i = 0; i < adjacent.Count; i++)
            //    {
            //        if (distances[adjacent[i]] > distances[minVertex] && visited[adjacent[i]] == false)
            //        {
            //            path.Add(adjacent[i]);
            //            visited[adjacent[i]] = true;
            //            distances[adjacent[i]] = distances[minVertex] + weightFunction(adjacent[i]);
            //            pq.Enqueue(adjacent[i], distances[adjacent[i]]);

            //            if (Equals(adjacent[i], goal)) found = true;
            //        }
            //    }
            //}

            //foreach (T key in distances.Keys)
            //{
            //    if (distances[key] != int.MaxValue) Console.WriteLine($"Distance from source {key.ToString()} ({distances[key]})");
            //}
        }
    }
}