using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Assignment.DataAccess;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment.Models.DBO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using IdentityRole = Microsoft.AspNetCore.Identity.IdentityRole;

namespace Assignment.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly BookShopDbContext _bookShopDbContext;
        public HomeController(INotyfService notyfService, BookShopDbContext bookShopDbContext, UserManager<AppUser> userManager)
        {
            _bookShopDbContext = bookShopDbContext;
            _userManager = userManager;
        }
        public IActionResult Index(int? page, string? search)
        {
            var cusUsers = _bookShopDbContext.AppUser
                            .Join(_bookShopDbContext.UserRoles, u => u.Id, r => r.UserId, (u, r) => new { Customer = u, RoleUser = r })
                            .Join(_bookShopDbContext.Roles, x => x.RoleUser.RoleId, y => y.Id, (x,y) => new {list = x, Roles = y})
                            .Where(q => q.Roles.Name.Equals("Customer"))
                            .Select(result => result.list.Customer);
                            
           // var users = _bookShopDbContext.AppUser.Where(u => u.EmailConfirmed == true);
            if (!string.IsNullOrEmpty(search))
            {
                cusUsers = cusUsers.Where(u => u.FullName.Contains(search));
            }
            var ListUser = cusUsers.ToList();
            if (page == null) page = 1;
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.StoredSearch = search;
            return View(ListUser.ToPagedList(pageNumber, pageSize));
        }

        [Route("Admin/Home/Owners")]
        public IActionResult Owners(int? page, string? search)
        {
            var cusUsers = _bookShopDbContext.AppUser
                          .Join(_bookShopDbContext.UserRoles, u => u.Id, r => r.UserId, (u, r) => new { Customer = u, RoleUser = r })
                          .Join(_bookShopDbContext.Roles, x => x.RoleUser.RoleId, y => y.Id, (x, y) => new { list = x, Roles = y })
                          .Where(q => q.Roles.Name.Equals("Shop"))
                          .Select(result => result.list.Customer);

            if (!string.IsNullOrEmpty(search))
            {
                cusUsers = cusUsers.Where(u => u.FullName.Contains(search));
            }
            var ListUser = cusUsers.ToList();
            if (page == null) page = 1;
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.StoredSearch = search;
            return View(ListUser.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult ChangePassword(string id)
        {
           var user =  _bookShopDbContext.AppUser.FirstOrDefault(u => u.Id == id);
            ChangePasswordWM model = new ChangePasswordWM
            {
                Email = user.Email,
                Password = user.PasswordHash,
                ConfirmPassword = user.PasswordHash
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordWM model)
        {
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var user = await _userManager.FindByEmailAsync(email);
            var code= await _userManager.GeneratePasswordResetTokenAsync(user);
            
            if (user == null)
            {
                return Json(email);
            }
            var result = await _userManager.ResetPasswordAsync(user, code , password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            ChangePasswordWM model2 = new ChangePasswordWM
            {
                Email = user.Email,
                Password = user.PasswordHash,
                ConfirmPassword = user.PasswordHash,
                Code = user.Id
            };

            return View(model2);
        }

        [HttpPost]
        public IActionResult RemoveConfirmed(string id)
        {
            var user = _bookShopDbContext.AppUser.FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                return Json("Not found");
            }
            _bookShopDbContext.AppUser.Remove(user);
            _bookShopDbContext.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
