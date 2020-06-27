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
            IRequestCultureFeature cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            string currentCulture = cultureFeature?.RequestCulture.Culture.Name;

            string newCulture = Request.Form["NewCulture"];
            string returnUrl = Request.Form["ReturnUrl"];

            if (cultureFeature != null)
            {
                // ignore case
                returnUrl = returnUrl.Replace(currentCulture, newCulture, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                returnUrl = $"{newCulture}{returnUrl}";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(newCulture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(HttpUtility.UrlDecode(returnUrl));
        }
    }
}