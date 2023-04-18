using AspNetCoreHero.ToastNotification.Abstractions;
using Assignment.DataAccess;
using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;

namespace Assignment.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly BookShopDbContext _bookShopDbContext;
        public INotyfService _notyfService { get; }
        public CategoryController(BookShopDbContext bookShopDbContext, INotyfService notyfService)
        {
            _bookShopDbContext = bookShopDbContext;
            _notyfService = notyfService;
        }
        public IActionResult Index(int? page)
        {
            var categories = _bookShopDbContext.Category.Where(s => s.Status.Equals("Processing")).ToList();
            if (page == null) page = 1;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(categories.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Accept(int id)
        {
            var category = _bookShopDbContext.Category.FirstOrDefault(i => i.ID == id);
            if(category == null) return NotFound();
            category.Status = "Active";
            _bookShopDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            Category category = _bookShopDbContext.Category.FirstOrDefault(c => c.ID == id);
            if (id == null || category == null)
            {
                return View("Error");
            }
            _bookShopDbContext.Category.Remove(category);
            _bookShopDbContext.SaveChanges();
            _notyfService.Success("You successfully remoce a category");
            return RedirectToAction("Index");
        }
    }
}
