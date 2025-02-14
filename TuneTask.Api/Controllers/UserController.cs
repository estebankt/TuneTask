using Microsoft.AspNetCore.Mvc;

namespace TuneTask.Api.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
