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
        public IActionResult OnPost()
        {
            var NewCulture = Request.Form["NewCulture"];
            var ReturnUrl = Request.Form["ReturnUrl"];

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(NewCulture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(HttpUtility.UrlDecode(ReturnUrl));
        }
    }
}
