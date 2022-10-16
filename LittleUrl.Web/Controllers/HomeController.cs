using Microsoft.AspNetCore.Mvc;
using LittleUrl.Website.Services;
using LittleUrl.Website.Models;
using System.Diagnostics;

namespace LittleUrl.Website.Controllers
{
    public class HomeController : Controller
    {
        readonly ILitlUrlService LitlUrlService;
        private readonly ILogger<HomeController> logger;

        public HomeController(ILitlUrlService LitlUrlService, ILogger<HomeController> logger)
        {
            this.LitlUrlService = LitlUrlService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        public async Task<ActionResult<string>> Post(string longUrl)
        {
            // check the longUrl is an actual Url
            if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
                return BadRequest("We're sorry, but that's not a valid url - why don't you try again.");

            var code = await LitlUrlService.AddLitlUrl(longUrl);

            return code == null
                ? StatusCode(500, "We're really sorry, but an error occured creating your li.tl.  Please try again later.")
                : Ok(code);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult> Get(string code)
        {
            if (code == null
                || code.Trim().Length == 0) return BadRequest("You must provide a code to use li.tl.");

            var longUrl = await LitlUrlService.GetWithCode(code);

            return longUrl == null
                ? StatusCode(500, "We're really sorry, but an error occured getting your li.tl.  Please try again later.")
                : (longUrl.Trim().Length > 0
                    ? Redirect(longUrl)
                    : NotFound("We're sorry, but that is not a valid li.tl url.")
                );
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}