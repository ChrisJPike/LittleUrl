
using System.Linq.Expressions;

namespace LittleUrl.Domain.Repositories
{
    public interface ILitlUrlRepository
    {
        Task<LitlUrl?> GetAsync(Expression<Func<LitlUrl, bool>> predicate);
        LitlUrl AddLitlUrl(string code, string longUrl);
        Task<bool> GetExistsAsync(Expression<Func<LitlUrl, bool>> predicate);

        Task<int> Save();

        // Out of scope: Get method which gets which allows for filtering, pagination etc for reporting.
        // Out of scope: Delete method to potentially delete expired LitlUrls.
    }
}
