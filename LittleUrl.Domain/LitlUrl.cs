namespace LittleUrl.Domain
{
    public class LitlUrl
    {
        public LitlUrl(string code, string longUrl)
        {
            this.Code = code;
            this.LongUrl = longUrl;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string LongUrl { get; set; }

        // Out of scope, but could be expanded upon to allow for expiry date and visit count if we wanted LitlUrls to expire and/or be reported on.
    }
}