using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Graph;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Walk
{
    public static class Crawler<T>
    {
        #region Generic
        #region Dfs
        public static async Task WalkDfsGeneric(IGraph<T> graph, T root, HashSet<T> visited, Action<T> action)
        {
            if (visited.Contains(root))
                return;
            visited.Add(root);
            action(root);
            foreach (var node in await graph.Edges(root))
            {
                await WalkDfsGeneric(graph, node, visited, action);
            }

        }
        public static async Task WalkDfsWithoutRecursionGeneric(IGraph<T> graph, T root, Action<T> action)
        {
            Stack<T> stack = new Stack<T>();
            HashSet<T> visited = new HashSet<T>();

            stack.Push(root);

            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (visited.Contains(node))
                    continue;
                action(node);
                visited.Add(node);
                foreach (var n in (await graph.Edges(node)).Reverse())
                {
                    if (!visited.Contains(n))
                    {
                        stack.Push(n);
                        //visited.Add(n);
                    }
                }
            }
        }
        public static async Task BetterWalkDfsWithoutRecursionGeneric(IGraph<T> graph, T root, Action<T> action)
        {
            Stack<IEnumerator> stack = new Stack<IEnumerator>();
            HashSet<T> visited = new HashSet<T>();
            action(root);
            visited.Add(root);

            stack.Push((await graph.Edges(root)).GetEnumerator());

            while (stack.Count != 0)
            {
                var enumerator = stack.Peek();
                if (enumerator.MoveNext())
                {
                    var node = (T)enumerator.Current;
                    if (!visited.Contains(node))
                    {
                        action(node);
                        visited.Add(node);
                        stack.Push((await graph.Edges(node)).GetEnumerator());
                    }
                }
                else
                {
                    stack.Pop();
                }
            }
        }
        #endregion

        public static async Task WalkBfsWithoutRecursionGeneric(IGraph<T> graph, T root, int n, Action<T> action)
        {
            Queue<(T, int)> queue = new Queue<(T, int)>();
            HashSet<T> visited = new HashSet<T>();

            queue.Enqueue((root, n));
            visited.Add(root);

            while (queue.Count != 0)
            {
                var (node, _n) = queue.Dequeue();
                if (_n < 0)
                    break;
                action(node);

                foreach (var _node in await graph.Edges(node))
                {
                    if (!visited.Contains(_node))
                    {
                        queue.Enqueue((_node, _n - 1));
                        visited.Add(_node);
                    }
                }
            }
        }

        public static async Task WalkParallelWithoutRecursionGeneric(IGraph<T> graph, T root, int n, int parallelism, Action<T> action)
        {
            ConcurrentQueue<(T, int)> queue = new ConcurrentQueue<(T, int)>();
            ConcurrentDictionary<T, T> visited = new ConcurrentDictionary<T, T>();

            queue.Enqueue((root, n));
            visited.TryAdd(root, root);

            List<Task> tasks = new List<Task>();
            var isProcessing = new bool[parallelism];
            for (int i = 0; i < parallelism; i++)
            {
                var _i = i;
                isProcessing[_i] = true;
                var task = Task.Run(async () =>
                {
                    while (isProcessing.Contains(true))
                    {
                        //Console.WriteLine($"I'm alive. From the {_i} universe");                        
                        if (queue.TryDequeue(out var result))
                        {
                            //Console.WriteLine($"The {_i} universe is working on {result}");

                            var (node, _n) = result;
                            if (_n < 0)
                                continue;
                            action(node);

                            foreach (var _node in await graph.Edges(node))
                            {
                                if (visited.TryAdd(_node, _node))
                                    queue.Enqueue((_node, _n - 1));
                            }
                        }
                        else
                        {
                            //await Task.Delay(1000);
                        }
                        isProcessing[_i] = (queue.Count != 0);
                    }
                    isProcessing[_i] = false;
                    //Console.WriteLine($"I am about to die...{_i}");
                });

                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        public static async Task WalkDynamicParallelWithoutRecursionGeneric(IGraph<T> graph, T root, int n, Action<T> action)
        {
            ConcurrentDictionary<T, T> visited = new ConcurrentDictionary<T, T>();
            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

            async Task Process(T node, int n)
            {
                if (n < 0)
                    return;

                action(node);
                foreach (var _node in await graph.Edges(node))
                {
                    if (visited.TryAdd(_node, _node))
                        tasks.Add(Process(_node, n - 1));
                }
            }

            tasks.Add(Process(root, n));
            visited.TryAdd(root, root);

            await Task.WhenAll(tasks);
        }

        #endregion

        #region NonGeneric
        public static void WalkDfs(Uri uri, HashSet<string> visited)
        {
            if (visited.Contains(uri.AbsoluteUri)) return;

            using (WebClient client = new WebClient())
            {
                visited.Add(uri.AbsoluteUri);
                string htmlCode = "";
                try
                {
                    htmlCode = client.DownloadString(uri);
                }
                catch (WebException) { return; }

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlCode);
                var nodes = htmlDoc.DocumentNode.Descendants("a").Where(x => x.Attributes["href"] != null);

                foreach (var node in nodes)
                {
                    if (Uri.TryCreate(node.Attributes["href"].Value, UriKind.RelativeOrAbsolute, out var _uri))
                    {
                        if (!_uri.IsAbsoluteUri && Uri.TryCreate(uri, _uri, out var _combinedUri))
                        {
                            Console.WriteLine($"name:{node.Attributes["href"].Name}, value: {node.Attributes["href"].Value}" +
                                            $"is absolute? : {_uri.IsAbsoluteUri} after combined : {_combinedUri.IsAbsoluteUri} " +
                                            $"new Uri: {_combinedUri.AbsoluteUri}");
                            WalkDfs(_combinedUri, visited);
                        }
                        else
                        {
                            Console.WriteLine($"name:{node.Attributes["href"].Name}, value: {node.Attributes["href"].Value}" +
                                            $"is absolute? : {_uri.IsAbsoluteUri}");
                            WalkDfs(_uri, visited);
                        }
                    }
                }
            }
        }
        public static void WalkDfsWithoutRecursion(Uri uri)
        {
            Stack<Uri> stackOfUris = new Stack<Uri>();
            stackOfUris.Push(uri);

            HashSet<string> visited = new HashSet<string>();
            visited.Add(uri.AbsoluteUri);

            while (stackOfUris.Count != 0)
            {
                uri = stackOfUris.Pop();
                using (WebClient client = new WebClient())
                {
                    visited.Add(uri.AbsoluteUri);
                    string htmlCode = "";
                    try
                    {
                        htmlCode = client.DownloadString(uri);
                    }
                    catch (WebException) { return; }

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlCode);
                    var nodes = htmlDoc.DocumentNode.Descendants("a").Where(x => x.Attributes["href"] != null);

                    foreach (var node in nodes.Reverse())
                    {
                        if (Uri.TryCreate(node.Attributes["href"].Value, UriKind.RelativeOrAbsolute, out var _uri))
                        {
                            if (!_uri.IsAbsoluteUri && Uri.TryCreate(uri, _uri, out var _combinedUri))
                            {
                                Console.WriteLine($"name:{node.Attributes["href"].Name}, value: {node.Attributes["href"].Value}" +
                                                $"is absolute? : {_uri.IsAbsoluteUri} after combined : {_combinedUri.IsAbsoluteUri} " +
                                                $"new Uri: {_combinedUri.AbsoluteUri}");
                                _uri = _combinedUri;
                            }
                            else
                            {
                                Console.WriteLine($"name:{node.Attributes["href"].Name}, value: {node.Attributes["href"].Value}" +
                                                $"is absolute? : {_uri.IsAbsoluteUri}");
                            }

                            if (!visited.Contains(_uri.AbsoluteUri))
                            {
                                stackOfUris.Push(_uri);
                                visited.Add(_uri.AbsoluteUri);

                            }
                        }
                    }
                }
            }
        }
        public static void WalkBfsWithoutRecursion(Uri uri)
        {
            Queue<Uri> queueOfUris = new Queue<Uri>();
            queueOfUris.Enqueue(uri);

            HashSet<string> visited = new HashSet<string>();
            visited.Add(uri.AbsoluteUri);

            while (queueOfUris.Count != 0)
            {
                uri = queueOfUris.Dequeue();
                using (WebClient client = new WebClient())
                {
                    visited.Add(uri.AbsoluteUri);
                    string htmlCode = "";
                    try
                    {
                        htmlCode = client.DownloadString(uri);
                    }
                    catch (WebException) { return; }

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlCode);
                    var nodes = htmlDoc.DocumentNode.Descendants("a").Where(x => x.Attributes["href"] != null);

                    foreach (var node in nodes)
                    {
                        if (Uri.TryCreate(node.Attributes["href"].Value, UriKind.RelativeOrAbsolute, out var _uri))
                        {
                            if (!_uri.IsAbsoluteUri && Uri.TryCreate(uri, _uri, out var _combinedUri))
                            {
                                Console.WriteLine($"name:{node.Attributes["href"].Name}, value: {node.Attributes["href"].Value}" +
                                                $"is absolute? : {_uri.IsAbsoluteUri} after combined : {_combinedUri.IsAbsoluteUri} " +
                                                $"new Uri: {_combinedUri.AbsoluteUri}");
                                _uri = _combinedUri;
                            }
                            else
                            {
                                Console.WriteLine($"name:{node.Attributes["href"].Name}, value: {node.Attributes["href"].Value}" +
                                                $"is absolute? : {_uri.IsAbsoluteUri}");
                            }

                            if (!visited.Contains(_uri.AbsoluteUri))
                            {
                                queueOfUris.Enqueue(_uri);
                                visited.Add(_uri.AbsoluteUri);

                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
