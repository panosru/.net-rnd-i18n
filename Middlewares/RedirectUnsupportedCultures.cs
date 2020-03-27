using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace i18n.Middlewares
{
    public class RedirectUnsupportedCultures
    {
        private readonly RequestDelegate _next;
        private readonly string _routeDataStringKey;
        private readonly IList<CultureInfo> _cultureItems;

        public RedirectUnsupportedCultures(
        RequestDelegate next, IOptions<RequestLocalizationOptions> options)
        {
            _next = next;
            var provider = options.Value.RequestCultureProviders
            .Select(x => x as RouteDataRequestCultureProvider)
            .FirstOrDefault(x => x != null);

            _cultureItems = options.Value.SupportedUICultures;

            _routeDataStringKey = provider.RouteDataStringKey;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestedCulture = context.GetRouteValue(_routeDataStringKey)?.ToString();
            var cultureFeature = context.Features.Get<IRequestCultureFeature>();

            var actualCulture = cultureFeature?.RequestCulture.Culture.Name;

            if(string.IsNullOrEmpty(requestedCulture) || _cultureItems.All(x => x.Name != requestedCulture)
            && !string.Equals(requestedCulture, actualCulture, StringComparison.OrdinalIgnoreCase))
        {
                var newCulturedPath = GetNewPath(context, actualCulture);
                context.Response.Redirect(newCulturedPath);
                return;
            }

            await _next.Invoke(context);
        }

        private string GetNewPath(HttpContext context, string newCulture)
        {
            var routeData = context.GetRouteData();
            var router = routeData.Routers[0]; // Does not exist :(
            var virtualPathContext = new VirtualPathContext(context, routeData.Values, new RouteValueDictionary {
                { _routeDataStringKey, newCulture }
            });

            return router.GetVirtualPath(virtualPathContext).VirtualPath;
        }
    }
}
