using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the order model factory implementation
    /// </summary>
    public partial class QuoteModelFactory : IQuoteModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IAffiliateService _affiliateService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IEncryptionService _encryptionService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderReportService _orderReportService;
        private readonly IQuoteService _quoteService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly OrderSettings _orderSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IUrlRecordService _urlRecordService;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public QuoteModelFactory(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAffiliateService affiliateService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDownloadService downloadService,
            IEncryptionService encryptionService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IOrderProcessingService orderProcessingService,
            IOrderReportService orderReportService,
            IQuoteService quoteService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            ITaxService taxService,
            IUrlHelperFactory urlHelperFactory,
            IVendorService vendorService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            IUrlRecordService urlRecordService,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _affiliateService = affiliateService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _downloadService = downloadService;
            _encryptionService = encryptionService;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _measureService = measureService;
            _orderProcessingService = orderProcessingService;
            _orderReportService = orderReportService;
            _quoteService = quoteService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _pictureService = pictureService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _returnRequestService = returnRequestService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _storeService = storeService;
            _taxService = taxService;
            _urlHelperFactory = urlHelperFactory;
            _vendorService = vendorService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _orderSettings = orderSettings;
            _shippingSettings = shippingSettings;
            _urlRecordService = urlRecordService;
            _taxSettings = taxSettings;
        }

        #endregion

        /// <summary>
        /// Prepare order search model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order search model
        /// </returns>
        public virtual async Task<QuoteSearchModel> PrepareQuoteSearchModelAsync(QuoteSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available order, payment and shipping statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare grid
            searchModel.SetGridPageSize();


            return searchModel;
        }

        /// <summary>
        /// Prepare paged order list model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order list model
        /// </returns>
        public virtual async Task<QuoteListModel> PrepareQuoteListModelAsync(QuoteSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            try
            {
                var loggedInCustomer = await _workContext.GetCurrentCustomerAsync();
                //get parameters to filter orders
                var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
                if (await _workContext.GetCurrentVendorAsync() != null)
                    searchModel.VendorId = (await _workContext.GetCurrentVendorAsync()).Id;
                var startDateValue = !searchModel.StartDate.HasValue ? null
                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
                var endDateValue = !searchModel.EndDate.HasValue ? null
                    : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
                var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
                var filterByProductId = product != null && (await _workContext.GetCurrentVendorAsync() == null || product.VendorId == (await _workContext.GetCurrentVendorAsync()).Id)
                    ? searchModel.ProductId : 0;

                //get orders
                QuoteListModel model = null;
                if (loggedInCustomer.VendorId == 0)
                {
                    var quotes = await _quoteService.SearchCustomerQuoteAsync(vendorId: searchModel.VendorId,
                    productId: filterByProductId,
                    CreatedOnFrom: startDateValue,
                    CreatedOnTo: endDateValue,
                    osIds: orderStatusIds,
                    pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

                    model = await new QuoteListModel().PrepareToGridAsync(searchModel, quotes, () =>
                    {
                    //fill in model values from the entity
                    return quotes.SelectAwait(async quote =>
                        {
                            var customer = await _customerService.GetCustomerByIdAsync(quote.CustomerId);
                        //fill in model values from the entity
                        var quoteModel = new QuoteModel
                            {
                                Id = quote.Id,
                                QuoteStatusId = quote.Status,
                                CustomerEmail = customer.Email,
                                CustomerFullName = customer.Username,
                                CustomerId = quote.CustomerId,
                                CustomQuoteNumber = quote.CustomQuoteNumber,
                                QuoteGuid = quote.QuoteGuid
                            };

                        //convert dates to the user time
                        quoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quote.CreatedOn, DateTimeKind.Utc);

                            quoteModel.QuoteStatus = await _localizationService.GetLocalizedEnumAsync(quote.OrderStatus);

                            return quoteModel;
                        });
                    });
                }
                else if (loggedInCustomer.VendorId > 0)
                {

                    var vendorQuotes = await _quoteService.SearchVendorQuoteAsync(vendorId: searchModel.VendorId,
                        productId: filterByProductId,
                        CreatedOnFrom: startDateValue,
                        CreatedOnTo: endDateValue,
                        osIds: orderStatusIds,
                        pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

                    model = await new QuoteListModel().PrepareToGridAsync(searchModel, vendorQuotes, () =>
                    {
                        //fill in model values from the entity
                        return vendorQuotes.SelectAwait(async quote =>
                        {
                            var customer = await _customerService.GetCustomerByIdAsync(quote.CustomerId);
                            //fill in model values from the entity
                            var quoteModel = new QuoteModel
                            {
                                Id = quote.Id,
                                QuoteStatusId = 10,
                                CustomerEmail = customer.Email,
                                CustomerFullName = customer.Username,
                                CustomerId = quote.CustomerId,
                                CustomQuoteNumber = quote.CustomQuoteNumber,
                                QuoteGuid = quote.QuoteGuid
                            };

                            //convert dates to the user time
                            quoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quote.CreatedOn, DateTimeKind.Utc);
                            quoteModel.QuoteStatus = "Pending";

                            return quoteModel;
                        });
                    });

                }
                //prepare list model
                
            return model;
            }
            catch (Exception ex)
            { 
            
            }
            return null;
        }

    }
}