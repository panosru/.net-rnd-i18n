using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using i18n.Resources;

namespace i18n.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStringLocalizer<IndexModel> _localizer;
        private readonly IStringLocalizer<SharedRes> _sharedLocalizer;

        public IndexModel(
            ILogger<IndexModel> logger,
            IStringLocalizer<IndexModel> localizer,
            IStringLocalizer<SharedRes> sharedLocalizer)
        {
            _logger = logger;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
        }

        public void OnGet()
        {
            _logger.LogInformation(_localizer["_WELCOME_"]);
            _logger.LogInformation(_sharedLocalizer["_COLOR_"]);
            _logger.LogDebug("test");
        }
    }
}
