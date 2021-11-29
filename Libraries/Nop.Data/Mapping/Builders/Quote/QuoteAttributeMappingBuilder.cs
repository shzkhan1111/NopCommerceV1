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
    public partial class QuoteAttributeMappingBuilder : NopEntityBuilder<QuoteAttributeMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(QuoteAttributeMapping.Value)).AsString(400).NotNullable()
               .WithColumn(nameof(QuoteAttributeMapping.AttributeId)).AsInt32().NotNullable()
               .WithColumn(nameof(QuoteAttributeMapping.QuoteId)).AsInt32().NotNullable();
        }

        #endregion
    }
}