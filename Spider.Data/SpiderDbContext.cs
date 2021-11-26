using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Spider.Data.Entities;
using Microsoft.Extensions.Logging.Console;
namespace Spider.Data
{
    public class SpiderDbContext : DbContext
    {
        public SpiderDbContext(DbContextOptions<SpiderDbContext> options)
            : base(options) { }
            

        public DbSet<Page> Pages { get; set; }
        public DbSet<PageMap> PageMaps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PageMap>(entity =>
            {
                entity.HasOne(e => e.From)
                    .WithMany();

                entity.HasOne(e => e.To)
                    .WithMany();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public class SpiderDBContextFactory : IDesignTimeDbContextFactory<SpiderDbContext>
        {
            public readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            public SpiderDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<SpiderDbContext>();
                optionsBuilder.UseSqlServer(connStr);
                optionsBuilder.UseLoggerFactory(_loggerFactory);

                return new SpiderDbContext(optionsBuilder.Options);
            }

        }

        public const string connStr = "Server=DESKTOP-F8K7M9N;Database=SpiderDb;Trusted_Connection=True;MultipleActiveResultSets=true";
        static void Main(string[] args)
        {
            
        }
    }
}
