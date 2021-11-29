using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Tax;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order model
    /// </summary>
    public partial record QuoteModel : BaseNopEntityModel
    {
        #region Ctor

        public QuoteModel()
        {
            CustomValues = new Dictionary<string, object>();
            TaxRates = new List<TaxRate>();
            Items = new List<QuoteItemModel>();
        }

        #endregion

        #region Properties

        public bool IsLoggedInAsVendor { get; set; }

        //identifiers
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.Quote.Fields.QuoteGuid")]
        public Guid QuoteGuid { get; set; }
        [NopResourceDisplayName("Admin.Quote.Fields.CustomQuoteNumber")]
        public string CustomQuoteNumber { get; set; }

        [NopResourceDisplayName("Admin.Quote.Fields.QuoteDate")]
        public string QuoteDate { get; set; }


        [NopResourceDisplayName("Admin.Quote.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }


        [NopResourceDisplayName("Admin.Quote.Fields.OrderShippingInclTax")]
        public string OrderShippingInclTax { get; set; }

        [NopResourceDisplayName("Admin.Quote.Fields.OrderShippingExclTax")]
        public string OrderShippingExclTax { get; set; }

        [NopResourceDisplayName("Admin.Quote.Fields.Warranty")]
        public string Warranty { get; set; }


        //customer info
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public string CustomerInfo { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }
        public string CustomerFullName { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerIP")]
        public string CustomerIp { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.CustomValues")]
        public Dictionary<string, object> CustomValues { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.Tax")]
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        //order status
        [NopResourceDisplayName("Admin.Quote.Fields.QuoteStatus")]
        public string QuoteStatus { get; set; }
        [NopResourceDisplayName("Admin.Quote.Fields.QuoteStatusId")]
        public int QuoteStatusId { get; set; }

        //items
        public IList<QuoteItemModel> Items { get; set; }

        //creation date
        [NopResourceDisplayName("Admin.Orders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }


        #endregion

        #region Nested Classes

        public partial record TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }
        #endregion
    }

    public partial record QuoteAggreratorModel : BaseNopModel
    {
        //aggergator properties
        public string aggregatorprofit { get; set; }
        public string aggregatorshipping { get; set; }
        public string aggregatortax { get; set; }
        public string aggregatortotal { get; set; }
    }
}