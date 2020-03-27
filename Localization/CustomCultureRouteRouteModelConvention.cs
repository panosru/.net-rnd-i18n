using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace i18n.Localization
{
    public class CustomCultureRouteRouteModelConvention : IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            (from SelectorModel in model.Selectors
             select new SelectorModel
             {
                 AttributeRouteModel = new AttributeRouteModel
                 {
                     Template = AttributeRouteModel.CombineTemplates("/{culture?}", SelectorModel.AttributeRouteModel.Template)
                 }
             })
             .ToList()
             .ForEach(s => model.Selectors.Add(s));
        }
    }
}
