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
    public partial class VendorQuote : BaseEntity, ISoftDeletedEntity
    {
        #region Properties

        public Guid QuoteGuid { get; set; }
        public int CustomerId { get; set; }
        public int CustomerQuoteId { get; set; }
        public int VendorId { get; set; }
        public string CustomQuoteNumber { get; set; }
        public DateTime QuoteDate { get; set; }
        public string ShippingMethod { get; set; }
        public string TaxRates { get; set; }
        public decimal OrderShippingInclTax { get; set; }
        public decimal OrderShippingExclTax { get; set; }
        public string Warranty { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOn { get; set; }
        

        #endregion
    }
}