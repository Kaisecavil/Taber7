using Microsoft.AspNetCore.Mvc;

namespace Taber7.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit() 
        {
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
