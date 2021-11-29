using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ChatHeaderViewComponent : NopViewComponent
    {
        private readonly IChatModelFactory _chatModelFactory;

        public ChatHeaderViewComponent(IChatModelFactory chatModelFactory)
        {
            _chatModelFactory = chatModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string UserName, string CompanyName, string Initials, bool IsAdmin, 
            string QuoteGuid, string CustomerQuoteGuid, int QuoteId, DateTime QuoteDate)
        {
            var model = await _chatModelFactory.PrepareChatHeaderAsync(UserName, CompanyName, Initials, IsAdmin);
            model.QuoteGuid = QuoteGuid;
            model.CustomerQuoteGuid = CustomerQuoteGuid;
            model.QuoteId = QuoteId;
            model.QuoteDate = QuoteDate;
            return View(model);
        }
    }
}
