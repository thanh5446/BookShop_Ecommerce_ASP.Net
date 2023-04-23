using Assignment.DataAccess;
using Assignment.Models;
using Assignment.Models.DBO;
using Assignment.Repositories.Abstraction;
using Assignment.Repositories.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Assignment.Controllers
{

    public class AccountController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly BookShopDbContext _bookShopDbContext;

        public AccountController(IUserAuthenticationService service, UserManager<AppUser> userManager,
            IWebHostEnvironment hostEnvironment, BookShopDbContext bookShopDbContext)
        {
            _service = service;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _bookShopDbContext = bookShopDbContext;
        }

        public IActionResult Register()
        {
            ViewData["Message"] = "Registration Page";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {         
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Role = "Admin";
            var result = await _service.RegistrationAsync(model);
            TempData["msg"] = result.Message;
            return View("Login");
        }

        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                var checkUser = await _userManager.FindByEmailAsync(user.Email);
                var result = await _service.LoginAsync(user);
                if(result.StatusCode == 1)
                {
                    if(await _userManager.IsInRoleAsync(checkUser, "Shop"))
                    {
                        return RedirectToAction("Index","Home", new { area = "Shop" });
                    }else if(await _userManager.IsInRoleAsync(checkUser, "Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["msg"] = result.Message;
                    return View(user);
                }
            }
            else
            {
                return View("Register");
            }
 
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _service.TaskLogoutAsync();
            return View("Login");
        }
        public async Task<IActionResult> Reg()
        {
            var model = new RegistrationModel
            {
                Address = "123 ABC street",
                FullName = "admin Thien Duc",
                Email = "admin@gmail.com",
                Password = "Admin@123",
                PhoneNumber = "0123456789"
              
            };
            model.Role = "Customer";
            var result = await _service.RegistrationAsync(model);
            return Ok(result);
        }

        [Authorize]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Orders = _bookShopDbContext.Order.Where(o => o.UserID == user.Id).ToList();
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var model = new EditProfileModal
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Address = user.Location,
                ProfilePictureUrl = user.ProfilePicture
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileModal model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.FullName = model.FullName;
                user.Location = model.Address;
                user.UserName = model.FullName.Replace(" ","");
                string uniqueFile = null;
                if (model.ProfilePicture != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/avatars");
                    string Extension = Path.GetExtension(model.ProfilePicture.FileName);
                    uniqueFile = Guid.NewGuid().ToString() + Extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFile);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await model.ProfilePicture.CopyToAsync(fileStream);
                    user.ProfilePicture = uniqueFile;
                }
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ViewProfile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("EditProfile",model);
        }
        
    }
}
