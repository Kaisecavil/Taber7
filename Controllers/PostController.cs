using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Taber7.Areas.Identity.Data;
using Taber7.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Xml.Linq;

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

        public IActionResult Index(int start = 0,int step = 5)
        {
            List<Post> posts = _applicationDbContext.Posts.Include(p => p.ApplicationUser).ToList();
            try
            {
                List<Post> postse = posts.GetRange(start, step);
                ViewBag.Start = start + step;
              
                return View(postse);
            }
            catch(Exception e)
            {
                ViewBag.Start = start + posts.Count - start;
                return View(posts.GetRange(start, posts.Count-start));
            }
            
        }

        public async Task<IActionResult> PostAsync(string id)
        {
            var post = _applicationDbContext.Posts.Include(p => p.ApplicationUser).Where(p => p.Id == id).First();
            //Post post = _applicationDbContext.Posts.Include(p => p.ApplicationUser).Find(id);
            ViewBag.Title = post.Title;
            ViewBag.PostId = post.Id;
            ViewBag.Html = post.Html;
            ViewBag.Author = post.ApplicationUser.FirstName + " " + post.ApplicationUser.LastName;
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string userid = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = _applicationDbContext.Users.Find(userid);
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                string str = "";
                foreach (var item in roles)
                {
                    str = str + item;
                }
                ViewBag.Roles = str;
            }
            else ViewBag.Roles = "Guest";
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

        [Authorize]
        public async Task<IActionResult> EditAsync(string id) 
        {
            try
            {
                string userid = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = _applicationDbContext.Users.Find(userid);
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var post = _applicationDbContext.Posts.Find(id);

                if (roles.Contains("Admin"))
                {
                    ViewBag.Title = post.Title;
                    ViewBag.Content = post.Html;
                    ViewBag.HttpPost = false;
                    ViewBag.Clive = post.CliveHanger;
                    ViewBag.Tags = post.Tags;
                    ViewBag.Rating = post.Rating;
                    ViewBag.UserId = post.ApplicationUserId;
                    ViewBag.PostId = post.Id;
                    return View(post);
                }
                else if (post.ApplicationUserId == user.Id)
                {
                    ViewBag.Title = post.Title;
                    ViewBag.Content = post.Html;
                    ViewBag.HttpPost = false;
                    ViewBag.Clive = post.CliveHanger;
                    ViewBag.Tags = post.Tags;
                    ViewBag.Rating = post.Rating;
                    ViewBag.UserId = post.ApplicationUserId;
                    ViewBag.PostId = post.Id;
                    return View(post);
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
        public async Task<IActionResult> EditComentAsync(string id)
        {
            ViewBag.HttpPost = true;
            string userid = _userManager.GetUserId(HttpContext.User);
            ApplicationUser user = _applicationDbContext.Users.Find(userid);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var coment = _applicationDbContext.Coments.Find(id);

            if (roles.Contains("Admin"))
            {
                return View(_applicationDbContext.Coments.Find(id));
            }
            else if (coment.UserId == user.Id)
            {
                return View(_applicationDbContext.Coments.Find(id));
            }
            else
            {
                return Content("Access Denied");
            }
            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditComentAsync(string id, string comm)
        {
            ViewBag.HttpPost = true;
           
            try
            {
                string userid = _userManager.GetUserId(HttpContext.User);
                ApplicationUser user = _applicationDbContext.Users.Find(userid);
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                var coment = _applicationDbContext.Coments.Find(id);

                if (roles.Contains("Admin"))
                {
                    coment.Description = comm;
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Post", new { id = coment.PostId });
                }
                else if (coment.UserId == user.Id)
                {
                    coment.Description = comm;
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Post", new { id = coment.PostId });
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

        public IActionResult Find()
        {
            ViewBag.HttpPost = false;
            return View(null);
        }

        [HttpPost]
        public IActionResult Find(string query, string filter)
        {
            ViewBag.HttpPost = true;
            ViewBag.Query = query;
            ViewBag.Filter = filter;
            var posts = _applicationDbContext.Posts.Where(p => p.Title.ToUpper().Contains(query.ToUpper())).ToList<Post>();
            switch (filter)
            {
                case "tag":
                    posts = _applicationDbContext.Posts.Where(p => p.Tags.ToUpper().Contains(query.ToUpper())).ToList<Post>(); break;
                case "title":
                    posts = _applicationDbContext.Posts.Where(p => p.Title.ToUpper().Contains(query.ToUpper())).ToList<Post>(); break;
                case "content":
                    posts = _applicationDbContext.Posts.Where(p => p.Html.ToUpper().Contains(query.ToUpper())).ToList<Post>(); break;
                case "user":
                    posts = _applicationDbContext.Posts.Include(p => p.ApplicationUser).Where(p => p.ApplicationUser.FirstName.ToUpper().Contains(query.ToUpper()) || p.ApplicationUser.LastName.ToUpper().Contains(query.ToUpper()) || p.ApplicationUser.Email.ToUpper().Contains(query.ToUpper())).ToList<Post>(); break;

            }
            return View(posts);
        }

    }

}
