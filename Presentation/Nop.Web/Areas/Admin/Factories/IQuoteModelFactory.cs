using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial interface IQuoteModelFactory
    {
        /// <summary>
        /// Prepare order search model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order search model
        /// </returns>
        Task<QuoteSearchModel> PrepareQuoteSearchModelAsync(QuoteSearchModel searchModel);

        /// <summary>
        /// Prepare paged order list model
        /// </summary>
        /// <param name="searchModel">Order search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order list model
        /// </returns>
        Task<QuoteListModel> PrepareQuoteListModelAsync(QuoteSearchModel searchModel);

    }
}