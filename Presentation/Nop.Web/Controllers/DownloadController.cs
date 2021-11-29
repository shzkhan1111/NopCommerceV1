using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    public partial class DownloadController : BasePublicController
    {
        private readonly CustomerSettings _customerSettings;
        private readonly IDownloadService _downloadService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly INopFileProvider _fileProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;


        public DownloadController(CustomerSettings customerSettings,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IOrderService orderService,
            IProductService productService,
            IWorkContext workContext,
            INopFileProvider fileProvider)
        {
            _customerSettings = customerSettings;
            _downloadService = downloadService;
            _orderService = orderService;
            _productService = productService;
            _workContext = workContext;
            _fileProvider = fileProvider;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> Sample(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return InvokeHttp404();

            if (!product.HasSampleDownload)
                return Content("Product doesn't have a sample download.");

            var download = await _downloadService.GetDownloadByIdAsync(product.SampleDownloadId);
            if (download == null)
                return Content("Sample download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetDownload(Guid orderItemId, bool agree = false)
        {
            var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
            if (orderItem == null)
                return InvokeHttp404();

            var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);

            if (!await _orderService.IsDownloadAllowedAsync(orderItem))
                return Content("Downloads are not allowed");

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (await _workContext.GetCurrentCustomerAsync() == null)
                    return Challenge();

                if (order.CustomerId != (await _workContext.GetCurrentCustomerAsync()).Id)
                    return Content("This is not your order");
            }

            var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

            var download = await _downloadService.GetDownloadByIdAsync(product.DownloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (product.HasUserAgreement && !agree)
                return RedirectToRoute("DownloadUserAgreement", new { orderItemId = orderItemId });


            if (!product.UnlimitedDownloads && orderItem.DownloadCount >= product.MaxNumberOfDownloads)
                return Content(string.Format(await _localizationService.GetResourceAsync("DownloadableProducts.ReachedMaximumNumber"), product.MaxNumberOfDownloads));

            if (download.UseDownloadUrl)
            {
                //increase download
                orderItem.DownloadCount++;
                await _orderService.UpdateOrderItemAsync(orderItem);

                //return result
                //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
                //In this case, it is not relevant. Url may not be local.
                return new RedirectResult(download.DownloadUrl);
            }

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //increase download
            orderItem.DownloadCount++;
            await _orderService.UpdateOrderItemAsync(orderItem);

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetLicense(Guid orderItemId)
        {
            var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
            if (orderItem == null)
                return InvokeHttp404();

            var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);

            if (!await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                return Content("Downloads are not allowed");

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (await _workContext.GetCurrentCustomerAsync() == null || order.CustomerId != (await _workContext.GetCurrentCustomerAsync()).Id)
                    return Challenge();
            }

            var download = await _downloadService.GetDownloadByIdAsync(orderItem.LicenseDownloadId ?? 0);
            if (download == null)
                return Content("Download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderItem.ProductId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        public virtual async Task<IActionResult> GetFileUpload(Guid downloadId)
        {
            var download = await _downloadService.GetDownloadByGuidAsync(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> GetOrderNoteFile(int orderNoteId)
        {
            var orderNote = await _orderService.GetOrderNoteByIdAsync(orderNoteId);
            if (orderNote == null)
                return InvokeHttp404();

            var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId);

            if (await _workContext.GetCurrentCustomerAsync() == null || order.CustomerId != (await _workContext.GetCurrentCustomerAsync()).Id)
                return Challenge();

            var download = await _downloadService.GetDownloadByIdAsync(orderNote.DownloadId);
            if (download == null)
                return Content("Download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderNote.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> UploadFile(string sourceParams)
        {

            var sourceParameters = sourceParams.Split(',').Select(Int32.Parse).ToList();
            int sourceId = sourceParameters[0]; //First paramter in string will be SourceId
            int sourceTypeId = sourceParameters[1]; //Second paramter in string will be SourceTypeId
            int authorId = sourceParameters[2]; //Thord paramter in string will be AuthorId

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            fileName = GetUniqueFileName(fileName, sourceId, fileExtension);

            var customer = await _workContext.GetCurrentCustomerAsync();
            string firstName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            string lastName = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);
            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true,
                SourceId = sourceId,
                SourceTypeId = sourceTypeId,
                AuthorId = authorId,
                CreatedOn = DateTime.Now,
                CreatedBy = firstName + "," + lastName
            };
            await _downloadService.InsertDownloadAsync(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await _localizationService.GetResourceAsync("Download.UploadFile"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
                downloadId = download.Id
            });
        }

        public string GetUniqueFileName(string fileName, int sourceId, string fileExtension)
        {
            try
            {
                string currentTime = DateTime.Now.Hour.ToString("D2") + DateTime.Now.Minute.ToString("D2") + DateTime.Now.Second.ToString("D2");
                string uniqueFileName = fileName.Substring(0, fileName.LastIndexOf(fileExtension));
                uniqueFileName = uniqueFileName + "_" + sourceId + "_";
                uniqueFileName += currentTime;
                uniqueFileName += fileExtension;

                return uniqueFileName;
            }
            catch (Exception exc)
            { }
            return fileName;
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveFile(int docId)
        {

            var uploadedFile = await _downloadService.GetDownloadByIdAsync(docId);
            await _downloadService.DeleteDownloadAsync(uploadedFile);
            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = await _localizationService.GetResourceAsync("Download.Removed"),
            });
        }

    }
}