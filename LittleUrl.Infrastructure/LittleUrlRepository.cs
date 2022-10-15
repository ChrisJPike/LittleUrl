using LittleUrl.Domain;
using LittleUrl.Domain.Repositories;
using LittleUrl.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LittleUrl.Infrastructure
{
    public class LitlUrlRepository : ILitlUrlRepository
    {
        private readonly LittleUrlDbContext context;

        public LitlUrlRepository(LittleUrlDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Checks to see if a LitlUrl already exists based on the longUrl.
        /// If it doesn't, a new LitlUrl is created and added to the context.
        /// </summary>
        /// <param name="longUrl">The longUrl to check</param>
        /// <returns>The existing, or new LitlUrl</returns>
        public LitlUrl AddLitlUrl(string code, string longUrl)
        {
            LitlUrl litlUrl = new LitlUrl(code, longUrl);
            context.Add(new LitlUrl(code, longUrl));
            return litlUrl;
        }

        public async Task<int> Save()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<LitlUrl?> GetAsync(Expression<Func<LitlUrl, bool>> predicate) => await context.LitlUrls.SingleOrDefaultAsync(predicate);

        public async Task<bool> GetExistsAsync(Expression<Func<LitlUrl, bool>> predicate) => await context.LitlUrls.AnyAsync(predicate);

        
    }
}
