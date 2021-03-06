using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spider.Data;
using Graph;
using Walk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Graph.Models;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace Spider
{
    class Program
    {
        public const string root = "https://en.wikipedia.org/wiki/Web_crawler";
        public class SpiderDBContextFactory : IDesignTimeDbContextFactory<SpiderDbContext>
        {
            public readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            public SpiderDbContext CreateDbContext(string[] args)
            {
                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", true, true)
                .Build();

                var optionsBuilder = new DbContextOptionsBuilder<SpiderDbContext>();
                optionsBuilder.UseSqlServer(config.GetConnectionString("SpiderDatabase"));
                optionsBuilder.UseLoggerFactory(_loggerFactory);

                return new SpiderDbContext(optionsBuilder.Options);
            }
        }

        static async IAsyncEnumerable<T> Merge<T>(IEnumerable<IAsyncEnumerable<T>> tasks)
        {
            var enumList = new List<IAsyncEnumerator<T>>();
            foreach (var asyncEnum in tasks)
            {
                enumList.Add(asyncEnum.GetAsyncEnumerator());
            }

            var moveNextTasks = new Dictionary<Task<bool>, IAsyncEnumerator<T>>();
            while (enumList.Count > 0)
            {
                foreach (var enumerator in enumList.ToList())
                {
                    if(!moveNextTasks.ContainsValue(enumerator))
                        moveNextTasks.Add(enumerator.MoveNextAsync().AsTask(),enumerator);
                }
                var completedTask = await Task.WhenAny(moveNextTasks.Keys);
                if(await completedTask)
                {
                    yield return moveNextTasks[completedTask].Current;
                }
                else
                {
                    enumList.Remove(moveNextTasks[completedTask]);
                }
                moveNextTasks.Remove(completedTask);
            }
        }

        static async IAsyncEnumerable<int> Even()
        {
            int i = 0;
            while (true)
            {
                await Task.Delay(1000);
                yield return i;
                i = i + 2;
            }
        }

        static async IAsyncEnumerable<int> Odd()
        {
            int i = 1;
            while (true)
            {
                await Task.Delay(5000);
                yield return i;
                i = i + 2;
            }
        }

        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //await foreach (var item in Merge(new[] { Even().Take(10), Odd() }))
            await foreach (var item in AsyncEnumerableEx.Merge(new[] { Even().Take(10), Odd() }))
            {
                Console.WriteLine(item);
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}

//var bag = new ConcurrentBag<int>() { 1, 2, 3 };
//foreach (var item in bag)
//{
//    Console.WriteLine(item);
//    bag.Add(4);
//}
//return;


////var edges = new (int, int)[] { (1, 2), (1, 3), (2, 4), (2, 5), (3, 6), (3, 7) };
//var edges = new (int, int)[] { (1,-1), (2, -1), (2, -1),(-1, 1), (0,0) };
//IGraph<int> graph = new InMemoryGraph<int>(edges);
////IGraph<Uri> _internet = new Internet();
////await Crawler<Uri>.WalkBfsParallelWithoutRecursionGeneric(_internet, new Uri(root), 2, 8, (_uri) => { });
//await Crawler<int>.WalkBfsWithoutRecursionGeneric(graph, edges[0].Item1, 2, (x) => Console.WriteLine(x));

//return;
//using (var context = new SpiderDBContextFactory().CreateDbContext(null))
//{
//    var pages = context.Pages.AsQueryable();
//    foreach (var page in pages)
//    {
//        Console.WriteLine(page);
//    }
//}


#region dfsExamples
////WalkDfs(new Uri(root), new HashSet<string>());
////WalkBfsWithoutRecursion(new Uri(root));
//var edges = new (int, int)[] { (1, 2), (1, 3), (2, 4), (2, 5), (5, 7), (3, 6), (3, 7) };
////edges = new (int, int)[] { (1, 2), (1, 3), (2, 1), (3, 4) };
////edges = new (int, int)[] { (1, 2), (1, 3), (2, 5), (5, 3), (5, 6), (3, 4) };
//edges = new (int, int)[] { (1, 2), (1, 3), (1, 4), (2, 5), (2, 6), (6, 4) };
//IGraph<int> graph = new InMemoryGraph<int>(edges);

//Crawler.WalkDfsWithoutRecursionGeneric(graph, 1, x => Console.WriteLine(x));
//Console.WriteLine("\nMethod with recursion:");
//Crawler.WalkDfsGeneric<int>(graph, 1, new HashSet<int>(), x => Console.WriteLine(x));

//return;
//IGraph<Uri> net = new Internet();
//foreach (var node in net.Edges(new Uri(root)))
//{
//    Console.WriteLine(node);
//}

//return;

////IGraph<int> graph = new InMemoryGraph(edges);
//foreach (var node in graph.Edges(7))
//{
//    Console.WriteLine(node);
//}
//return;
#endregion