using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Quote;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Quote
{
    /// <summary>
    /// Represents a order entity builder
    /// </summary>
    public partial class CustomerQuoteBuilder : NopEntityBuilder<CustomerQuote>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomerQuote.CustomQuoteNumber)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(CustomerQuote.CustomerId)).AsInt32().ForeignKey<Customer>(onDelete: Rule.None)
                .WithColumn(nameof(CustomerQuote.VendorId)).AsInt32().ForeignKey<Vendor>(onDelete: Rule.None);
        }

        #endregion
    }
}