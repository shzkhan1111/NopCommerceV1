using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class FileListerViewComponent : NopViewComponent
    {
        private readonly IFileManager _fileManagerModelFactory;

        public FileListerViewComponent(IFileManager fileManagerModelFactory)
        {
            _fileManagerModelFactory = fileManagerModelFactory;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int sourceTypeId, int sourceId, bool IsAdmin, int UserId)
        {
            var model = await _fileManagerModelFactory.PrepareFileListerModelAsync(sourceTypeId, sourceId);
            model.IsAdmin = IsAdmin;
            model.UserId = UserId;
            return View(model);
        }
    }
}
