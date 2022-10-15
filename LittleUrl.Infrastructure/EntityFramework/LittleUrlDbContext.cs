using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using LittleUrl.Domain;
using System;
using System.IO;

namespace LittleUrl.Infrastructure.EntityFramework
{
    public class LittleUrlDbContext : DbContext
    {
        public LittleUrlDbContext(DbContextOptions<LittleUrlDbContext> options) : base(options) { }

        public DbSet<Domain.LitlUrl> LitlUrls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public class LittleUrlDbContextFactory : IDesignTimeDbContextFactory<LittleUrlDbContext>
        {
            public LittleUrlDbContext CreateDbContext(string[] args)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory() + "/../LittleUrl.Web")
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .Build();

                var optionsBuilder = new DbContextOptionsBuilder<LittleUrlDbContext>();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                return new LittleUrlDbContext(optionsBuilder.Options);
            }
        }
    }
}
