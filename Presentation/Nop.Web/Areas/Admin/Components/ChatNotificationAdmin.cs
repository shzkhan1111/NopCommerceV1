﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ChatNotificationAdminViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public ChatNotificationAdminViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();
            return View(model);
        }
    }
}
