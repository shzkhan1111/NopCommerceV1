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
    public partial class VendorQuoteItem : BaseEntity, ISoftDeletedEntity
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
        public string TermsOfSale { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}