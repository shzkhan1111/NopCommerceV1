using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Quote;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Quote
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class CustomerQuoteItemBuilder : NopEntityBuilder<CustomerQuoteItem>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerQuoteItem.QuoteId)).AsInt32().ForeignKey<Order>()
                .WithColumn(nameof(CustomerQuoteItem.ProductId)).AsInt32().ForeignKey<Product>();
        }

        #endregion
    }
}