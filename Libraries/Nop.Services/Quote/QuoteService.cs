using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Quote;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Shipping;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class QuoteService : IQuoteService
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerQuote> _quoteRepository;
        private readonly IRepository<CustomerQuoteItem> _quoteItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IShipmentService _shipmentService;
        private readonly IRepository<VendorQuote> _vendorQuoteRepository;
        private readonly IRepository<VendorQuoteItem> _vendorQuoteItemRepository;
        private readonly IRepository<QuoteAttribute> _quoteAttributeRepository;
        private readonly IRepository<QuoteAttributeMapping> _quoteAttributeMappingRepository;

        #endregion

        #region Ctor

        public QuoteService(IProductService productService,
            IRepository<Address> addressRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerQuote> quoteRepository,
            IRepository<CustomerQuoteItem> quoteItemRepository,
            IRepository<Product> productRepository,
            IShipmentService shipmentService,
            IRepository<Vendor> vendorRepository,
            IRepository<VendorQuote> vendorQuoteRepository,
            IRepository<QuoteAttribute> quoteAttributeRepository,
            IRepository<QuoteAttributeMapping> quoteAttributeMappingRepository,
            IRepository<VendorQuoteItem> vendorQuoteItemRepository
            )
        {
            _productService = productService;
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _quoteRepository = quoteRepository;
            _quoteItemRepository = quoteItemRepository;
            _productRepository = productRepository;
            _vendorRepository = vendorRepository;
            _shipmentService = shipmentService;
            _vendorQuoteRepository = vendorQuoteRepository;
            _vendorQuoteItemRepository = vendorQuoteItemRepository;
            _quoteAttributeRepository = quoteAttributeRepository;
            _quoteAttributeMappingRepository = quoteAttributeMappingRepository;
        }

        #endregion


        #region Methods

        #region Customer Quote

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<CustomerQuote> GetCustomerQuoteByIdAsync(int quoteId)
        {
            return await _quoteRepository.GetByIdAsync(quoteId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<Order>.ByIdCacheKey, quoteId));
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<CustomerQuote> GetCustomerQuoteByCustomQuoteNumberAsync(string customQuoteNumber)
        {
            if (string.IsNullOrEmpty(customQuoteNumber))
                return null;

            return await _quoteRepository.Table
                .FirstOrDefaultAsync(o => o.CustomQuoteNumber == customQuoteNumber);
        }

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<CustomerQuote> GetCustomerQuoteByQuoteItemAsync(int quoteItemId)
        {
            if (quoteItemId == 0)
                return null;

            return await (from o in _quoteRepository.Table
                          join oi in _quoteItemRepository.Table on o.Id equals oi.QuoteId
                          where oi.Id == quoteItemId
                          select o).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<IList<CustomerQuote>> GetCustomerQuotesByIdsAsync(int[] quoteIds)
        {
            return await _quoteRepository.GetByIdsAsync(quoteIds, includeDeleted: false);
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        public virtual async Task<CustomerQuote> GetCustomerQuoteByGuidAsync(Guid quoteGuid)
        {
            if (quoteGuid == Guid.Empty)
                return null;

            var query = from o in _quoteRepository.Table
                        where o.QuoteGuid == quoteGuid
                        select o;
            var order = await query.FirstOrDefaultAsync();

            return order;
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerQuoteAsync(CustomerQuote quote)
        {
            await _quoteRepository.DeleteAsync(quote);
        }

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; 0 to load all orders</param>
        /// <param name="productId">Product identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="affiliateId">Affiliate identifier; 0 to load all orders</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier, only orders with products from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="osIds">Order status identifiers; null to load all orders</param>
        /// <param name="psIds">Payment status identifiers; null to load all orders</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all orders</param>
        /// <param name="billingPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the orders
        /// </returns>
        public virtual async Task<IPagedList<CustomerQuote>> SearchCustomerQuoteAsync(int customerId = 0, int vendorId = 0,
            int productId = 0, DateTime? CreatedOnFrom = null, DateTime? CreatedOnTo = null,
            string ShippingMethod = null, string TaxRates = null, string Warranty = null, List<int> osIds = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _quoteRepository.Table;

            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            //if (vendorId > 0)
            //    query = query.Where(o => o.VendorId == vendorId);

            if (vendorId > 0)
                query = from o in query
                        join oi in _quoteItemRepository.Table on o.Id equals oi.QuoteId
                        join prod in _productRepository.Table on oi.ProductId equals prod.Id
                        where prod.VendorId == vendorId
                        select o;

            if (productId > 0)
                query = from o in query
                        join oi in _quoteItemRepository.Table on o.Id equals oi.QuoteId
                        where oi.ProductId == productId
                        select o;
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.Status));

            if (CreatedOnFrom.HasValue)
                query = query.Where(o => CreatedOnFrom.Value <= o.CreatedOn);

            if (CreatedOnTo.HasValue)
                query = query.Where(o => CreatedOnTo.Value >= o.CreatedOn);

            if (!string.IsNullOrEmpty(ShippingMethod))
                query = query.Where(o => o.ShippingMethod == ShippingMethod);

            if (!string.IsNullOrEmpty(TaxRates))
                query = query.Where(o => o.TaxRates == TaxRates);

            if (!string.IsNullOrEmpty(Warranty))
                query = query.Where(o => o.Warranty == Warranty);
            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOn);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerQuoteAsync(CustomerQuote quote)
        {
            await _quoteRepository.InsertAsync(quote);
        }

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerQuoteAsync(CustomerQuote quote)
        {
            await _quoteRepository.UpdateAsync(quote);
        }

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        public virtual SortedDictionary<decimal, decimal> ParseTaxRates(CustomerQuote quote, string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();

            if (string.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            var lines = taxRatesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                var taxes = line.Split(':');
                if (taxes.Length != 2)
                    continue;

                try
                {
                    var taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                    var taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                    taxRatesDictionary.Add(taxRate, taxValue);
                }
                catch
                {
                    // ignored
                }
            }

            //add at least one tax rate (0%)
            if (!taxRatesDictionary.Any())
                taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

            return taxRatesDictionary;
        }

        #endregion

        #region Customer Quote items

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        public virtual async Task<CustomerQuoteItem> GetCustomerQuoteItemByIdAsync(int quoteItemId)
        {
            return await _quoteItemRepository.GetByIdAsync(quoteItemId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<OrderItem>.ByIdCacheKey, quoteItemId));
        }

        /// <summary>
        /// Gets a product of specify order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        public virtual async Task<Product> GetProductByCustomerQuoteItemIdAsync(int quoteItemId)
        {
            if (quoteItemId == 0)
                return null;

            return await (from p in _productRepository.Table
                          join oi in _quoteItemRepository.Table on p.Id equals oi.ProductId
                          where oi.Id == quoteItemId
                          select p).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Gets a list items of order
        /// </summary>
        /// <param name="quoteId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<CustomerQuoteItem>> GetCustomerQuoteItemsAsync(int quoteId)
        {
            if (quoteId == 0)
                return new List<CustomerQuoteItem>();

            return await (from oi in _quoteItemRepository.Table
                          join p in _productRepository.Table on oi.ProductId equals p.Id
                          where
                          oi.QuoteId == quoteId
                          select oi).ToListAsync();
        }

        public virtual async Task<IList<CustomerQuoteItemModel>> GetCustomerQuoteItemsModelAsync(int quoteId)
        {
            if (quoteId == 0)
                return new List<CustomerQuoteItemModel>();

            return await (from oi in _quoteItemRepository.Table
                          join p in _productRepository.Table on oi.ProductId equals p.Id
                          join v in _vendorRepository.Table on p.VendorId equals v.Id
                          where
                          oi.QuoteId == quoteId
                          select new CustomerQuoteItemModel { 
                            Deleted = oi.Deleted,
                            Id = oi.Id,
                            PriceExclTax = oi.PriceExclTax,
                            VendorId = v.Id,
                            PriceIncTax = oi.PriceIncTax,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            QuoteId = oi.QuoteId,
                            QuoteItemGuid = oi.QuoteItemGuid,
                            TermsOfSale = oi.TermsOfSale,
                            UnitPriceExclTax = oi.UnitPriceExclTax,
                            UnitPriceInclTax = oi.UnitPriceInclTax
                          }).OrderBy(x => x.VendorId).ToListAsync();
        }

        /// <summary>
        /// Gets an Customer Quote
        /// </summary>
        /// <param name="quoteItemGuid">Customer Quote identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        public virtual async Task<CustomerQuoteItem> GetCustomerQuoteItemByGuidAsync(Guid quoteItemGuid)
        {
            if (quoteItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _quoteItemRepository.Table
                        where orderItem.QuoteItemGuid == quoteItemGuid
                        select orderItem;
            var item = await query.FirstOrDefaultAsync();
            return item;
        }

        /// <summary>
        /// Gets all downloadable customer quote
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        public virtual async Task<IList<CustomerQuoteItem>> GetDownloadableCustomerQuoteItemsAsync(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            var query = from quoteItem in _quoteItemRepository.Table
                        join o in _quoteRepository.Table on quoteItem.QuoteId equals o.Id
                        join p in _productRepository.Table on quoteItem.ProductId equals p.Id
                        where customerId == o.CustomerId &&
                        p.IsDownload &&
                        !o.Deleted
                        orderby o.CreatedOn descending, quoteItem.Id
                        select quoteItem;

            var quoteItems = await query.ToListAsync();
            return quoteItems;
        }

        /// <summary>
        /// Delete an customer quote
        /// </summary>
        /// <param name="quoteItem">The customer quote</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerQuoteItemAsync(CustomerQuoteItem quoteItem)
        {
            await _quoteItemRepository.DeleteAsync(quoteItem);
        }


        /// <summary>
        /// Inserts a customer quote 
        /// </summary>
        /// <param name="quoteItem">customer quote item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerQuoteItemAsync(CustomerQuoteItem quoteItem)
        {
            quoteItem.Deleted = false;
            await _quoteItemRepository.InsertAsync(quoteItem);
        }

        /// <summary>
        /// Updates a customer quote
        /// </summary>
        /// <param name="quoteItem">quote item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerQuoteItemAsync(CustomerQuoteItem quoteItem)
        {
            await _quoteItemRepository.UpdateAsync(quoteItem);
        }

        #endregion

        #endregion


        #region Vender Quote

        /// <summary>
        /// Get vendor quote by vendor quote id
        /// </summary>
        /// <param name="vendorQuoteId"></param>
        /// <returns></returns>
        public virtual async Task<VendorQuote> GetVendorQuoteByIdAsync(int vendorQuoteId)
        {
            return await _vendorQuoteRepository.GetByIdAsync(vendorQuoteId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<Order>.ByIdCacheKey, vendorQuoteId));
        }

        public virtual async Task<VendorQuote> GetVendorQuoteByQuoteIdAsync(int QuoteId)
        {
            return await _vendorQuoteRepository.Table
               .FirstOrDefaultAsync(o => o.CustomerQuoteId == QuoteId);
        }

        public virtual async Task<VendorQuote> GetVendorQuoteByQuoteIdAndVendorAsync(int QuoteId, int vendorId)
        {
            return await _vendorQuoteRepository.Table
               .FirstOrDefaultAsync(o => o.CustomerQuoteId == QuoteId && o.VendorId == vendorId);
        }

        /// <summary>
        /// Get vendor quote by custom Quote Number
        /// </summary>
        /// <param name="customQuoteNumber"></param>
        /// <returns></returns>
        public virtual async Task<VendorQuote> GetVendorQuoteByCustomQuoteNumberAsync(string customQuoteNumber)
        {
            if (string.IsNullOrEmpty(customQuoteNumber))
                return null;

            return await _vendorQuoteRepository.Table
                .FirstOrDefaultAsync(o => o.CustomQuoteNumber == customQuoteNumber);
        }

        /// <summary>
        /// Get vendor quote by vendorQuoteItemId
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        public virtual async Task<VendorQuote> GetVendorQuoteByQuoteItemAsync(int vendorQuoteItemId)
        {
            if (vendorQuoteItemId == 0)
                return null;

            return await (from o in _vendorQuoteRepository.Table
                          join oi in _vendorQuoteItemRepository.Table on o.Id equals oi.QuoteId
                          where oi.Id == vendorQuoteItemId
                          select o).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get Vendor quotes by list of int vendorQuoteIds
        /// </summary>
        /// <param name="vendorQuoteIds"></param>
        /// <returns></returns>
        public virtual async Task<IList<VendorQuote>> GetVendorQuotesByIdsAsync(int[] vendorQuoteIds)
        {
            return await _vendorQuoteRepository.GetByIdsAsync(vendorQuoteIds, includeDeleted: false);
        }

        /// <summary>
        /// Get vendor quote by quote guid
        /// </summary>
        /// <param name="quoteGuid"></param>
        /// <returns></returns>
        public virtual async Task<VendorQuote> GetVendorQuoteByGuidAsync(Guid quoteGuid)
        {
            if (quoteGuid == Guid.Empty)
                return null;

            var query = from o in _vendorQuoteRepository.Table
                        where o.QuoteGuid == quoteGuid
                        select o;
            var order = await query.FirstOrDefaultAsync();

            return order;
        }

        /// <summary>
        /// Delete vendor qoute
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public virtual async Task DeleteVendorQuoteAsync(VendorQuote quote)
        {
            await _vendorQuoteRepository.DeleteAsync(quote);
        }

        /// <summary>
        /// Search vendor quote by mutiple parameters
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="vendorId"></param>
        /// <param name="productId"></param>
        /// <param name="CreatedOnFrom"></param>
        /// <param name="CreatedOnTo"></param>
        /// <param name="ShippingMethod"></param>
        /// <param name="TaxRates"></param>
        /// <param name="Warranty"></param>
        /// <param name="osIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="getOnlyTotalCount"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<VendorQuote>> SearchVendorQuoteAsync(int customerId = 0, int vendorId = 0,
            int productId = 0, DateTime? CreatedOnFrom = null, DateTime? CreatedOnTo = null,
            string ShippingMethod = null, string TaxRates = null, string Warranty = null, List<int> osIds = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _vendorQuoteRepository.Table;

            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (vendorId > 0)
                query = query.Where(o => o.VendorId == vendorId);

            if (productId > 0)
                query = from o in query
                        join oi in _quoteItemRepository.Table on o.Id equals oi.QuoteId
                        where oi.ProductId == productId
                        select o;
            //if (osIds != null && osIds.Any())
            //    query = query.Where(o => osIds.Contains(o.Status));

            if (CreatedOnFrom.HasValue)
                query = query.Where(o => CreatedOnFrom.Value <= o.CreatedOn);

            if (CreatedOnTo.HasValue)
                query = query.Where(o => CreatedOnTo.Value >= o.CreatedOn);

            if (!string.IsNullOrEmpty(ShippingMethod))
                query = query.Where(o => o.ShippingMethod == ShippingMethod);

            if (!string.IsNullOrEmpty(TaxRates))
                query = query.Where(o => o.TaxRates == TaxRates);

            if (!string.IsNullOrEmpty(Warranty))
                query = query.Where(o => o.Warranty == Warranty);
            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOn);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Add new vendor quote
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public virtual async Task InsertVendorQuoteAsync(VendorQuote quote)
        {
            await _vendorQuoteRepository.InsertAsync(quote);
        }

        /// <summary>
        /// Update existing Vendor Quote
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public virtual async Task UpdateVendorQuoteAsync(VendorQuote quote)
        {
            await _vendorQuoteRepository.UpdateAsync(quote);
        }

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        public virtual SortedDictionary<decimal, decimal> ParseVendorTaxRates(VendorQuote quote, string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();

            if (string.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            var lines = taxRatesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;

                var taxes = line.Split(':');
                if (taxes.Length != 2)
                    continue;

                try
                {
                    var taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                    var taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                    taxRatesDictionary.Add(taxRate, taxValue);
                }
                catch
                {
                    // ignored
                }
            }

            //add at least one tax rate (0%)
            if (!taxRatesDictionary.Any())
                taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

            return taxRatesDictionary;
        }

        #endregion

        #region Vendor Quote items

        /// <summary>
        /// Get vendor quote item by vendor quote item id
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        public virtual async Task<VendorQuoteItem> GetVendorQuoteItemByIdAsync(int vendorQuoteItemId)
        {
            return await _vendorQuoteItemRepository.GetByIdAsync(vendorQuoteItemId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<OrderItem>.ByIdCacheKey, vendorQuoteItemId));
        }

        public virtual async Task<VendorQuoteItem> GetVendorQuoteItemAsync(int vendorId, int productId, int customerQuoteId)
        {
            var query = from quoteItem in _vendorQuoteItemRepository.Table
                        join quote in _vendorQuoteRepository.Table on quoteItem.QuoteId equals quote.Id
                        where quote.CustomerQuoteId == customerQuoteId && quote.VendorId == vendorId
                        && quoteItem.ProductId == productId
                        select quoteItem;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Get product by vendor quote id
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        public virtual async Task<Product> GetProductByVendorQuoteItemIdAsync(int vendorQuoteItemId)
        {
            if (vendorQuoteItemId == 0)
                return null;

            return await (from p in _productRepository.Table
                          join oi in _vendorQuoteItemRepository.Table on p.Id equals oi.ProductId
                          where oi.Id == vendorQuoteItemId
                          select p).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get Vendor Quote
        /// </summary>
        /// <param name="vendorQuoteId"></param>
        /// <returns></returns>
        public virtual async Task<IList<VendorQuoteItem>> GetVendorQuoteItemsAsync(int vendorQuoteId)
        {
            if (vendorQuoteId == 0)
                return new List<VendorQuoteItem>();

            return await (from oi in _vendorQuoteItemRepository.Table
                          join p in _productRepository.Table on oi.ProductId equals p.Id
                          where
                          oi.QuoteId == vendorQuoteId
                          select oi).ToListAsync();
        }

        /// <summary>
        /// Get Vendor Quote Item By Guid
        /// </summary>
        /// <param name="quoteItemGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        public virtual async Task<VendorQuoteItem> GetVendorQuoteItemByGuidAsync(Guid vendorQuoteItemGuid)
        {
            if (vendorQuoteItemGuid == Guid.Empty)
                return null;

            var query = from orderItem in _vendorQuoteItemRepository.Table
                        where orderItem.QuoteItemGuid == vendorQuoteItemGuid
                        select orderItem;
            var item = await query.FirstOrDefaultAsync();
            return item;
        }

        /// <summary>
        /// Get Downloadable Vendor Quote Items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        public virtual async Task<IList<VendorQuoteItem>> GetDownloadableVendorQuoteItemsAsync(int customerId)
        {
            if (customerId == 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            var query = from quoteItem in _vendorQuoteItemRepository.Table
                        join o in _vendorQuoteRepository.Table on quoteItem.QuoteId equals o.Id
                        join p in _productRepository.Table on quoteItem.ProductId equals p.Id
                        where customerId == o.CustomerId &&
                        p.IsDownload &&
                        !o.Deleted
                        orderby o.CreatedOn descending, quoteItem.Id
                        select quoteItem;

            var quoteItems = await query.ToListAsync();
            return quoteItems;
        }

        /// <summary>
        /// Delete Vendor Quote Item
        /// </summary>
        /// <param name="vendorQuoteItem">The vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteVendorQuoteItemAsync(VendorQuoteItem vendorQuoteItem)
        {
            await _vendorQuoteItemRepository.DeleteAsync(vendorQuoteItem);
        }


        /// <summary>
        /// Inserts a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertVendorQuoteItemAsync(VendorQuoteItem vendorQuoteItem)
        {
            await _vendorQuoteItemRepository.InsertAsync(vendorQuoteItem);
        }

        /// <summary>
        /// Updates a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateVendorQuoteItemAsync(VendorQuoteItem vendorQuoteItem)
        {
            await _vendorQuoteItemRepository.UpdateAsync(vendorQuoteItem);
        }

        public virtual async Task<VendorQuote> GetVendorQuoteByCustomerQuoteIdAsync(int customerQuoteId)
        {
            var query = from orderItem in _vendorQuoteRepository.Table
                        where orderItem.CustomerQuoteId == customerQuoteId
                        select orderItem;
            var item = await query.FirstOrDefaultAsync();
            return item;
        }

        public virtual async Task<VendorQuoteItem> GetVendorQuoteItemByQuoteAndProductIdAsync(int quoteId, int productId)
        {
            var query = from orderItem in _vendorQuoteItemRepository.Table
                        where orderItem.QuoteId == quoteId && orderItem.ProductId == productId
                        select orderItem;
            var item = await query.FirstOrDefaultAsync();
            return item;
        }


        #endregion


        #region QuoteAttribute

        /// <summary>
        /// Get vendor quote item by vendor quote item id
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        public virtual async Task<QuoteAttribute> GetQuoteAttributeByIdAsync(int attributeId)
        {
            return await _quoteAttributeRepository.GetByIdAsync(attributeId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<QuoteAttribute>.ByIdCacheKey, attributeId));
        }

        public virtual async Task<QuoteAttribute> GetQuoteAttributeByNameAsync(string attributeName)
        {
            return await _quoteAttributeRepository.Table.Where(x => x.Name == attributeName).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Inserts a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertQuoteAttributeAsync(QuoteAttribute quoteAttribute)
        {
            await _quoteAttributeRepository.InsertAsync(quoteAttribute);
        }

        /// <summary>
        /// Updates a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateQuoteAttributeAsync(QuoteAttribute quoteAttribute)
        {
            await _quoteAttributeRepository.UpdateAsync(quoteAttribute);
        }

        public virtual async Task<List<QuoteAttribute>> GetAllQuoteAttrributes()
        {
            return await (from qa in _quoteAttributeRepository.Table select qa).ToListAsync();
        }

        #endregion


        #region QuoteAttribute Mappng

        /// <summary>
        /// Get vendor quote item by vendor quote item id
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        public virtual async Task<QuoteAttributeMapping> GetQuoteAttributeMappingByIdAsync(int attributeMappingId)
        {
            return await _quoteAttributeMappingRepository.GetByIdAsync(attributeMappingId,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<QuoteAttributeMapping>.ByIdCacheKey, attributeMappingId));
        }

        public virtual async Task<QuoteAttributeMapping> GetQuoteAttributeMappingByAttributAndQuoteIdAsync(int attributeId, int quoteId)
        {
            return await _quoteAttributeMappingRepository.Table.Where(x => x.AttributeId == attributeId && x.QuoteId == quoteId).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Inserts a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertQuoteAttributeMappingAsync(QuoteAttributeMapping quoteAttributeMapping)
        {
            await _quoteAttributeMappingRepository.InsertAsync(quoteAttributeMapping);
        }

        /// <summary>
        /// Updates a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateQuoteAttributeMappingAsync(QuoteAttributeMapping quoteAttributeMapping)
        {
            await _quoteAttributeMappingRepository.UpdateAsync(quoteAttributeMapping);
        }

        public virtual async Task<List<QuoteAttribueMappingModel>> GetQuoteAttributeMappingModelAsync(int quoteId)
        {

            return await (from qam in _quoteAttributeMappingRepository.Table
                          join qa in _quoteAttributeRepository.Table on qam.AttributeId equals qa.Id
                          where
                          qam.QuoteId == quoteId
                          select new QuoteAttribueMappingModel
                          {
                              Id = qam.Id,
                              AttributeId = qam.AttributeId,
                              Name = qa.Name,
                              QuoteId = qam.QuoteId,
                              Value = qam.Value
                          }).OrderBy(x => x.Id).ToListAsync();
        }
        #endregion
    }
}