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
       // private readonly ILogger<HomeController> _logger;
        private readonly BookShopDbContext _bookShopDbContext;
        public HomeController( BookShopDbContext bookShopDbContext)
        {
           // _logger = logger;
            _bookShopDbContext = bookShopDbContext;
        }

        [Route("ListProduct")]
        public IActionResult ListProduct(int? page, string? search, int? categoryId, string? sortPrice)
        {
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
            int pageSize = 1;
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

        public IActionResult Index()
        {
            return View();
        }

        [Route("GetNewProducts")]
        public IActionResult GetNewProducts(int? page, string? search, int? categoryId, string? sortPrice)
        {
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



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}