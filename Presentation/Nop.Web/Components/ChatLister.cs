using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Common;

namespace Nop.Web.Components
{
    public class ChatListerViewComponent : NopViewComponent
    {
        private readonly IChatModelFactory _chatModelFactory;

        public ChatListerViewComponent(IChatModelFactory chatModelFactory)
        {
            _chatModelFactory = chatModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string UserName, string CompanyName, string Initials, int QuoteId, bool customerFlag, bool IsAdmin, DateTime QuoteDate, int UserId, string QuoteGuid, string CustomerQuoteGuid)
        {
            var model = await _chatModelFactory.PrepareChatListerAsync(UserName, CompanyName, Initials, QuoteId);
            model.CustomerFlag = customerFlag;
            model.UserId = UserId;
            model.IsAdmin = IsAdmin;
            model.QuoteDate = QuoteDate;
            model.QuoteGuid = QuoteGuid;
            model.CustomerQuoteGuid = CustomerQuoteGuid;
            return View(model);
        }
    }
}