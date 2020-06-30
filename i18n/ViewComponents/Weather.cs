using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace i18n.ViewComponents
{
    public class Weather : ViewComponent
    {
        private double Temperature;

        public Weather()
        {
            Random rand = new Random();
            Temperature = rand.Next(-14, 42);

            if("en-US" == CultureInfo.CurrentCulture.Name)
            {
                Temperature = Temperature * 1.8 + 32;
            }

            Temperature = Math.Round(Temperature, 0);
        }

        public IViewComponentResult Invoke()
        {
            return View("Default", (object)Temperature);
        }
    }
}
