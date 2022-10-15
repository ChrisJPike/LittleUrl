using LittleUrl.Domain;
using LittleUrl.Domain.Repositories;

namespace LittleUrl.Website.Services
{
    public interface ILitlUrlService
    {
        public Task<string?> AddLitlUrl(string longUrl);
        public Task<string?> GetWithCode(string code);
    }

    public class LitlUrlService : ILitlUrlService
    {
        readonly ILitlUrlRepository litlUrlRepository;
        readonly ILogger<LitlUrlService> logger;

        private const int minCharacters = 4;
        private const int maxCharacters = 9;

        public LitlUrlService( ILitlUrlRepository litlUrlRepository, ILogger<LitlUrlService> logger)
        {
            this.litlUrlRepository = litlUrlRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Attempts to return a code based on a longUrl.
        /// Checks if the longUrl already exists in the database first.
        /// If not, adds a new LitlUrl to the database with a newly generated code.
        /// </summary>
        /// <param name="longUrl">The longUrl which needs to be assigned to a code.</param>
        /// <returns>The code to use as part of li.tl, or null if there is an exception.</returns>
        public async Task<string?> AddLitlUrl(string longUrl)
        {
            try
            {
                // check to see if there is already a LitlUrl with this longUrl
                LitlUrl? litlUrl = await litlUrlRepository.GetAsync(u => u.LongUrl == longUrl);
                if (litlUrl != null)
                    return litlUrl.Code;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occured.");
                return null;
            }

            string? code = await GenerateCode();
            // error already logged in above method.
            if (code == null)
                return null;

            try
            {
                LitlUrl LitlUrl = litlUrlRepository.AddLitlUrl(code, longUrl);
                await litlUrlRepository.Save();
                return LitlUrl.Code;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occured.");
                return null;
            }
        }

        /// <summary>
        /// Generates a new code.
        /// If it exists in the context, loops through until a unique code is created.
        /// </summary>
        /// <returns>A unique code.</returns>
        private async Task<string?> GenerateCode()
        {
            try
            {
                string code = CodeFromGuid(minCharacters, maxCharacters);

                while (await litlUrlRepository.GetExistsAsync(u => u.Code == code))
                    code = CodeFromGuid(minCharacters, maxCharacters);

                return code;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occured.");
                return null;
            }
        }

        /// <summary>
        /// Creates a Guid and pulls out the first X characters to form a string.
        /// </summary>
        /// <param name="min">The minumum number of characters to use.</param>
        /// <param name="max">The maximum number of chracters to use.</param>
        /// <returns>A string, between {min} and {max} in length.</returns>
        private string CodeFromGuid(int min, int max) => Guid.NewGuid().ToString().Replace("-", "").Substring(0, new Random(DateTime.Now.Millisecond).Next(min, max));

        /// <summary>
        /// Attempts to retrieve the longUrl for a given code.
        /// </summary>
        /// <param name="code">The code to check against in the database.</param>
        /// <returns>The longUrl if a LitlUrl exists in the database, otherwise an empty string if nothing is found, or null if an exception occurs.</returns>
        public async Task<string?> GetWithCode(string code)
        {
            try
            {
                LitlUrl? LitlUrl = await litlUrlRepository.GetAsync(u => u.Code == code);
                return LitlUrl?.LongUrl ?? string.Empty;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occured.");
                return null;
            }            
        }
    }
}
