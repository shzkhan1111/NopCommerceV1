using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Quote;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Quote;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class QuoteController : BaseAdminController
    {
        #region Fields

        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IEncryptionService _encryptionService;
        private readonly IExportManager _exportManager;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IQuoteModelFactory _quoteModelFactory;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IQuoteService _quoteService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public QuoteController(IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IEncryptionService encryptionService,
            IExportManager exportManager,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IQuoteModelFactory quoteModelFactory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IQuoteService quoteService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            OrderSettings orderSettings)
        {
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _encryptionService = encryptionService;
            _exportManager = exportManager;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _quoteModelFactory = quoteModelFactory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _permissionService = permissionService;
            _priceCalculationService = priceCalculationService;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _orderSettings = orderSettings;
            _quoteService = quoteService;
        }

        #endregion


        #region Order list

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await _quoteModelFactory.PrepareQuoteSearchModelAsync(new QuoteSearchModel
            {
                OrderStatusIds = orderStatuses
            });

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> QuoteList(QuoteSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _quoteModelFactory.PrepareQuoteListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-quote-by-number")]
        public virtual async Task<IActionResult> GoToQuoteId(QuoteSearchModel searchModel)
        {
            var quote = await _quoteService.GetCustomerQuoteByCustomQuoteNumberAsync(searchModel.GoDirectlyToCustomQuoteNumber);

            if (quote == null)
                return await List();

            return RedirectToAction("Messenger", new { id = quote.QuoteGuid });
        }

        public virtual async Task<IActionResult> GetQuoteGuid(int CustomeQuoteNumber)
        {
            try
            {
                var guid = "-1";
                var quote = await _quoteService.GetCustomerQuoteByCustomQuoteNumberAsync(CustomeQuoteNumber.ToString());
                if (quote != null)
                    guid = quote.QuoteGuid.ToString();


                return Json(new
                {
                    success = true,
                    data = guid,
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

        public virtual async Task<IActionResult> Messenger(string id, bool isVendor = false, int vendorId = 0)
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
                    model.CustomQuoteId = customerQuote.Id;
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
                    model.CustomQuoteId = customerQuote.Id;
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
                model.CustomQuoteId = customerQuote.Id;
            }
            return View(model);
        }


        public virtual async Task<IActionResult> PdfQuote(int quoteId)
        {
            //a vendor should have access only to his products
            var vendorId = 0;
            if (await _workContext.GetCurrentVendorAsync() != null)
            {
                vendorId = (await _workContext.GetCurrentVendorAsync()).Id;
            }

            var order = await _quoteService.GetCustomerQuoteByIdAsync(quoteId);
            var orders = new List<CustomerQuote>
            {
                order
            };

            byte[] bytes;
            await using (var stream = new MemoryStream())
            {
                await _pdfService.PrintQuotesToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await _workContext.GetWorkingLanguageAsync()).Id, vendorId);
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, $"quote_{order.Id}.pdf");
        }

        //[HttpPost, ActionName("PdfQuote")]
        //[FormValueRequired("pdf-quotes-all")]
        [HttpPost]
        public virtual async Task<IActionResult> PdfQuoteAll(QuoteSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var loggedInCustomer = await _workContext.GetCurrentCustomerAsync();
            //get parameters to filter orders
            var orderStatusIds = (model.OrderStatusIds?.Contains(0) ?? true) ? null : model.OrderStatusIds.ToList();
            if (await _workContext.GetCurrentVendorAsync() != null)
                model.VendorId = (await _workContext.GetCurrentVendorAsync()).Id;
            var startDateValue = !model.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !model.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var product = await _productService.GetProductByIdAsync(model.ProductId);
            var filterByProductId = product != null && (await _workContext.GetCurrentVendorAsync() == null || product.VendorId == (await _workContext.GetCurrentVendorAsync()).Id)
                ? model.ProductId : 0;

            //get orders
            
            var quotes = await _quoteService.SearchCustomerQuoteAsync(vendorId: model.VendorId,
            productId: filterByProductId,
            CreatedOnFrom: startDateValue,
            CreatedOnTo: endDateValue,
            osIds: orderStatusIds,
            pageIndex: model.Page - 1, pageSize: model.PageSize);

            //ensure that we at least one order selected
            if (!quotes.Any())
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Quotes.NoQuotes"));
                return RedirectToAction("List");
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintQuotesToPdfAsync(stream, quotes, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await _workContext.GetWorkingLanguageAsync()).Id, model.VendorId);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "quotes.pdf");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        private async Task<List<CustomerQuote>> GetCustomerQuoteList(int [] ids)
        {
            var quotes = new List<CustomerQuote>();
            foreach (var id in ids)
            {
                var quote = await _quoteService.GetCustomerQuoteByIdAsync(id);
                quotes.Add(quote);
            }
            return quotes;
        }

        [HttpPost]
        public virtual async Task<IActionResult> PdfQuoteSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var quotes = new List<CustomerQuote>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                quotes = await GetCustomerQuoteList(ids);
            }

            

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintQuotesToPdfAsync(stream, quotes, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : (await _workContext.GetWorkingLanguageAsync()).Id, 0);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "quotes.pdf");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var quotes = new List<CustomerQuote>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                quotes = await GetCustomerQuoteList(ids);
            }

            try
            {
                var bytes = await _exportManager.ExportQuotesToXlsxAsync(quotes);
                return File(bytes, MimeTypes.TextXlsx, "quotes.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var quotes = new List<CustomerQuote>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                quotes = await GetCustomerQuoteList(ids);
            }

            try
            {
                var xml = await _exportManager.ExportQuotesToXmlAsync(quotes);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "quotes.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }
        #endregion

    }
}
