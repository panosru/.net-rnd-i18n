using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace i18n.Middlewares
{
    public class RedirectUnsupportedCultures : IRule
    {
        //private readonly string _extension;
        //private readonly PathString _newPath;
        private IList<CultureInfo> _cultureItems;
        private string _cultureRouteKey;

        public RedirectUnsupportedCultures(IOptions<RequestLocalizationOptions> options)
        {
            RouteDataRequestCultureProvider provider = options.Value.RequestCultureProviders
                .OfType<RouteDataRequestCultureProvider>()
                .First();

            _cultureItems = options.Value.SupportedUICultures;

            _cultureRouteKey = provider.RouteDataStringKey;
        }

        public void ApplyRule(RewriteContext rewriteContext)
        {
            // TODO find why non-existing resources that would give 404 (such as missing ico) send into infinite loop
            if (rewriteContext.HttpContext.Request.Path.Value.EndsWith(".ico") ||
                rewriteContext.HttpContext.Request.Path.Value.Contains("change-culture"))
            {
                return;
            }

            IRequestCultureFeature cultureFeature = rewriteContext.HttpContext.Features.Get<IRequestCultureFeature>();

            string actualCulture = cultureFeature?.RequestCulture.Culture.Name;

            string requestedCulture = rewriteContext.HttpContext.GetRouteValue(_cultureRouteKey)?.ToString();

            // TODO ensure to give precedence to cookie containing localization. Either redirect here, or changeculture model needs to be aware of more corner cases.
            if (string.IsNullOrEmpty(requestedCulture) || _cultureItems.All(x => x.Name != requestedCulture)
                && !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase))
            {
                string localizedPath = $"/{actualCulture}{rewriteContext.HttpContext.Request.Path.Value}".ToLower();

                HttpResponse response = rewriteContext.HttpContext.Response;
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                rewriteContext.Result = RuleResult.EndResponse;

                // preserve query part parameters of the URL (?parameters) if there were any
                response.Headers[HeaderNames.Location] =
                    localizedPath + rewriteContext.HttpContext.Request.QueryString;
            }
        }
    }
}