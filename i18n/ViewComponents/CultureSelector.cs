using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace i18n.ViewComponents
{
    public class CultureSelector : ViewComponent
    {
        private string ReturnUrl;
        private string CurrentCulture;

        public CultureSelector(IHttpContextAccessor httpContextAccessor)
        {
            var request = httpContextAccessor.HttpContext.Request;

            ReturnUrl = string.IsNullOrEmpty(request.Path)
                ? "/"
                : $"{request.Path}{request.QueryString}";

            CurrentCulture = CultureInfo.CurrentCulture.Name;

            string newCulture = CurrentCulture switch
            {
                "en-GB" => "/en-US",
                "en-US" => "/en-GB",
                _ => $"/{Startup.DefaultRequestCulture.Culture.ToString().ToLower()}"
            };

            ReturnUrl = ReturnUrl.Replace($"/{CurrentCulture}", newCulture, StringComparison.OrdinalIgnoreCase).ToLower();
        }

        public IViewComponentResult Invoke()
        {
            return View("Default", new Dictionary<string, string>
            {
                {"ReturnUrl", ReturnUrl},
                {"CurrentCulture", CurrentCulture}
            });
        }
    }
}