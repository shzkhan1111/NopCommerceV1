using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Quote;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Quote
{
    public partial record CustomerQuoteListModel : BaseNopModel
    {
        public CustomerQuoteListModel()
        {
            Quotes = new List<QuoteDetailsModel>();
        }
        public int CustomerId { set; get; }
        public int VendorId { set; get; }
        public bool IsAdmin { set; get; }

        public IList<QuoteDetailsModel> Quotes { get; set; }

        #region Nested classes

        public partial record QuoteDetailsModel : BaseNopEntityModel
        {
            public QuoteDetailsModel()
            {
                QuoteItems = new List<QuoteItemDetailsModel>();
                QuoteAttributeList = new List<QuoteAttribueMappingModel>();
            }

            public string CustomQuoteNumber { get; set; }
            public string CustomerName { get; set; }
            public string CompanyName { get; set; }
            public string QuoteGuid { get; set; }
            public string QuoteTotal { get; set; }
            public OrderStatus QuoteStatusEnum { get; set; }
            public string QuoteStatus { get; set; }
            public DateTime CreatedOn { get; set; }
            public IList<QuoteItemDetailsModel> QuoteItems { get; set; }
            public IList<QuoteAttribueMappingModel> QuoteAttributeList  { get; set; }
        }

        public partial record QuoteItemDetailsModel : BaseNopEntityModel
        {
            public Guid QuoteItemGuid { get; set; }
            public int QuoteId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string SKU { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPriceInclTax { get; set; }
            public decimal UnitPriceExclTax { get; set; }
            public decimal PriceIncTax { get; set; }
            public decimal PriceExclTax { get; set; }
            public string TermsOfSale { get; set; }
            public bool Deleted { get; set; }
        }

        #endregion
    }
}