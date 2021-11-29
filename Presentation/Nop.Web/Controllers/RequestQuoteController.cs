using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Services.Catalog;
using Nop.Core.Domain.Quote;
using Nop.Web.Framework.EmailHelper;

namespace Nop.Web.Controllers
{
    public partial class RequestQuoteController : BasePublicController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IQuoteModelFactory _quoteModelFactory;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IQuoteService _quoteService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IShipmentService _shipmentService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        IProductService _productService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IFileManager _fileManager;
        private readonly IEmailService _emailService;

        #endregion

        #region Ctor

        public RequestQuoteController(ICustomerService customerService,
            IQuoteModelFactory quoteModelFactory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IShipmentService shipmentService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IQuoteService quoteService,
            ILocalizationService localizationService,
            IProductService productService,
            IFileManager fileManager,
            IEmailService emailService,
        RewardPointsSettings rewardPointsSettings)
        {
            _customerService = customerService;
            _quoteModelFactory = quoteModelFactory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _shipmentService = shipmentService;
            _webHelper = webHelper;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
            _localizationService = localizationService;
            _quoteService = quoteService;
            _productService = productService;
            _fileManager = fileManager;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        //My account / Orders
        public virtual async Task<IActionResult> CustomerQuotes()
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await _quoteModelFactory.PrepareCustomerQuoteListModelAsync();
            return View(model);
        }


        [HttpPost]
        public virtual async Task<IActionResult> DeleteCustomerQuoteItem(int quoteItemId)
        {
            try
            {
                await _quoteModelFactory.DeleteCustomerQuoteItem(quoteItemId);
                //var quoteItem = await _quoteService.GetCustomerQuoteItemByIdAsync(quoteItemId);
                //await _quoteService.DeleteCustomerQuoteItemAsync(quoteItem);
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.

                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Removed"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Removed.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateCustomerQuoteItemQunatity(int quoteItemId, int quantity, decimal price)
        {
            try
            {
                var quoteItem = await _quoteService.GetCustomerQuoteItemByIdAsync(quoteItemId);
                quoteItem.Quantity = quantity;
                quoteItem.UnitPriceInclTax = price;
                await _quoteService.UpdateCustomerQuoteItemAsync(quoteItem);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Saved"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Saved.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> RestoreCustomerQuoteItemId(int quoteItemId)
        {
            try
            {
                await _quoteModelFactory.RestoreCustomerQuoteItem(quoteItemId);
                //var quoteItem = await _quoteService.GetCustomerQuoteItemByIdAsync(quoteItemId);
                //quoteItem.Deleted = false;
                //await _quoteService.UpdateCustomerQuoteItemAsync(quoteItem);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteVendorQuoteItem(int quoteItemId) // quoteItemId is actually vendorQuiteItemId
        {
            try
            {
                var vendorQuoteItem = await _quoteService.GetVendorQuoteItemByIdAsync(quoteItemId);
                await _quoteService.DeleteVendorQuoteItemAsync(vendorQuoteItem);
                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.

                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Removed"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Removed.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateVendorQuoteItemQunatity(int quoteItemId, int quantity, decimal price) // quoteItemId is actually vendorQuiteItemId
        {
            try
            {
                var quoteItem = await _quoteService.GetVendorQuoteItemByIdAsync(quoteItemId);
                quoteItem.Quantity = quantity;
                quoteItem.UnitPriceInclTax = price;
                await _quoteService.UpdateVendorQuoteItemAsync(quoteItem);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Saved"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Saved.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> RestoreVendorQuoteItemId(int quoteItemId) // quoteItemId is actually vendorQuiteItemId
        {
            try
            {
                var quoteItem = await _quoteService.GetVendorQuoteItemByIdAsync(quoteItemId);
                quoteItem.Deleted = false;
                await _quoteService.UpdateVendorQuoteItemAsync(quoteItem);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> SearchProductForQuote(string productName)
        {
            try
            {
                var product = await _productService.GetProductBySkuOrNameAsync(productName);
                
                return Json(new
                {
                    success = true,
                    result = product,
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateVendorQuotes(int [] quoteItemId, int quoteId)
        {
            try
            {
                int error = await _quoteModelFactory.CreateVendorQuotes(quoteItemId, quoteId);
                if (error == 100)
                {
                    return Json(new
                    {
                        success = true,
                        message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                    });
                }
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendFilesToVendors(int[] fileIds, int [] targetIds, int quoteId) // targetIds are vendor ids
        {
            try
            {
                _fileManager.SendFilesToVendors(fileIds, targetIds, quoteId);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendFilesToCustomers(int[] fileIds, int[] targetIds, int quoteId) // targetIds are customer ids
        {
            try
            {
                _fileManager.SendFilesToCustomers(fileIds, targetIds, quoteId);

                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddProductInVendorQuote(string quoteGuid, int productId, int vendorId = 0) // targetIds are vendor ids
        {
            try
            {
                await _quoteModelFactory.AddProductInVendorQuote(quoteGuid, productId, vendorId);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddProductInCustomerQuote(string quoteGuid, int productId, int vendorId = 0) // targetIds are vendor ids
        {
            try
            {
                await _quoteModelFactory.AddProductInCustomerQuote(quoteGuid, productId);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        public virtual async Task<IActionResult> CopyResponseToCustomer(int[] vendorQuoteItemIds) 
        {
            try
            {
                await _quoteModelFactory.CopyResponseToCustomer(vendorQuoteItemIds);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        public virtual async Task<IActionResult> AddAttributesToQuote(List<QuoteAttribueMappingModel> attributeList, int QuoteId)
        {
            try
            {
                await _quoteModelFactory.AddAttributesToQuote(attributeList, QuoteId);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        public virtual async Task<IActionResult> UpdateQuoteStatus(int QuoteId, int Status)
        {
            try
            {
                await _quoteModelFactory.UpdateQuoteStatus(QuoteId, Status);
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored"),
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("QuoteItem.Restored.Error"),
                });
            }
        }

        public virtual async Task<IActionResult> GetCustomerQuoteGuid(int QuoteId)
        {
            try
            {
                var quote = await  _quoteService.GetCustomerQuoteByIdAsync(QuoteId);
                return Json(new
                {
                    success = true,
                    quoteGuid = quote.QuoteGuid,
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    quoteGuid = "0",
                });
            }
        }

        public virtual async Task<IActionResult> GetVendorQuoteGuid(int QuoteId)
        {
            try
            {
                var vendQuote = await _quoteService.GetVendorQuoteByIdAsync(QuoteId);
                var quote = await _quoteService.GetCustomerQuoteByIdAsync(vendQuote.CustomerQuoteId);
                return Json(new
                {
                    success = true,
                    quoteGuid = quote.QuoteGuid,
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    quoteGuid = "0",
                });
            }
        }

        public virtual async Task<IActionResult> SendEmailToNotSignInUsers(int quoteId, int [] userIdsList)
        {
            try
            {
                await _emailService.SendEmailToNotSignInUsers(quoteId, userIdsList.ToList());
                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                });
            }
        }
        #endregion
    }
}