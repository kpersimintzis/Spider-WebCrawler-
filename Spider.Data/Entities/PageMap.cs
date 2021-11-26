using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider.Data.Entities
{
    public class PageMap
    {
        public long Id { get; set; }
        public Page From { get; set; }
        public Page To { get; set; }
    }
}
