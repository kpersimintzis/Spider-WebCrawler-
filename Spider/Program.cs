using Spider.Data;
using System.Configuration;
using System.Linq;
using System;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new SpiderDbContext.SpiderDBContextFactory().CreateDbContext(null))
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