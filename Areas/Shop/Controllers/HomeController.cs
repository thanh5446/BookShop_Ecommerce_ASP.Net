using AspNetCoreHero.ToastNotification.Abstractions;
using Assignment.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class HomeController : Controller
    {
        private readonly BookShopDbContext _bookShopDbContext;
        public INotyfService _notyfService { get; }
        public HomeController( INotyfService notyfService, BookShopDbContext bookShopDbContext)
        {
            _bookShopDbContext = bookShopDbContext;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            var now = DateTime.Now.Month;
            ViewBag.TotalSale = _bookShopDbContext.Order.Sum(i => i.Total);
            ViewBag.TotalOrder = _bookShopDbContext.Order.Count();
            ViewBag.TotalProduct = _bookShopDbContext.Book.Count();
            ViewBag.SaleMonth = _bookShopDbContext.Order.Where(i => i.OrderDate.Month == now).Sum(i => i.Total);
            ViewBag.OrderMonth = _bookShopDbContext.Order.Where(i => i.OrderDate.Month == now).Count();
            ViewBag.Users = _bookShopDbContext.AppUser.Count();

            ViewBag.RecentOrders = _bookShopDbContext.Order.OrderBy(i => i.OrderDate).Take(4).ToList();
            ViewBag.InforUsers = _bookShopDbContext.AppUser.ToList();
            ViewBag.TopProducts = _bookShopDbContext.OrderDetail
                .Join(_bookShopDbContext.Book, od =>od.ProductID, p => p.ID, (od, p) => new { OrderDetail = od, Book = p })
                .GroupBy(od => od.OrderDetail.ProductID)
                .OrderByDescending(q => q.Sum(o => o.OrderDetail.Quantity))
                .Take(3).Select(g => g.FirstOrDefault().Book);
            ViewBag.Books = _bookShopDbContext.Book.ToList();
           _notyfService.Information("Welcome to the Shop Owner board");
            return View();
        }
    }
}
