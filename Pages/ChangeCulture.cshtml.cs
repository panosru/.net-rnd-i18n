using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace i18n.Pages
{
    public class ChangeCultureModel : PageModel
    {
        public string NewCulture { get; set; }
        public string ReturnUrl { get; set; }

        public IActionResult OnGet(string NewCulture, string ReturnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(NewCulture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            
            return LocalRedirect(HttpUtility.UrlDecode(ReturnUrl));
        }
    }
}
