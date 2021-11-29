using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Common;

namespace Nop.Web.Components
{
    public class HomeSliderViewComponent : NopViewComponent
    {
        public HomeSliderViewComponent()
        {
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}