using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taber7.Areas.Identity.Data;
using Taber7.Models;

namespace Taber7.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _applicationDbContext;

        public PostController(ILogger<PostController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            List<Post> posts =  _applicationDbContext.Posts.ToList();

            return View(posts);
        }

        [HttpPost]
        public IActionResult Create(string title, string content)
        {
            
            ViewBag.Title = title;
            ViewBag.Content = content;
            ViewBag.HttpPost = true;
            
            var userId = _userManager.GetUserId(HttpContext.User);
            Post post = new Post(title, content,userId);
            post.CreatedDate = DateTime.Now;
            post.Id = Guid.NewGuid().ToString();
            _applicationDbContext.Posts.Add(post);
            _applicationDbContext.SaveChanges();
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.HttpPost = false;
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
