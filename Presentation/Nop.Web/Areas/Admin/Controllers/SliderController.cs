using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        #endregion
        public SliderController(
            IPermissionService permissionService, 
            ICategoryModelFactory categoryModelFactory
            )
        {
            this._permissionService = permissionService;
            this._categoryModelFactory = categoryModelFactory;
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
            

            return View();
        }

        public virtual IActionResult SliderPictureList()
        {
            var model = 0;
            return Json(model);
        }
    }
}
