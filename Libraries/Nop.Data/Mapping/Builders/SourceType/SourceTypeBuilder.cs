using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.SourceType;

namespace Nop.Data.Mapping.Builders.SourceType
{    /// <summary>
     /// Represents a download entity builder
     /// </summary>
    public partial class SourceTypeBuilder : NopEntityBuilder<SourceTypes>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}
