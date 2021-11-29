using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult FileLister()
        {
            return ViewComponent("FileLister", new { sourceTypeId = 1, sourceId = 2 });
        }

        public virtual IActionResult Products()
        {
            return ViewComponent("ChatProductDetails");
        }
        public virtual IActionResult ChatCartDetails()
        {
            return ViewComponent("ChatCartDetails");
        }
        public virtual IActionResult ChatBox()
        {
            return ViewComponent("ChatBox");
        }

    }
}