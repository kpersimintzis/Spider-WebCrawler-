using Spider.Data;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
//using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using System.Net;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace Spider
{
    public interface IGraph<TNode>
    {
        TNode[] Edges(TNode node);
    }

    public class InMemoryGraph : IGraph<int>
    {
        private readonly (int from, int to)[] edges;
        public InMemoryGraph((int from, int to)[] edges)
        {
            this.edges = edges;
        }
        public int[] Edges(int node)
        {
            var res = new List<int>();
            foreach (var (from,to) in edges)
            {
                if(node == from)
                {
                    res.Add(to);
                }
            }
            return res.ToArray();
        }
    }

    class Program
    {
        
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


        public const string root = "https://en.wikipedia.org/wiki/Web_crawler";
        static void Main(string[] args)
        {
            //WalkDfs(new Uri(root), new HashSet<string>());
            //WalkBfsWithoutRecursion(new Uri(root));
            var edges = new (int, int)[] { (1, 2), (1, 3), (2, 4), (2, 5), (5, 7), (3, 6), (3, 7) };
            IGraph<int> graph = new InMemoryGraph(edges);
            foreach (var node in graph.Edges(7))
            {
                Console.WriteLine(node);
            }
            return;

            using (var context = new SpiderDBContextFactory().CreateDbContext(null))
            {
                var pages = context.Pages.AsQueryable();
                foreach (var page in pages)
                {
                    Console.WriteLine(page);
                }

            }
        }
    }
}