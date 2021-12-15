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

        public const string root = "https://en.wikipedia.org/wiki/Web_crawler";
        static void Main(string[] args)
        {
            WalkDfs(new Uri(root), new HashSet<string>());

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