using Nop.Web.Models.Common;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial interface IFileManager
    {
        /// <summary>
        /// Prepare the File Upload Model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the File Upload Model
        /// </returns>
        Task<FileUploadModel> PrepareFileUploadModelAsync(string extensions, int sourceTypeId, int sourceId);

        /// <summary>
        /// Prepare the File Lister Model
        /// </summary>
        /// A task that represents the asynchronous operation
        /// The task result contains the File Lister Model
        /// <returns></returns>
        Task<FileListerModel> PrepareFileListerModelAsync(int sourceTypeId, int sourceId);
        void SendFilesToCustomers(int[] fileIds, int[] customerIds, int quoteId);
        void SendFilesToVendors(int[] fileIds, int[] vendorIds, int quoteId);
    }
}
