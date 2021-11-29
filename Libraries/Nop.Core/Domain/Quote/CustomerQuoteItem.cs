using System;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;

namespace Nop.Core.Domain.Quote
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public partial class CustomerQuoteItem : BaseEntity, ISoftDeletedEntity
    {
        #region Properties

        public Guid QuoteItemGuid { get; set; }
        public int QuoteId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceInclTax { get; set; }
        public decimal UnitPriceExclTax { get; set; }
        public decimal PriceIncTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public bool Deleted { get; set; }
        public string TermsOfSale { get; set; }
        #endregion
    }

    public partial class CustomerQuoteItemModel
    {
        #region Properties

        public Guid QuoteItemGuid { get; set; }
        public int Id { get; set; }
        public int QuoteId { get; set; }
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceInclTax { get; set; }
        public decimal UnitPriceExclTax { get; set; }
        public decimal PriceIncTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public bool Deleted { get; set; }
        public string TermsOfSale { get; set; }
        #endregion
    }

    public partial class ProductModel
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string ProductName { get; set; }
        public string VendorName { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
    }
}