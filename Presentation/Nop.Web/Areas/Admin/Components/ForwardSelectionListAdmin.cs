using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Common;

namespace Nop.Web.Areas.Admin.Components
{
    public class ForwardSelectionListAdminViewComponent : NopViewComponent
    {
        private readonly IChatModelFactory _chatModelFactory;

        public ForwardSelectionListAdminViewComponent(IChatModelFactory chatModelFactory)
        {
            _chatModelFactory = chatModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int customerQuoteId, bool isCustomer)
        {
            var model = await _chatModelFactory.PrepareForwardSelectionListAsync(customerQuoteId, isCustomer);
            return View(model);
        }
    }
}