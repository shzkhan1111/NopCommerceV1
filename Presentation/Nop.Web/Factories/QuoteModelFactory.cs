using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Quote;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Models.Common;
using Nop.Web.Models.Order;
using Nop.Web.Models.Quote;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial class QuoteModelFactory : IQuoteModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IQuoteService _quoteService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public QuoteModelFactory(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IGenericAttributeService genericAttributeService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PdfSettings pdfSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            IQuoteService quoteService,
            VendorSettings vendorSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _pdfSettings = pdfSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _quoteService = quoteService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer order list model
        /// </returns>
        public virtual async Task<CustomerQuoteListModel> PrepareCustomerQuoteListModelAsync()
        {
            var model = new CustomerQuoteListModel();
            var quotes = await _quoteService.SearchCustomerQuoteAsync(customerId: (await _workContext.GetCurrentCustomerAsync()).Id);
            foreach (var quote in quotes)
            {
                var quoteModel = new CustomerQuoteListModel.QuoteDetailsModel
                {
                    Id = quote.Id,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quote.CreatedOn, DateTimeKind.Utc),
                    QuoteStatusEnum = (OrderStatus)quote.Status,
                    CustomQuoteNumber = quote.CustomQuoteNumber,
                    QuoteStatus = quote.Status.ToString(),
                    QuoteGuid = quote.QuoteGuid.ToString()
                };
                quoteModel.QuoteTotal = (await _quoteService.GetCustomerQuoteItemsAsync(quote.Id)).Sum(x => x.Quantity).ToString();

                model.Quotes.Add(quoteModel);
            }

            return model;
        }

        public virtual async Task<CustomerQuoteListModel> PrepareCustomerQuoteDetailListModelAsync(int QuoteId)
        {
            var model = new CustomerQuoteListModel();
            var customerQuote = await _quoteService.GetCustomerQuoteByIdAsync(QuoteId);
            var customer = await _customerService.GetCustomerByIdAsync(customerQuote.CustomerId);
            model.CustomerId = customer.Id;

            string firstName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            string lastName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);
            string company = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute);
            var customerQuoteModel = new CustomerQuoteListModel.QuoteDetailsModel
            {
                Id = customerQuote.Id,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customerQuote.CreatedOn, DateTimeKind.Utc),
                QuoteStatusEnum = (OrderStatus)customerQuote.Status,
                CustomQuoteNumber = customerQuote.CustomQuoteNumber,
                QuoteStatus = customerQuote.Status.ToString(),
                CustomerName = firstName + " " + lastName,
                CompanyName = company,
                QuoteGuid = customerQuote.QuoteGuid.ToString()
            };
            customerQuoteModel.QuoteAttributeList = await GetQuoteAttributeMappingModelAsync(customerQuoteModel.Id);

            model.Quotes.Add(customerQuoteModel);

            var quoteItems = await _quoteService.GetCustomerQuoteItemsAsync(QuoteId);
            foreach (var quote in quoteItems)
            {
                var product = await _quoteService.GetProductByCustomerQuoteItemIdAsync(quote.Id);
                var quoteModel = new CustomerQuoteListModel.QuoteItemDetailsModel
                {
                    QuoteItemGuid = quote.QuoteItemGuid,
                    Deleted = quote.Deleted,
                    Id = quote.Id,
                    PriceExclTax = quote.PriceExclTax,
                    ProductId = quote.ProductId,
                    ProductName = product.Name,
                    SKU = product.Sku,
                    PriceIncTax = quote.PriceIncTax,
                    Quantity = quote.Quantity,
                    QuoteId = quote.QuoteId,
                    TermsOfSale = quote.TermsOfSale,
                    UnitPriceExclTax = quote.UnitPriceExclTax,
                    UnitPriceInclTax = quote.UnitPriceInclTax
                };

                model.Quotes[0].QuoteItems.Add(quoteModel);
            }

            return model;
        }

        public virtual async Task<CustomerQuoteListModel> PrepareVendorQuoteListModelAsync()
        {
            var model = new CustomerQuoteListModel();
            var quotes = await _quoteService.SearchVendorQuoteAsync(vendorId: (await _workContext.GetCurrentCustomerAsync()).VendorId);
            foreach (var quote in quotes)
            {
                var quoteModel = new CustomerQuoteListModel.QuoteDetailsModel
                {
                    Id = quote.Id,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quote.CreatedOn, DateTimeKind.Utc),
                    CustomQuoteNumber = quote.CustomQuoteNumber,
                    QuoteGuid = quote.QuoteGuid.ToString()
                };
                quoteModel.QuoteTotal = (await _quoteService.GetCustomerQuoteItemsAsync(quote.Id)).Sum(x => x.Quantity).ToString();

                model.Quotes.Add(quoteModel);
            }

            return model;
        }

        public virtual async Task<CustomerQuoteListModel> PrepareVendorQuoteDetailListModelAsync(int VendorQuoteId)
        {
            var model = new CustomerQuoteListModel();
            var vendorQuote = await _quoteService.GetVendorQuoteByIdAsync(VendorQuoteId);
            var vendor = await _vendorService.GetVendorByIdAsync(vendorQuote.VendorId);
            model.VendorId = vendor.Id;

            //string firstName = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopCustomerDefaults.FirstNameAttribute);
            //string lastName = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopCustomerDefaults.LastNameAttribute);
            //string company = await _genericAttributeService.GetAttributeAsync<string>(vendor, NopCustomerDefaults.CompanyAttribute);

            var vendorQuoteModel = new CustomerQuoteListModel.QuoteDetailsModel
            {
                Id = vendorQuote.Id,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(vendorQuote.CreatedOn, DateTimeKind.Utc),
                CustomQuoteNumber = vendorQuote.CustomQuoteNumber,
                CustomerName = vendor.Name,
                CompanyName = "",
                QuoteGuid = vendorQuote.QuoteGuid.ToString()
            };

            model.Quotes.Add(vendorQuoteModel);

            var quoteItems = await _quoteService.GetVendorQuoteItemsAsync(VendorQuoteId);
            foreach (var quote in quoteItems)
            {
                var product = await _quoteService.GetProductByVendorQuoteItemIdAsync(quote.Id);
                var quoteModel = new CustomerQuoteListModel.QuoteItemDetailsModel
                {
                    QuoteItemGuid = quote.QuoteItemGuid,
                    Deleted = quote.Deleted,
                    Id = quote.Id,
                    PriceExclTax = quote.PriceExclTax,
                    ProductId = quote.ProductId,
                    ProductName = product.Name,
                    SKU = product.Sku,
                    PriceIncTax = quote.PriceIncTax,
                    Quantity = quote.Quantity,
                    QuoteId = quote.QuoteId,
                    TermsOfSale = quote.TermsOfSale,
                    UnitPriceExclTax = quote.UnitPriceExclTax,
                    UnitPriceInclTax = quote.UnitPriceInclTax
                };

                model.Quotes[0].QuoteItems.Add(quoteModel);
            }

            return model;
        }

        private async Task<VendorQuote> CreateVendorQuote(CustomerQuoteItemModel quoteModel, int CustomerId)
        {
            var vendorQuote = await _quoteService.GetVendorQuoteByCustomerQuoteIdAsync(quoteModel.QuoteId);
            if (vendorQuote == null)
            {
                vendorQuote = new VendorQuote();
                vendorQuote.VendorId = quoteModel.VendorId;
                vendorQuote.CustomerId = CustomerId;
                vendorQuote.CreatedOn = DateTime.Now;
                vendorQuote.Deleted = false;
                vendorQuote.QuoteDate = DateTime.Now;
                vendorQuote.QuoteGuid = Guid.NewGuid();
                vendorQuote.CustomerQuoteId = quoteModel.QuoteId;
                await _quoteService.InsertVendorQuoteAsync(vendorQuote);
                vendorQuote.CustomQuoteNumber = vendorQuote.Id.ToString();
                await _quoteService.UpdateVendorQuoteAsync(vendorQuote);
            }

            return vendorQuote;
        }

        private async void CreateVendorQuoteItem(int vendorQuoteId, CustomerQuoteItemModel quoteModel)
        {
            var oldVendQuoteItem = await _quoteService.GetVendorQuoteItemByQuoteAndProductIdAsync(vendorQuoteId, quoteModel.ProductId);
            if (oldVendQuoteItem == null)
            {
                VendorQuoteItem vendorQuoteItem = new VendorQuoteItem();
                vendorQuoteItem.PriceExclTax = 0;
                vendorQuoteItem.PriceIncTax = 0;
                vendorQuoteItem.UnitPriceInclTax = 0;
                vendorQuoteItem.UnitPriceExclTax = 0;
                vendorQuoteItem.ProductId = quoteModel.ProductId;
                vendorQuoteItem.Quantity = quoteModel.Quantity;
                vendorQuoteItem.QuoteId = vendorQuoteId;
                vendorQuoteItem.QuoteItemGuid = Guid.NewGuid();
                vendorQuoteItem.QuoteId = vendorQuoteId;
                await _quoteService.InsertVendorQuoteItemAsync(vendorQuoteItem);
            }
        }

        public virtual async Task<int>  CreateVendorQuotes(int [] QuoteItemIds, int QuoteId)
        {
            int errorFlag = 100;

            try
            {
                var customerQuote = await _quoteService.GetCustomerQuoteByIdAsync(QuoteId);

                var quoteItems = await _quoteService.GetCustomerQuoteItemsModelAsync(QuoteId);
                quoteItems = quoteItems.OrderBy(x => x.VendorId).ToList();
                bool newQuoteFlag = false;
                int vendorQuoteId = 0;
                List<int> vendorIds = new List<int>();
                if (quoteItems != null && quoteItems.Count > 0)
                {
                    int count = 0;
                    for (int i = 0; i < quoteItems.Count; i++)
                    {
                        if (QuoteItemIds.Contains(quoteItems[i].Id))
                        {
                            if (count == 0 || (quoteItems[i].VendorId != quoteItems[i - 1].VendorId && !vendorIds.Contains(quoteItems[i].VendorId)))
                            {
                                vendorIds.Add(quoteItems[i].VendorId);
                                newQuoteFlag = true;
                            }

                            if (newQuoteFlag)
                            {
                                var oldVendorQuote = await CreateVendorQuote(quoteItems[i], customerQuote.CustomerId);
                                vendorQuoteId = oldVendorQuote.Id;

                                var vendorQuoteItems = quoteItems.Where(x => x.VendorId == quoteItems[i].VendorId).ToList();
                                foreach (var vendorQuoteItem in vendorQuoteItems)
                                { 
                                    CreateVendorQuoteItem(vendorQuoteId, vendorQuoteItem);
                                }
                            }
                            count++;
                        }
                    }

                    await UpdateQuoteStatus(customerQuote.Id, (int)OrderStatus.Processing);
                    //customerQuote.Status = (int)OrderStatus.Processing;
                    //await _quoteService.UpdateCustomerQuoteAsync(customerQuote);
                }
            }
            catch (Exception ex)
            {
                errorFlag = 420;
            }
            return errorFlag;
        }


        public virtual async Task<int> AddProductInVendorQuote(string QuoteGuid, int ProductId, int VendorId)
        {
            int errorFlag = 100;

            try
            {
                Guid guid = new Guid(QuoteGuid);
                var customerQuote = await _quoteService.GetCustomerQuoteByGuidAsync(guid);
                var customerQuoteItems = await _quoteService.GetCustomerQuoteItemsModelAsync(customerQuote.Id);
                var quoteItem = customerQuoteItems.Where(x => x.ProductId == ProductId).FirstOrDefault();

                if (quoteItem != null)
                {
                    var vendorQuote = await CreateVendorQuote(quoteItem, customerQuote.CustomerId);
                    CreateVendorQuoteItem(vendorQuote.Id, quoteItem);
                }
                else
                {
                    var vendorQuote = await _quoteService.GetVendorQuoteByQuoteIdAndVendorAsync(customerQuote.Id, VendorId);

                    if (vendorQuote == null)
                    {
                        vendorQuote = new VendorQuote();
                        vendorQuote.QuoteDate = DateTime.Now;
                        vendorQuote.QuoteGuid = Guid.NewGuid();
                        vendorQuote.VendorId = VendorId;
                        vendorQuote.CreatedOn = DateTime.Now;
                        vendorQuote.CustomerId = customerQuote.CustomerId;
                        await _quoteService.InsertVendorQuoteAsync(vendorQuote);
                        vendorQuote.CustomerQuoteId = vendorQuote.Id;
                        await _quoteService.UpdateVendorQuoteAsync(vendorQuote);
                    }

                    var vendorQuoteItems = await _quoteService.GetVendorQuoteItemsAsync(vendorQuote.Id);
                    VendorQuoteItem oldQuoteItem = null;

                    if (vendorQuoteItems != null && vendorQuoteItems.Count > 0)
                    {
                        oldQuoteItem = vendorQuoteItems.Where(x => x.ProductId == ProductId).FirstOrDefault();
                    }

                    if (oldQuoteItem == null)
                    {
                        VendorQuoteItem vendorQuoteItem = new VendorQuoteItem();
                        vendorQuoteItem.QuoteId = vendorQuote.Id;
                        vendorQuoteItem.ProductId = ProductId;
                        vendorQuoteItem.Quantity = 1;
                        vendorQuoteItem.QuoteItemGuid = Guid.NewGuid();
                        vendorQuoteItem.PriceExclTax = 0;
                        vendorQuoteItem.PriceIncTax = 0;
                        vendorQuoteItem.UnitPriceInclTax = 0;
                        vendorQuoteItem.UnitPriceExclTax = 0;

                        await _quoteService.InsertVendorQuoteItemAsync(vendorQuoteItem);
                    }
                }

            }
            catch (Exception ex)
            {
                errorFlag = 420;
            }
            return errorFlag;
        }


        public virtual async Task<int> AddProductInCustomerQuote(string QuoteGuid, int ProductId)
        {
            int errorFlag = 100;

            try
            {
                Guid guid = new Guid(QuoteGuid);
                var customerQuote = await _quoteService.GetCustomerQuoteByGuidAsync(guid);
                var customerQuoteItems = await _quoteService.GetCustomerQuoteItemsModelAsync(customerQuote.Id);
                var quoteItem = customerQuoteItems.Where(x => x.ProductId == ProductId).FirstOrDefault();

                if (quoteItem == null)
                {
                    CustomerQuoteItem customerQuoteItem = new CustomerQuoteItem();
                    customerQuoteItem.QuoteId = customerQuote.Id;
                    customerQuoteItem.ProductId = ProductId;
                    customerQuoteItem.Quantity = 1;
                    customerQuoteItem.QuoteItemGuid = Guid.NewGuid();
                    customerQuoteItem.PriceExclTax = 0;
                    customerQuoteItem.PriceIncTax = 0;
                    customerQuoteItem.UnitPriceInclTax = 0;
                    customerQuoteItem.UnitPriceExclTax = 0;
                    await _quoteService.InsertCustomerQuoteItemAsync(customerQuoteItem);
                }
            }
            catch (Exception ex)
            {
                errorFlag = 420;
            }
            return errorFlag;
        }

        public virtual async Task<int> AddProductToVendorQuote(string sku, int customerQuoteId)
        {
            int errorFlag = 100;
            try
            {
                var customerQuote = await _quoteService.GetCustomerQuoteByIdAsync(customerQuoteId);
                var quoteItems = await _quoteService.GetCustomerQuoteItemsModelAsync(customerQuoteId);
                var product = await _productService.GetProductBySkuAsync(sku);

                var productQuoteItem = quoteItems.Where(x => x.ProductId == product.Id).FirstOrDefault();
                var oldVendorQuote = await CreateVendorQuote(productQuoteItem, customerQuote.CustomerId);
                CreateVendorQuoteItem(oldVendorQuote.Id, productQuoteItem);
            }
            catch (Exception ex)

            {
                errorFlag = 420;
            }
            return errorFlag;
        }

        public virtual async Task<int> CopyResponseToCustomer(int [] vendorQuoteItemIds)
        {
            int errorFlag = 100;
            try
            {
                var vendorQuoteItem = await _quoteService.GetVendorQuoteItemByIdAsync(vendorQuoteItemIds[0]);
                var vendorQuote = await _quoteService.GetVendorQuoteByIdAsync(vendorQuoteItem.QuoteId);
                //vendorQuote.CustomerQuoteId actually is CustomerQuoteId
                int CustomerQuoteId = vendorQuote.CustomerQuoteId;
                var customerQuote = await _quoteService.GetCustomerQuoteByIdAsync(CustomerQuoteId);
                var quoteItems = await _quoteService.GetCustomerQuoteItemsAsync(CustomerQuoteId);

                foreach (int itemId in vendorQuoteItemIds)
                {
                    var venQuoteItem = await _quoteService.GetVendorQuoteItemByIdAsync(itemId);

                    var quoteItem = quoteItems.Where(x => x.ProductId == venQuoteItem.ProductId).FirstOrDefault();
                    if (quoteItem == null)
                    {
                        CopyNewVendorQuoteToCustomerQuote(venQuoteItem, CustomerQuoteId);
                    }
                    else
                    { 
                        CopyVendorQuoteToCustomerQuote(quoteItem, venQuoteItem);
                    }
                }
                    await UpdateQuoteStatus(customerQuote.Id, (int)OrderStatus.Drafted);
                //customerQuote.Status = (int)OrderStatus.Consolidated;
                //await _quoteService.UpdateCustomerQuoteAsync(customerQuote);
            }
            catch (Exception ex)

            {
                errorFlag = 420;
            }
            return errorFlag;
        }

        public void CopyNewVendorQuoteToCustomerQuote(VendorQuoteItem vendorQuote, int customerQuoteId)
        {
            CustomerQuoteItem customerQuoteItem = new CustomerQuoteItem();
            customerQuoteItem.QuoteId = customerQuoteId;
            customerQuoteItem.ProductId = vendorQuote.ProductId;
            customerQuoteItem.Quantity = vendorQuote.Quantity;
            customerQuoteItem.QuoteItemGuid = Guid.NewGuid();
            customerQuoteItem.PriceExclTax = vendorQuote.PriceExclTax;
            customerQuoteItem.PriceIncTax = vendorQuote.PriceIncTax;
            customerQuoteItem.UnitPriceInclTax = vendorQuote.UnitPriceInclTax;
            customerQuoteItem.UnitPriceExclTax = vendorQuote.UnitPriceExclTax;
            customerQuoteItem.Deleted = false;
            _quoteService.InsertCustomerQuoteItemAsync(customerQuoteItem);
        }

        public void CopyVendorQuoteToCustomerQuote(CustomerQuoteItem customerQuoteItem, VendorQuoteItem vendorQuote)
        {
            customerQuoteItem.Quantity = vendorQuote.Quantity;
            customerQuoteItem.PriceExclTax = vendorQuote.PriceExclTax;
            customerQuoteItem.PriceIncTax = vendorQuote.PriceIncTax;
            customerQuoteItem.UnitPriceInclTax = vendorQuote.UnitPriceInclTax;
            customerQuoteItem.UnitPriceExclTax = vendorQuote.UnitPriceExclTax;
            customerQuoteItem.Deleted = vendorQuote.Deleted;
            _quoteService.UpdateCustomerQuoteItemAsync(customerQuoteItem);
        }
        #endregion

        public Task<List<QuoteAttribueMappingModel>> GetQuoteAttributeMappingModelAsync(int QuoteId)
        {
            return _quoteService.GetQuoteAttributeMappingModelAsync(QuoteId);
        }

        public async void InsertQuoteAttributes(int QuoteId)
        {
            var attributeList = await _quoteService.GetAllQuoteAttrributes();
            foreach (var attribute in attributeList)
            {
                QuoteAttributeMapping mapping = new QuoteAttributeMapping();
                mapping.AttributeId = attribute.Id;
                mapping.QuoteId = QuoteId;
                mapping.Value = "";

                await _quoteService.InsertQuoteAttributeMappingAsync(mapping);
            }
        }

        public async Task<int> AddAttributesToQuote(List<QuoteAttribueMappingModel> attributeList, int QuoteId)
        {
            int errorCode = 100;

            try
            {
                foreach (var attribute in attributeList)
                {
                    if (attribute.Id == 0)
                    {

                        QuoteAttribute attribute1 = await _quoteService.GetQuoteAttributeByNameAsync(attribute.Name.ToUpper());

                        if (attribute1 == null)
                        {
                            attribute1 = new QuoteAttribute();
                            attribute1.Name = attribute.Name.ToUpper();
                            await _quoteService.InsertQuoteAttributeAsync(attribute1);
                        }

                        var oldMapping = await _quoteService.GetQuoteAttributeMappingByAttributAndQuoteIdAsync(attribute1.Id, QuoteId);
                        if (oldMapping == null)
                        {
                            QuoteAttributeMapping mapping = new QuoteAttributeMapping();
                            mapping.AttributeId = attribute1.Id;
                            mapping.Value = attribute.Value;
                            mapping.QuoteId = QuoteId;

                            await _quoteService.InsertQuoteAttributeMappingAsync(mapping);
                        }
                        else
                        {
                            oldMapping.Value = attribute.Value;
                            await _quoteService.UpdateQuoteAttributeMappingAsync(oldMapping);
                        }
                    }
                    else
                    {
                        var quoteAttribute = await _quoteService.GetQuoteAttributeByIdAsync(attribute.AttributeId);

                        if (quoteAttribute.Name.ToUpper() != attribute.Name.ToUpper())
                        {
                            quoteAttribute.Name = attribute.Name.ToUpper();
                            await _quoteService.UpdateQuoteAttributeAsync(quoteAttribute);
                        }

                        var quoteAttributeMapping = await _quoteService.GetQuoteAttributeMappingByIdAsync(attribute.Id);
                        quoteAttributeMapping.Value = attribute.Value;
                        await _quoteService.UpdateQuoteAttributeMappingAsync(quoteAttributeMapping);
                    }
                }
            }
            catch (Exception exc)
            {
                errorCode = 420;
            }

            return errorCode;
        }

        public async Task UpdateQuoteStatus(int QuoteId, int Status)
        {
            var customerQuote = await _quoteService.GetCustomerQuoteByIdAsync(QuoteId);
            if (customerQuote.Status != 60 && (Status == 30 || Status == 40))
            {
                return;
            }
            customerQuote.Status = Status;
            await _quoteService.UpdateCustomerQuoteAsync(customerQuote);
        }

        public async Task DeleteCustomerQuoteItem(int quoteItemId)
        {
            var quoteItem = await _quoteService.GetCustomerQuoteItemByIdAsync(quoteItemId);
            await _quoteService.DeleteCustomerQuoteItemAsync(quoteItem);
            var product = await _productService.GetProductByIdAsync(quoteItem.ProductId);
            var vendorQuoteItem = await _quoteService.GetVendorQuoteItemAsync(product.VendorId, product.Id, quoteItem.QuoteId);
            if(vendorQuoteItem != null)
                await _quoteService.DeleteVendorQuoteItemAsync(vendorQuoteItem);
        }

        public async Task RestoreCustomerQuoteItem(int quoteItemId)
        {
            var quoteItem = await _quoteService.GetCustomerQuoteItemByIdAsync(quoteItemId);
            quoteItem.Deleted = false;
            await _quoteService.UpdateCustomerQuoteItemAsync(quoteItem);
            var product = await _productService.GetProductByIdAsync(quoteItem.ProductId);
            var vendorQuoteItem = await _quoteService.GetVendorQuoteItemAsync(product.VendorId, product.Id, quoteItem.QuoteId);
            if (vendorQuoteItem != null)
            {
                vendorQuoteItem.Deleted = false;
                await _quoteService.UpdateVendorQuoteItemAsync(vendorQuoteItem);
            }
        }

    }
}