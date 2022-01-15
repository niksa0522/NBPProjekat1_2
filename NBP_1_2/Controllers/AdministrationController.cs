using Microsoft.AspNetCore.Mvc;

namespace NBP_1_2.Controllers
{
    public class AdministrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
