using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.SourceType;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Order service interface
    /// </summary>
    public partial interface ISourceTypeService
    {
        Task<SourceTypes> GetSourceTypeByIdAsync(int id);
        Task<IList<SourceTypes>> GetSourceTypesByIdsAsync(int[] ids);
        Task DeleteSourceTypeAsync(SourceTypes sourceTypes);
        Task InsertVendorQuoteAsync(SourceTypes sourceTypes);
        Task UpdateVendorQuoteAsync(SourceTypes sourceTypes);
    }
}