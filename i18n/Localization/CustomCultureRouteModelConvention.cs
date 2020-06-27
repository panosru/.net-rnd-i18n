using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace i18n.Localization
{
    public class CustomCultureRouteModelConvention : IPageRouteModelConvention
    {
        //public void Apply(PageRouteModel model)
        //{
        //    (from SelectorModel in model.Selectors
        //     select new SelectorModel
        //     {
        //         AttributeRouteModel = new AttributeRouteModel
        //         {
        //             Template = AttributeRouteModel.CombineTemplates("/{culture:required}",
        //                SelectorModel.AttributeRouteModel.Template)
        //         }
        //     })
        //     .ToList()
        //     .ForEach(s => model.Selectors.Add(s));
        //}

        public void Apply(PageRouteModel model)
        {
            var selectorCount = model.Selectors.Count;

            for (var i = 0; i < selectorCount; i++)
            {
                var selector = model.Selectors[i];

                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Order = -1,
                        Template = AttributeRouteModel.CombineTemplates(
                              "{culture}", selector.AttributeRouteModel.Template),
                    }
                });

                //selector.AttributeRouteModel.SuppressLinkGeneration = true;
            }
        }
    }
}
