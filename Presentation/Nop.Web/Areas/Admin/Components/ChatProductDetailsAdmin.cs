using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Areas.Admin.Components
{
    public class ChatProductDetailsAdminViewComponent : NopViewComponent
    {
        private readonly IChatModelFactory _chatModelFactory;

        public ChatProductDetailsAdminViewComponent(IChatModelFactory chatModelFactory)
        {
            _chatModelFactory = chatModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string QuoteGuid, bool CustomerFlag)
        {
            var model = await _chatModelFactory.PrepareChatProductDetailsAsync();
            model.QuoteGuid = QuoteGuid;
            model.CustomerFlag = CustomerFlag;
            return View(model);
        }
    }
}
