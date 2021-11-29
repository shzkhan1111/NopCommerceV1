using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Quote;
using Nop.Core.Domain.Shipping;
using Nop.Web.Models.Quote;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the order model factory
    /// </summary>
    public partial interface IQuoteModelFactory
    {
        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer order list model
        /// </returns>
        Task<CustomerQuoteListModel> PrepareCustomerQuoteListModelAsync();
        Task<CustomerQuoteListModel> PrepareCustomerQuoteDetailListModelAsync(int QuoteId);
        Task<CustomerQuoteListModel> PrepareVendorQuoteListModelAsync();
        Task<CustomerQuoteListModel> PrepareVendorQuoteDetailListModelAsync(int VendorQuoteId);
        Task<int> CreateVendorQuotes(int[] QuoteItemIds, int QuoteId);
        Task<int> AddProductToVendorQuote(string sku, int customerQuoteId);
        Task<int> AddProductInVendorQuote(string QuoteGuid, int ProductId, int VendorId);
        Task<int> AddProductInCustomerQuote(string QuoteGuid, int ProductId);

        Task<int> CopyResponseToCustomer(int[] vendorQuoteItemIds);
        Task<List<QuoteAttribueMappingModel>> GetQuoteAttributeMappingModelAsync(int QuoteId);
        void InsertQuoteAttributes(int QuoteId);
        Task<int> AddAttributesToQuote(List<QuoteAttribueMappingModel> attributeList, int QuoteId);
        Task UpdateQuoteStatus(int QuoteId, int Status);
        Task DeleteCustomerQuoteItem(int quoteItemId);
        Task RestoreCustomerQuoteItem(int quoteItemId);
    }
}
