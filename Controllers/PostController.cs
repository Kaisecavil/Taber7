using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taber7.Areas.Identity.Data;
using Taber7.Models;
using System.Linq;

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
            List<Post> posts = _applicationDbContext.Posts.ToList();

            return View(posts);
        }

        public IActionResult Post(string id)
        {
            Post post = _applicationDbContext.Posts.Find(id);
            ViewBag.Title = post.Title;
            ViewBag.Html = post.Html;
            List<Coment> coments = _applicationDbContext.Coments.Where(c => c.PostId == post.Id).ToList();
            return View(coments);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post(string id, string comment)
        {
            Coment coment = new Coment(id, comment);
            coment.CreatedDate = DateTime.Now;
            Post post = _applicationDbContext.Posts.Find(id);
            coment.Post = post;
            coment.Id = Guid.NewGuid().ToString();
            _applicationDbContext.Coments.Add(coment);
            _applicationDbContext.SaveChanges();

            return Content(coment.ToString());
           
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(string title, string clive, string tags , string content)
        {
            
            ViewBag.Title = title;
            ViewBag.Content = content;
            ViewBag.HttpPost = true;
            ViewBag.Clive = clive;
            ViewBag.Tags = tags;

            var userId = _userManager.GetUserId(HttpContext.User);
            Post post = new Post(title,clive,tags, content,userId);
            post.CreatedDate = DateTime.Now;
            post.Id = Guid.NewGuid().ToString();
            _applicationDbContext.Posts.Add(post);
            _applicationDbContext.SaveChanges();
            return View();
        }

        [Authorize]
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
