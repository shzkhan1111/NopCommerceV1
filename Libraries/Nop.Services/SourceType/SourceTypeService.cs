using Nop.Core.Caching;
using Nop.Core.Domain.SourceType;
using Nop.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Services.Orders
{
    /// <summary>
    /// SourceTypes service
    /// </summary>
    public partial class SourceTypeService : ISourceTypeService
    {
        #region Fields

        private readonly IRepository<SourceTypes> _sourceTypeRepository;

        #endregion

        #region Ctor

        public SourceTypeService(
            IRepository<SourceTypes> sourceTypeRepository
            )
        {
            _sourceTypeRepository = sourceTypeRepository;
        }

        #endregion

        #region Source Type

        /// <summary>
        /// Get source type by source type id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<SourceTypes> GetSourceTypeByIdAsync(int id)
        {
            return await _sourceTypeRepository.GetByIdAsync(id,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<SourceTypes>.ByIdCacheKey, id));
        }

        /// <summary>
        /// Get source types by list of int vendorQuoteIds
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<IList<SourceTypes>> GetSourceTypesByIdsAsync(int[] ids)
        {
            return await _sourceTypeRepository.GetByIdsAsync(ids, includeDeleted: false);
        }

        /// <summary>
        /// Delete source type
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public virtual async Task DeleteSourceTypeAsync(SourceTypes sourceTypes)
        {
            await _sourceTypeRepository.DeleteAsync(sourceTypes);
        }

        /// <summary>
        /// Add new source type
        /// </summary>
        /// <param name="sourceTypes"></param>
        /// <returns></returns>
        public virtual async Task InsertVendorQuoteAsync(SourceTypes sourceTypes)
        {
            await _sourceTypeRepository.InsertAsync(sourceTypes);
        }

        /// <summary>
        /// Update existing source type
        /// </summary>
        /// <param name="sourceTypes"></param>
        /// <returns></returns>
        public virtual async Task UpdateVendorQuoteAsync(SourceTypes sourceTypes)
        {
            await _sourceTypeRepository.UpdateAsync(sourceTypes);
        }

        #endregion


    }
}