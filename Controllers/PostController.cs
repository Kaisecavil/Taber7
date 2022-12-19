using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taber7.Areas.Identity.Data;
using Taber7.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Taber7.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _applicationDbContext;
        //private string _userId;
        //private ApplicationUser _user;

        public PostController(ILogger<PostController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            //_userId = _userManager.GetUserId(HttpContext.User);
            //_user = _applicationDbContext.Users.Find(_userId);
        }

        public IActionResult Index()
        {
            List<Post> posts = _applicationDbContext.Posts.ToList();

            return View(posts);
        }

        public async Task<IActionResult> PostAsync(string id)
        {
            Post post = _applicationDbContext.Posts.Find(id);
            ViewBag.Title = post.Title;
            ViewBag.PostId = post.Id;
            ViewBag.Html = post.Html;
            string userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = _applicationDbContext.Users.Find(userid);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            string str = "";
            foreach (var item in roles)
            {
                str = str + item;
            }
            ViewBag.Roles = str;
            //List<Coment> coments = _applicationDbContext.Coments.Where(c => c.PostId == post.Id).ToList();
            List<Coment> coments = _applicationDbContext.Coments.Where(c => c.PostId == post.Id).Include(u => u.User).ToList();
            return View(coments);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post(string id, string comment)
        {
            string userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = _applicationDbContext.Users.Find(userid);
            Coment coment = new Coment(id,userid, comment);
            coment.CreatedDate = DateTime.Now;
            coment.User = user;
            coment.UserId = userid;
            Post post = _applicationDbContext.Posts.Find(id);
            coment.Post = post;
            coment.Id = Guid.NewGuid().ToString();
            _applicationDbContext.Coments.Add(coment);
            _applicationDbContext.SaveChanges();

            return RedirectToAction("Post", new { id = id });

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

        public IActionResult Edit(string str) 
        {
            return Content(str);
            //return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteAsync(string id)
        {

            try
            {
                string userid = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = _applicationDbContext.Users.Find(userid);
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var post = _applicationDbContext.Posts.Find(id);

                if (roles.Contains("Admin"))
                {
                    _applicationDbContext.Posts.Remove(post);
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (post.ApplicationUserId == user.Id)
                {
                    _applicationDbContext.Posts.Remove(post);
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("Access Denied");
                }

            }
            catch (Exception ex)
            {
                return Content("не найдена запись");
            }

            

        }

        [Authorize]
        public async Task<IActionResult> DeleteComentAsync(string id)
        {
            string userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = _applicationDbContext.Users.Find(userid);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var coment = _applicationDbContext.Coments.Find(id);

            if (roles.Contains("Admin"))
            {
                _applicationDbContext.Coments.Remove(coment);
                _applicationDbContext.SaveChanges();
                return RedirectToAction("Post", new { id = coment.PostId });
            }
            else if (coment.UserId == user.Id)
            {
                _applicationDbContext.Coments.Remove(coment);
                _applicationDbContext.SaveChanges();
                return RedirectToAction("Post", new { id = coment.PostId });
            }
            else
            {
                return Content("Access Denied");
            }
            
        }
    }
}
