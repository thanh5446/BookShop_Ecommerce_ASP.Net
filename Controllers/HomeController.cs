using AspNetCoreHero.ToastNotification.Abstractions;
using Assignment.DataAccess;
using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using X.PagedList;
namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly BookShopDbContext _bookShopDbContext;
        public HomeController( BookShopDbContext bookShopDbContext, INotyfService notyfService)
        {
           _notyf = notyfService;
            _bookShopDbContext = bookShopDbContext;
        }

        [Route("ListProduct")]
        public IActionResult ListProduct(int? page, string? search, int? categoryId, string? sortPrice)
        {
            _notyf.Success("All your desired products");
            var categories = _bookShopDbContext.Category.ToList();
            var products = _bookShopDbContext.Book.Where(p => p.Status.Equals("InStock"));
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(x => x.Name.Contains(search));
            }
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }
            if (!string.IsNullOrEmpty(sortPrice))
            {
                switch (sortPrice)
                {
                    case "Ascending":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "Descending":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                }
            }
            var productList = products.ToList();
            ViewBag.listProducts = productList;
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Ascending", Value = "Ascending" });
            items.Add(new SelectListItem { Text = "Descending", Value = "Descending" });
            ViewBag.PriceSort = new SelectList(items, "Text", "Value",sortPrice);
            ViewBag.CategoryList = new SelectList(_bookShopDbContext.Category, "ID", "Name", categoryId);
            ViewBag.NameCategory = categories;
            ViewBag.SortPrice = sortPrice;
            ViewBag.categoryId = categoryId;
            return View(productList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [Route("Products/Search")]
        public JsonResult SearchProducts(string search)
        {
            var products = _bookShopDbContext.Book
              .Where(p => p.Name.Contains(search))
              .ToList();
            return Json(products);
        }

        public IActionResult Index()
        {
            _notyf.Custom("Welcome to my book-store", 5, "#B600FF", "fa fa-home");
            ViewBag.TopProducts = _bookShopDbContext.OrderDetail
                .Join(_bookShopDbContext.Book, od => od.ProductID, p => p.ID, (od, p) => new { OrderDetail = od, Book = p })
                .GroupBy(od => od.OrderDetail.ProductID)
                .OrderByDescending(q => q.Sum(o => o.OrderDetail.Quantity))
                .Take(4).Select(g => g.FirstOrDefault().Book);

            ViewBag.NewProducts = _bookShopDbContext.Book.OrderByDescending(i => i.ID).Take(4).ToList();
            return View();
        }

        [Route("GetNewProducts")]
        public IActionResult GetNewProducts(int? page, string? search, int? categoryId, string? sortPrice)
        {
            _notyf.Success("All new products in the store");
            var categories = _bookShopDbContext.Category.ToList();
            var products = _bookShopDbContext.Book.OrderByDescending(p => p.ID).Where(p => p.Status.Equals("InStock"));
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(x => x.Name.Contains(search));
            }
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }
            if (!string.IsNullOrEmpty(sortPrice))
            {
                switch (sortPrice)
                {
                    case "Ascending":
                        products = products.OrderBy(p => p.Price);
                        break;
                    case "Descending":
                        products = products.OrderByDescending(p => p.Price);
                        break;
                }
            }
            var productList = products.ToList();
            ViewBag.listProducts = productList;
            if (page == null) page = 1;
            int pageSize = 1;
            int pageNumber = (page ?? 1);

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Ascending", Value = "Ascending" });
            items.Add(new SelectListItem { Text = "Descending", Value = "Descending" });
            ViewBag.PriceSort = new SelectList(items, "Text", "Value", sortPrice);
            ViewBag.CategoryList = new SelectList(_bookShopDbContext.Category, "ID", "Name", categoryId);
            ViewBag.NameCategory = categories;
            return View("ListProduct",productList.ToPagedList(pageNumber, pageSize));
        }

        //[HttpGet]
        //[Route("ViewDetail/Product")]
        //[ActionName("ViewDetail")]
        public IActionResult ViewDetail(int? id)
        {
            var product = _bookShopDbContext.Book.Find(id);
            if (product != null)
            {
                var productCatId = product.CategoryID;
                ViewBag.RelatedProducts = _bookShopDbContext.Book.Where(p => p.CategoryID == productCatId)
                    .Where(p => p.Status.Equals("InStock"))
                    .Where(p => p.ID != id).Take(4)
                    .ToList();
                ViewBag.NameCategory = _bookShopDbContext.Category.Where(c => c.ID == product.CategoryID).ToList();
                return View(product);
            }
            return RedirectToAction("Index");
        }

        public IActionResult ViewBlog()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}