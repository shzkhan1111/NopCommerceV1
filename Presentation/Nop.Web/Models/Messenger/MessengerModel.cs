using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Quote
{
    public partial record MessengerModel : BaseNopModel
    {
        public MessengerModel(int quoteId)
        {
            QuoteId = quoteId;
        }
        public MessengerModel()
        {
        }

        public string CustomerQuoteGuid { get; set; }
        public string Guid { get; set; }
        public int QuoteId { get; set; }
        public int CustomQuoteId { get; set; }
        public int VendorQuoteId { get; set; }
        public int CustomerId { get; set; }
        public int VendorId { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime QuoteDate { get; set; }
    }
}