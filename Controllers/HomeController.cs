using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taber7.Areas.Identity.Data;
using Taber7.Models;

namespace Taber7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
        public IActionResult Test()
        {
            var post = _applicationDbContext.Posts.Find("abcd");
            ViewData["post"] = post.Html;
            var user = _userManager.Users.First();
            
            ViewData["userId"] = _userManager.GetUserId(HttpContext.User);
            ViewData["userRole"] = user.Id + " " + user.FirstName + " " + user.LastName + " " + HttpContext.User.IsInRole("Admin") + " " + HttpContext.User.IsInRole("ZXC");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}