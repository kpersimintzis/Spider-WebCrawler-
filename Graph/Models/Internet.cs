using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class Internet : IGraph<Uri>
    {
        public async Task<Uri[]> Edges(Uri uri)
        {
            var uris = new List<Uri>();
            using (WebClient client = new WebClient())
            {
                string htmlCode = "";
                try
                {
                    htmlCode = await client.DownloadStringTaskAsync(uri);
                }
                catch (WebException) { return new Uri[] { }; }

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlCode);
                var nodes = htmlDoc.DocumentNode.Descendants("a").Where(x => x.Attributes["href"] != null);

                foreach (var node in nodes)
                {
                    if (Uri.TryCreate(node.Attributes["href"].Value, UriKind.RelativeOrAbsolute, out var _uri))
                    {
                        if (!_uri.IsAbsoluteUri && Uri.TryCreate(uri, _uri, out var _combinedUri))
                        {
                            _uri = _combinedUri;
                        }
                        uris.Add(_uri);
                    }
                }
                return uris.ToArray();
            }
        }
    }
}