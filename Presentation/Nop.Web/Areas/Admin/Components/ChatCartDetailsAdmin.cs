using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Quote;

namespace Nop.Web.Areas.Admin.Components
{
    public class ChatCartDetailsAdminViewComponent : NopViewComponent
    {
        private readonly IQuoteModelFactory _quoteModelFactory;

        public ChatCartDetailsAdminViewComponent(IQuoteModelFactory quoteModelFactory)
        {
            _quoteModelFactory = quoteModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int QuoteId, bool customerFlag, bool IsAdmin)
        {
            CustomerQuoteListModel model = new CustomerQuoteListModel();
            if(customerFlag)
                 model = await _quoteModelFactory.PrepareCustomerQuoteDetailListModelAsync(QuoteId);
            else
                model = await _quoteModelFactory.PrepareVendorQuoteDetailListModelAsync(QuoteId);
            model.IsAdmin = IsAdmin;
            return View(model);
        }
    }
}
