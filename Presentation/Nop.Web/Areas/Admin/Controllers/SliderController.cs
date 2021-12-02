using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.SliderFolder;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class SliderController : BaseAdminController
    {
        #region Feilds 
        private readonly IPermissionService _permissionService;
        private readonly ICategoryModelFactory _categoryModelFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly MediaSettings _mediaSettings;
        #endregion
        public SliderController(
            IPermissionService permissionService,
            ICategoryModelFactory categoryModelFactory,
            IHttpContextAccessor httpContextAccessor,
            IWebHelper webHelper,
            MediaSettings mediaSettings
            )
        {
            this._permissionService = permissionService;
            this._categoryModelFactory = categoryModelFactory;
            this._httpContextAccessor = httpContextAccessor;
            this._webHelper = webHelper;
            this._mediaSettings = mediaSettings;

        }
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        //public virtual async Task<IActionResult> List()
        //{
        //    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
        //        return AccessDeniedView();

        //    //prepare model
        //    var model = await _productModelFactory.PrepareProductSearchModelAsync(new ProductSearchModel());

        //    return View(model);
        //}
        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();


            //prepare model
            SliderListModel Model = SliderPictureList();
            return View(Model);
        }

        [HttpGet]
        public virtual IActionResult DeletePicture(string Name)
        {

            string filesToDelete = @"";
            string currentDir = System.IO.Directory.GetCurrentDirectory();

            filesToDelete = currentDir + "/wwwroot/images/slider/";
            string[] fileList = System.IO.Directory.GetFiles(filesToDelete, Name);
            foreach (string file in fileList)
            {
                //System.Diagnostics.Debug.WriteLine(file + "will be deleted");
                System.IO.File.Delete(file);
            }

            //return Ok("done");
            return RedirectToAction("List");

        }

        public virtual SliderListModel SliderPictureList()
        {

            //path = System.IO.Directory.GetCurrentDirectory()
            //var files = Directory.GetFiles("wwwroot/images/slider" , "*.jpg");
            List<string> jpg = Directory.GetFiles("wwwroot/images/slider", "*.jpg").ToList();
            List<string> png = Directory.GetFiles("wwwroot/images/slider", "*.png").ToList();
            List<string> svg = Directory.GetFiles("wwwroot/images/slider", "*.svg").ToList();
            List<string> files = new List<string>();
            files.AddRange(jpg);
            files.AddRange(png);
            files.AddRange(svg);

            List<SliderModel> sliderModels = new List<SliderModel>();
            foreach (string file in files)
            {
                SliderModel temp = new SliderModel();
                temp.PictureUrl = GetImagesPathUrlAsync(file);
                temp.PictureName = GetImageName(file);
                sliderModels.Add(temp);
            }
            SliderListModel sliderListModel = new SliderListModel();
            sliderListModel.sliderModels = sliderModels;

            //files = File

            //var files = Directory.GetFiles("wwwroot/images/slider").Where(file => Regex.IsMatch(file, @" ^.+\.(jpg|png|svg)$"));

            //var files = Directory.EnumerateFiles("~/wwwroot/images/slider", "*.*", SearchOption.AllDirectories)
            //.Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
            return sliderListModel;
        }


        [HttpPost]
        public virtual async Task<IActionResult> UploadMultiple(ICollection<IFormFile> files)
        {
            string filesToDelete = @"";
            string currentDir = System.IO.Directory.GetCurrentDirectory();

            filesToDelete = currentDir + "/wwwroot/images/slider/";

            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            var uploads = filesToDelete;// Path.Combine(filesToDelete, "uploads");
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            return RedirectToAction("List");
        }

        protected virtual string GetImageName(string storeLocation = null)
        {
            string result = "";
            if (storeLocation.LastIndexOf('\\') != -1)
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('\\') + 1);
            }
            else
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('/') + 1);
            }

            return result;
        }
        protected virtual string GetImagesPathUrlAsync(string storeLocation = null)
        {
            //var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
            //var imagesPathUrl = _mediaSettings.UseAbsoluteImagePath ? storeLocation : $"{pathBase}/";
            //imagesPathUrl = string.IsNullOrEmpty(imagesPathUrl) ? _webHelper.GetStoreLocation() : imagesPathUrl;
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var imagesPathUrl = _webHelper.GetStoreLocation();
            string result;
            if (storeLocation.LastIndexOf('\\') != -1)
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('\\') + 1);
            }
            else
            {
                result = storeLocation.Substring(storeLocation.LastIndexOf('/') + 1);
            }
            imagesPathUrl = imagesPathUrl + "images/slider/" + result;
            //return Task.FromResult(imagesPathUrl);
            return imagesPathUrl;
        }
    }
}
