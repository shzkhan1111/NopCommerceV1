using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Quote;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using Nop.Web.Models.Quote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class MessengerController : BasePublicController
    {
        private readonly IQuoteService _quoteService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;

        public MessengerController(IQuoteService quoteService, ICustomerService customerService, IWorkContext workContext)
        {
            _quoteService = quoteService;
            _customerService = customerService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> Index(string id, bool isVendor = false, int vendorId = 0)
        {
            Guid guid = new Guid(id);
            MessengerModel model = new MessengerModel();

            if (!isVendor)
            {
                CustomerQuote customerQuote = await _quoteService.GetCustomerQuoteByGuidAsync(guid);
                VendorQuote vendorQuote = await _quoteService.GetVendorQuoteByGuidAsync(guid);
                if (customerQuote != null)
                {
                    var customer = await _workContext.GetCurrentCustomerAsync();
                    model.QuoteId = customerQuote.Id;
                    model.CustomerId = customer.Id;
                    model.IsAdmin = await _customerService.IsAdminAsync(customer);
                    model.VendorId = 0;
                    model.CustomerQuoteGuid = id;
                    model.QuoteDate = customerQuote.CreatedOn;
                }
                else
                {

                    model.QuoteId = vendorQuote.Id;
                    model.VendorId = vendorQuote.VendorId;
                    model.VendorQuoteId = vendorQuote.Id;
                    model.CustomerId = 0;
                    model.QuoteDate = vendorQuote.CreatedOn;
                    customerQuote = await _quoteService.GetCustomerQuoteByIdAsync(vendorQuote.CustomerQuoteId);
                    model.CustomerQuoteGuid = customerQuote.QuoteGuid.ToString();
                }
                model.Guid = id;
            }
            else
            { 
                CustomerQuote customerQuote = await _quoteService.GetCustomerQuoteByGuidAsync(guid);
                VendorQuote vendorQuote = null;
                if (vendorId > 0)
                { 
                    vendorQuote = await _quoteService.GetVendorQuoteByQuoteIdAndVendorAsync(customerQuote.Id, vendorId);
                }
                else
                {
                    vendorQuote = await _quoteService.GetVendorQuoteByQuoteIdAsync(customerQuote.Id);
                }
                var customer = await _workContext.GetCurrentCustomerAsync();
                model.IsAdmin = await _customerService.IsAdminAsync(customer);
                model.QuoteId = vendorQuote.Id;
                model.VendorId = vendorQuote.VendorId;
                model.VendorQuoteId = vendorQuote.Id;
                model.CustomerId = 0;
                model.CustomerQuoteGuid = customerQuote.QuoteGuid.ToString();
                model.Guid = id;
                model.QuoteDate = vendorQuote.CreatedOn;
            }
            return View(model);
        }

    }
}
