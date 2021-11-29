using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Quote;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Factories
{
    public partial class FileManager : IFileManager
    {
        private readonly IDownloadService _downloadService;
        private readonly IQuoteService _quoteService;
        private readonly IWorkContext _workContext;
        public FileManager(IDownloadService downloadService, IQuoteService quoteService, IWorkContext workContext)
        {
            _downloadService = downloadService;
            _quoteService = quoteService;
            _workContext = workContext;
        }
        /// <summary>
        /// Prepare the File Upload Model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the File Upload Model
        /// </returns>
        public virtual async Task<FileUploadModel> PrepareFileUploadModelAsync(string extensions, int sourceTypeId, int sourceId)
        {
            var allowedFileExtensions = extensions.Split(',').ToList();

            return new FileUploadModel(allowedFileExtensions, sourceTypeId, sourceId);
        }

        /// <summary>
        /// Prepare the file lister model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the file lister model
        /// </returns>
        public virtual async Task<FileListerModel> PrepareFileListerModelAsync(int sourceTypeId, int sourceId)
        {
            var listerModel = new FileListerModel(sourceTypeId, sourceId);

            listerModel.DownloadList = await _downloadService.GetDownloadBySourceTypeAndSourceIdAsync(listerModel.SourceTypeId ?? 0, listerModel.SourceId ?? 0);
            return listerModel;
        }

        public virtual async void SendFilesToVendors(int [] fileIds, int [] vendorIds, int quoteId)
        {
            foreach (int vendorId in vendorIds)
            {
                VendorQuote quote = await _quoteService.GetVendorQuoteByQuoteIdAndVendorAsync(quoteId, vendorId);

                foreach (int fileId in fileIds)
                {
                    Download download = await _downloadService.GetDownloadByIdAsync(fileId);
                    insertNewDownloadFile(download, 3, quote.Id);

                }
            }

        }


        public virtual async void SendFilesToCustomers(int[] fileIds, int[] customerIds, int quoteId)
        {

            VendorQuote quote = await _quoteService.GetVendorQuoteByIdAsync(quoteId);
            foreach (int fileId in fileIds)
            {
                Download download = await _downloadService.GetDownloadByIdAsync(fileId);
                insertNewDownloadFile(download, 1, quote.CustomerQuoteId);
            }
        }

        async void insertNewDownloadFile(Download download, int sourceTypeId, int sourceId)
        {
            int userId = (await _workContext.GetCurrentCustomerAsync()).Id;
            Download newDownload = new Download();
            newDownload.ContentType = download.ContentType;
            newDownload.CreatedBy = download.CreatedBy;
            newDownload.CreatedOn = DateTime.Now;
            newDownload.DownloadBinary = download.DownloadBinary;
            newDownload.DownloadGuid = Guid.NewGuid();
            newDownload.DownloadUrl = download.DownloadUrl;
            newDownload.Extension = download.Extension;
            newDownload.Filename = download.Filename;
            newDownload.IsNew = download.IsNew;
            newDownload.SourceId = sourceId;
            newDownload.SourceTypeId = sourceTypeId;
            newDownload.AuthorId = userId;
            newDownload.UseDownloadUrl = download.UseDownloadUrl;
            await _downloadService.InsertDownloadAsync(newDownload);
        }
    }
}
