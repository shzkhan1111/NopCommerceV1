using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Quote;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service interface
    /// </summary>
    public partial interface IQuoteService
    {
        #region Customer Quotes

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<CustomerQuote> GetCustomerQuoteByIdAsync(int quoteId);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<CustomerQuote> GetCustomerQuoteByCustomQuoteNumberAsync(string customQuoteNumber);

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<CustomerQuote> GetCustomerQuoteByQuoteItemAsync(int quoteItemId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<IList<CustomerQuote>> GetCustomerQuotesByIdsAsync(int[] quoteIds);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<CustomerQuote> GetCustomerQuoteByGuidAsync(Guid quoteGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomerQuoteAsync(CustomerQuote quote);

        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="storeId">Store identifier; null to load all orders</param>
        /// <param name="vendorId">Vendor identifier; null to load all orders</param>
        /// <param name="customerId">Customer identifier; null to load all orders</param>
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
        Task<IPagedList<CustomerQuote>> SearchCustomerQuoteAsync(int customerId = 0, int vendorId = 0,
            int productId = 0, DateTime? CreatedOnFrom = null, DateTime? CreatedOnTo = null,
            string ShippingMethod = null, string TaxRates = null, string Warranty = null, List<int> osIds = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerQuoteAsync(CustomerQuote quote);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerQuoteAsync(CustomerQuote quote);

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        SortedDictionary<decimal, decimal> ParseTaxRates(CustomerQuote quote, string taxRatesStr);

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
        Task<CustomerQuoteItem> GetCustomerQuoteItemByIdAsync(int quoteItemId);

        /// <summary>
        /// Gets a product of specify order item
        /// </summary>
        /// <param name="orderItemId">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product
        /// </returns>
        Task<Product> GetProductByCustomerQuoteItemIdAsync(int quoteItemId);

        /// <summary>
        /// Gets a list items of order
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="isNotReturnable">Value indicating whether this product is returnable; pass null to ignore</param>
        /// <param name="isShipEnabled">Value indicating whether the entity is ship enabled; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<IList<CustomerQuoteItem>> GetCustomerQuoteItemsAsync(int quoteId);
        Task<IList<CustomerQuoteItemModel>> GetCustomerQuoteItemsModelAsync(int quoteId);

        /// <summary>
        /// Gets an order item
        /// </summary>
        /// <param name="orderItemGuid">Order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        Task<CustomerQuoteItem> GetCustomerQuoteItemByGuidAsync(Guid quoteItemGuid);

        /// <summary>
        /// Gets all downloadable order items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        Task<IList<CustomerQuoteItem>> GetDownloadableCustomerQuoteItemsAsync(int customerId);

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="orderItem">The order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCustomerQuoteItemAsync(CustomerQuoteItem quoteItem);
        Task<VendorQuoteItem> GetVendorQuoteItemAsync(int vendorId, int productId, int customerQuoteId);

        /// <summary>
        /// Inserts a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCustomerQuoteItemAsync(CustomerQuoteItem quoteItem);

        /// <summary>
        /// Updates a order item
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCustomerQuoteItemAsync(CustomerQuoteItem quoteItem);

        #endregion




        #region Vendor Quotes

       
        Task<VendorQuote> GetVendorQuoteByIdAsync(int vendorQuoteId);

        Task<VendorQuote> GetVendorQuoteByQuoteIdAsync(int QuoteId);
        Task<VendorQuote> GetVendorQuoteByQuoteIdAndVendorAsync(int QuoteId, int vendorId);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<VendorQuote> GetVendorQuoteByCustomQuoteNumberAsync(string customQuoteNumber);

        /// <summary>
        /// Gets an order by order item identifier
        /// </summary>
        /// <param name="orderItemId">The order item identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<VendorQuote> GetVendorQuoteByQuoteItemAsync(int vendorQuoteItemId);

        /// <summary>
        /// Get orders by identifiers
        /// </summary>
        /// <param name="orderIds">Order identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<IList<VendorQuote>> GetVendorQuotesByIdsAsync(int[] vendorQuoteIds);

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderGuid">The order identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order
        /// </returns>
        Task<VendorQuote> GetVendorQuoteByGuidAsync(Guid quoteGuid);

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteVendorQuoteAsync(VendorQuote quote);

        Task<IPagedList<VendorQuote>> SearchVendorQuoteAsync(int customerId = 0, int vendorId = 0,
            int productId = 0, DateTime? CreatedOnFrom = null, DateTime? CreatedOnTo = null,
            string ShippingMethod = null, string TaxRates = null, string Warranty = null, List<int> osIds = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Inserts an order
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertVendorQuoteAsync(VendorQuote quote);

        /// <summary>
        /// Updates the order
        /// </summary>
        /// <param name="order">The order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateVendorQuoteAsync(VendorQuote quote);

        /// <summary>
        /// Parse tax rates
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="taxRatesStr"></param>
        /// <returns>Rates</returns>
        SortedDictionary<decimal, decimal> ParseVendorTaxRates(VendorQuote quote, string taxRatesStr);

        #endregion

        #region Vendor Quote items

        /// <summary>
        /// Get vendor quote item by vendor quote item id
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        Task<VendorQuoteItem> GetVendorQuoteItemByIdAsync(int vendorQuoteItemId);

        /// <summary>
        /// Get product by vendor quote id
        /// </summary>
        /// <param name="vendorQuoteItemId"></param>
        /// <returns></returns>
        Task<Product> GetProductByVendorQuoteItemIdAsync(int vendorQuoteItemId);

        /// <summary>
        /// Get Vendor Quote
        /// </summary>
        /// <param name="vendorQuoteId"></param>
        /// <returns></returns>
        Task<IList<VendorQuoteItem>> GetVendorQuoteItemsAsync(int vendorQuoteId);

        /// <summary>
        /// Get Vendor Quote Item By Guid
        /// </summary>
        /// <param name="quoteItemGuid"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order item
        /// </returns>
        Task<VendorQuoteItem> GetVendorQuoteItemByGuidAsync(Guid vendorQuoteItemGuid);

        /// <summary>
        /// Get Downloadable Vendor Quote Items
        /// </summary>
        /// <param name="customerId">Customer identifier; null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order items
        /// </returns>
        Task<IList<VendorQuoteItem>> GetDownloadableVendorQuoteItemsAsync(int customerId);

        /// <summary>
        /// Delete Vendor Quote Item
        /// </summary>
        /// <param name="vendorQuoteItem">The vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteVendorQuoteItemAsync(VendorQuoteItem quoteItem);


        /// <summary>
        /// Inserts a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertVendorQuoteItemAsync(VendorQuoteItem quoteItem);

        /// <summary>
        /// Updates a vendor item
        /// </summary>
        /// <param name="vendorQuoteItem">vendor item</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateVendorQuoteItemAsync(VendorQuoteItem quoteItem);

        Task<VendorQuote> GetVendorQuoteByCustomerQuoteIdAsync(int customerQuoteId);
        Task<VendorQuoteItem> GetVendorQuoteItemByQuoteAndProductIdAsync(int quoteId, int productId);
        #endregion

        Task<QuoteAttribute> GetQuoteAttributeByIdAsync(int attributeId);
        Task<QuoteAttribute> GetQuoteAttributeByNameAsync(string attributeName);

        Task InsertQuoteAttributeAsync(QuoteAttribute quoteAttribute);

        Task UpdateQuoteAttributeAsync(QuoteAttribute quoteAttribute);
        Task<List<QuoteAttribute>> GetAllQuoteAttrributes();
        Task<QuoteAttributeMapping> GetQuoteAttributeMappingByIdAsync(int attributeMappingId);
        Task<QuoteAttributeMapping> GetQuoteAttributeMappingByAttributAndQuoteIdAsync(int attributeId, int quoteId);
        Task InsertQuoteAttributeMappingAsync(QuoteAttributeMapping quoteAttributeMapping);
        Task UpdateQuoteAttributeMappingAsync(QuoteAttributeMapping quoteAttributeMapping);
        Task<List<QuoteAttribueMappingModel>> GetQuoteAttributeMappingModelAsync(int quoteId);

    }
}