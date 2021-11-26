using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider.Data.Entities
{
    public class Page
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public DateTime CrawledDate { get; set; }
    }
}
