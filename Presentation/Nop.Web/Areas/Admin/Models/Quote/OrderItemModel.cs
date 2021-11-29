using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order item model
    /// </summary>
    public partial record QuoteItemModel : BaseNopEntityModel
    {
        #region Ctor

        public QuoteItemModel()
        {
            PurchasedGiftCardIds = new List<int>();
        }

        #endregion

        #region Properties

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string VendorName { get; set; }

        public string Sku { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public string UnitPriceInclTax { get; set; }

        public string UnitPriceExclTax { get; set; }

        public decimal PriceInclTax { get; set; }

        public decimal PriceExclTax { get; set; }

        public int Quantity { get; set; }

        public string TermOfSale { get; set; }


        public IList<int> PurchasedGiftCardIds { get; set; }

        #endregion


    }
}