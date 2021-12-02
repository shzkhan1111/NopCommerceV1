using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Media;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.SliderFolder;
using System.IO;
using Nop.Web.Areas.Admin.Models.Picture;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class ProductImageController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;

        public static string imagesrcPath = @"wwwroot/";
        public static string imagePath =  @"images/ProductImages/";
        public ProductImageController(
            IPermissionService permissionService,
            IWebHelper webHelper
            )
        {
            _permissionService = permissionService;
            _webHelper = webHelper;
        }

        public virtual IActionResult Index()    
        {
            return RedirectToAction("List");
        }


        [HttpGet]
        public virtual IActionResult DeletePicture(string Name)
        {

            string filesToDelete = @"";
            string currentDir = System.IO.Directory.GetCurrentDirectory();

            //filesToDelete = currentDir + "/wwwroot/images/slider/";
            filesToDelete = currentDir + "/wwwroot/" + imagePath;
            string[] fileList = System.IO.Directory.GetFiles(filesToDelete, Name);
            foreach (string file in fileList)
            {
                //System.Diagnostics.Debug.WriteLine(file + "will be deleted");
                System.IO.File.Delete(file);
            }

            //return Ok("done");
            return RedirectToAction("List");

        }


        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();


            //prepare model
            PictureListModel Model = ProductPictureList();
            return View(Model);
        }
        public virtual PictureListModel ProductPictureList()
        {

            //path = System.IO.Directory.GetCurrentDirectory()
            //var files = Directory.GetFiles("wwwroot/images/slider" , "*.jpg");
            List<string> jpg = Directory.GetFiles(imagesrcPath + imagePath, "*.jpg").ToList();
            List<string> png = Directory.GetFiles(imagesrcPath + imagePath, "*.png").ToList();
            List<string> svg = Directory.GetFiles(imagesrcPath + imagePath, "*.svg").ToList();
            List<string> jpeg = Directory.GetFiles(imagesrcPath + imagePath, "*.jpeg").ToList();
            //List<string> png = Directory.GetFiles("wwwroot/images/ProductImages", "*.png").ToList();
            //List<string> svg = Directory.GetFiles("wwwroot/images/ProductImages", "*.svg").ToList();
            //List<string> jpeg = Directory.GetFiles("wwwroot/images/ProductImages", "*.jpeg").ToList();
            List<string> files = new List<string>();
            files.AddRange(jpg);
            files.AddRange(png);
            files.AddRange(svg);
            files.AddRange(jpeg);

            List<PictureModel> pictureModels = new List<PictureModel>();
            foreach (string file in files)
            {
                PictureModel temp = new PictureModel();
                temp.PictureUrl = GetImagesPathUrlAsync(file);
                temp.PictureName = GetImageName(file);
                pictureModels.Add(temp);
            }
            PictureListModel ProductPictureListModel = new PictureListModel();
            ProductPictureListModel.pictureModels = pictureModels;

            //files = File

            //var files = Directory.GetFiles("wwwroot/images/slider").Where(file => Regex.IsMatch(file, @" ^.+\.(jpg|png|svg)$"));

            //var files = Directory.EnumerateFiles("~/wwwroot/images/slider", "*.*", SearchOption.AllDirectories)
            //.Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
            return ProductPictureListModel;
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
            imagesPathUrl = imagesPathUrl + imagePath + result;
            return imagesPathUrl;
        }
    }
}
