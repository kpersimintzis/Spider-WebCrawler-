using System;
using Xunit;
using FsCheck.Xunit;
using Spider;
using Graph;
using Graph.Models;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Walk;
using System.Diagnostics;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Test
{
    public class UnitTest
    {
        //[Property(MaxTest = 5000)]
        //public bool TestCommutativity(int a, int b)
        //{
        //    Debug.WriteLine($"{a} {b}");
        //    return (a + b == b + a);
        //}

        //[Property(MaxTest = 10000)]
        //public bool TestDfsNew(Node node)
        //{
        //    if (node == null) return true;

        //    IGraph<Node> graph = new NodeGraph();
        //    List<Node> orderOfRecursiveNodes = new List<Node>();
        //    Crawler.WalkDfsGeneric<Node>(graph, node, new HashSet<Node>(), x => orderOfRecursiveNodes.Add(x));

        //    List<Node> orderOfNonRecursiveNodes =new List<Node>();
        //    Crawler.WalkDfsWithoutRecursionGeneric(graph, node, x => orderOfNonRecursiveNodes.Add(x));
        //    return orderOfNonRecursiveNodes.SequenceEqual(orderOfRecursiveNodes);
        //}

        //[Property(MaxTest = 10000)]
        //public bool TestDfs(Tuple<int, int>[] edges)
        //{
        //    if (edges.Length == 0) return true;
        //    var newEdges = edges.Select(x => (from: x.Item1, to: x.Item2)).ToArray();
        //    IGraph<int> graph = new InMemoryGraph<int>(newEdges);
        //    List<int> orderOfRecursiveNodes = new List<int>();
        //    Crawler.WalkDfsGeneric<int>(graph, newEdges[0].from, new HashSet<int>(), x => orderOfRecursiveNodes.Add(x));

        //    List<int> orderOfNonRecursiveNodes = new List<int>();
        //    Crawler.WalkDfsWithoutRecursionGeneric(graph, newEdges[0].from, x => orderOfNonRecursiveNodes.Add(x));
        //    return orderOfNonRecursiveNodes.SequenceEqual(orderOfRecursiveNodes);
        //}

        //[Property(MaxTest = 10000)]
        //public bool TestDfs(Tuple<int, int>[] edges)
        //{
        //    if (edges.Length == 0) return true;
        //    var newEdges = edges.Select(x => (from: x.Item1, to: x.Item2)).ToArray();
        //    IGraph<int> graph = new InMemoryGraph<int>(newEdges);
        //    List<int> orderOfRecursiveNodes = new List<int>();
        //    Crawler<int>.WalkDfsGeneric(graph, newEdges[0].from, new HashSet<int>(), x => orderOfRecursiveNodes.Add(x)).Wait();

        //    List<int> orderOfNonRecursiveNodes = new List<int>();
        //    Crawler<int>.BetterWalkDfsWithoutRecursionGeneric(graph, newEdges[0].from, x => orderOfNonRecursiveNodes.Add(x)).Wait();
        //    return orderOfNonRecursiveNodes.SequenceEqual(orderOfRecursiveNodes);
        //}

        //[Property(MaxTest = 10000)]
        //public bool TestParallelQueues(uint n)
        //{
        //    var queue = new ConcurrentQueue<int>();
        //    List<Task> tasks = new List<Task>();
        //    for (int i = 0; i < n; i++)
        //    {
        //        var _i = i;
        //        var enqueuetask = Task.Run(() => queue.Enqueue(_i));
        //        var dequeuetask = Task.Run(() => 
        //        { 
        //            while(!queue.TryDequeue(out int result)) {} 
        //        });
        //        //task.Wait();
        //        tasks.Add(dequeuetask);
        //        tasks.Add(enqueuetask);
        //    }
        //    Task.WhenAll(tasks).Wait();
        //    return queue.Count == 0;
        //}

        //[Property(MaxTest = 10000)]
        //public bool TestParallelHashsets(uint n)
        //{
        //    var dictionary = new ConcurrentDictionary<int, int>();
        //    List<Task<bool>> tasks = new List<Task<bool>>();
        //    for (int i = 0; i < n; i++)
        //    {
        //        int _i = i;
        //        var dictionaryFunctions = Task.Run(() =>
        //        {
        //            while (!dictionary.TryAdd(_i, _i)){}
        //            return dictionary.ContainsKey(_i);

        //        });
        //        tasks.Add(dictionaryFunctions);
        //    }

        //    Task.WhenAll(tasks).Wait();
        //    foreach (var task in tasks)
        //    {
        //        if (!task.Result)
        //            return false;
        //    }
        //    return true;

        //}

        //[Property(MaxTest = 100000)]
        //public bool TestCrawlerSeqVsParallel(Tuple<int, int>[] edges)
        //{
        //    if (edges.Length == 0) return true;
        //    var newEdges = edges.Select(x => (from: x.Item1, to: x.Item2)).ToArray();
        //    IGraph<int> graph = new InMemoryGraph<int>(newEdges);
        //    List<int> resultOfSeq = new List<int>();
        //    Crawler<int>.WalkBfsWithoutRecursionGeneric(graph, newEdges[0].from, int.MaxValue, (x) => resultOfSeq.Add(x)).Wait();

        //    List<int> resultOfParallel = new List<int>();
        //    Crawler<int>.WalkParallelWithoutRecursionGeneric(graph, newEdges[0].from, int.MaxValue, 2, (x) =>
        //    {
        //        lock (resultOfParallel)
        //        {
        //            resultOfParallel.Add(x);
        //        }
        //    }).Wait();

        //    //if(resultOfParallel.Count != resultOfSeq.Count)
        //    //{

        //    //}
        //    //return true;
        //    return resultOfParallel.Count == resultOfSeq.Count;
        //    //return resultOfParallel.OrderBy(p => p).SequenceEqual(resultOfSeq.OrderBy(s => s));
        //}

        //[Property(MaxTest = 100000)]
        //public bool TestCrawlerDynamicParallelVsSeq(Tuple<int, int>[] edges)
        //{
        //    if (edges.Length == 0) return true;
        //    var newEdges = edges.Select(x => (from: x.Item1, to: x.Item2)).ToArray();
        //    IGraph<int> graph = new InMemoryGraph<int>(newEdges);
        //    List<int> resultOfSeq = new List<int>();
        //    Crawler<int>.WalkBfsWithoutRecursionGeneric(graph, newEdges[0].from, int.MaxValue, (x) => resultOfSeq.Add(x)).Wait();

        //    List<int> resultOfParallel = new List<int>();
        //    Crawler<int>.WalkDynamicParallelWithoutRecursionGeneric(graph, newEdges[0].from, int.MaxValue, (x) =>
        //    {
        //        lock (resultOfParallel)
        //        {
        //            resultOfParallel.Add(x);
        //        }
        //    }).Wait();

        //    var parallel = resultOfParallel.ToList();
        //    var seq = resultOfSeq.ToList();

        //    //if (parallel.Count != seq.Count)
        //    //{

        //    //}
        //    //return true;
        //    return resultOfParallel.Count == resultOfSeq.Count;
        //    //return resultOfParallel.OrderBy(p => p).SequenceEqual(resultOfSeq.OrderBy(s => s));
        //}

        [Property(MaxTest = 100000)]
        public bool TestCrawlerDfsVsDfsAsyncEnum(Tuple<int, int>[] edges)
        {
            if (edges.Length == 0) return true;
            var newEdges = edges.Select(x => (from: x.Item1, to: x.Item2)).ToArray();
            IGraph<int> graph = new InMemoryGraph<int>(newEdges);
            List<int> resultOfDfs = new List<int>();
            Crawler<int>.WalkDfsGeneric(graph, newEdges[0].from, new HashSet<int>(),(x) => resultOfDfs.Add(x)).Wait();

            
            var result = Crawler<int>.WalkDfsGenericAsyncEnum(graph, newEdges[0].from, new HashSet<int>()).ToListAsync().Result;
           
            var seq = resultOfDfs.ToList();

            //if (parallel.Count != seq.Count)
            //{

            //}
            //return true;
            //return result.Count == resultOfDfs.Count;
            return result.SequenceEqual(resultOfDfs);
        }


    }
}
