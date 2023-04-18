using AspNetCoreHero.ToastNotification.Abstractions;
using Assignment.DataAccess;
using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreHero.ToastNotification.Abstractions;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Assignment.Models.DBO;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class OrderController : Controller
    {
        public INotyfService _notyfService { get; }
        private readonly BookShopDbContext _bookShopDbContext;
        public OrderController(BookShopDbContext bookShopDbContext, INotyfService notyfService)
        {
            _bookShopDbContext = bookShopDbContext;
            _notyfService = notyfService;
        }
        public IActionResult Index(int? page, string? search, string? status, string? sortNew)
        {
            var orders = _bookShopDbContext.Order.Where(d => d.Delete == false);
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(s => s.Status.Equals(status));
            }
            if (!string.IsNullOrEmpty(sortNew))
            {
                switch (sortNew)
                {
                    case "New":
                        orders = orders.OrderByDescending(p => p.OrderDate);
                        break;
                    case "Old":
                        orders = orders.OrderBy(p => p.OrderDate);
                        break;
                }
            }
            var orderList = orders.ToList();

            if (page == null) page = 1;
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            items.Add(new SelectListItem { Text = "Processing", Value = "Processing" });
            items.Add(new SelectListItem { Text = "Delivered", Value = "Delivered" });
            items.Add(new SelectListItem { Text = "Cancelled", Value = "Cancelled" });
            ViewBag.StatusList = new SelectList(items, "Text", "Value", status);

            List<SelectListItem> sortItems = new List<SelectListItem>();
            sortItems.Add(new SelectListItem { Text = "New", Value = "New To Old" });
            sortItems.Add(new SelectListItem { Text = "Old", Value = "Old To New" });
            ViewBag.NewSort = new SelectList(sortItems, "Text","Value",sortNew);
            ViewBag.StoredStatus = status;
            ViewBag.StoredNew = sortNew;
            return View(orderList.ToPagedList(pageNumber,pageSize));
        }


        [HttpGet]
        public async Task<IActionResult> ViewDetail(int id)
        {
           var order = _bookShopDbContext.Order.FirstOrDefault(o => o.ID == id);
           var customer = _bookShopDbContext.AppUser.FirstOrDefault(c => c.Id == order.UserID);
           var ListProduct = _bookShopDbContext.OrderDetail.Where(od => od.OrderID == order.ID).ToList();
            ViewBag.ListProducts = ListProduct;
            ViewBag.InforCustomer = customer;
            ViewBag.Books = _bookShopDbContext.Book.ToList();

            OrderViewModel viewModel = new OrderViewModel
            {
                ID = order.ID,
                UserID = order.UserID,
                Total = order.Total,
                Delete = order.Delete,
                Note = order.Note,
                OrderDate = order.OrderDate,
                PaymentMethod = order.PaymentMethod,
                Status = order.Status
            };
            

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            items.Add(new SelectListItem { Text = "Processing", Value = "Processing" });
            items.Add(new SelectListItem { Text = "Delivered", Value = "Delivered" });
            items.Add(new SelectListItem { Text = "Cancelled", Value = "Cancelled" });
            ViewBag.StatusList = new SelectList(items, "Text", "Value");
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewDetail()
        {
            int id = Convert.ToInt32(Request.Form["orderId"]);
            var order = _bookShopDbContext.Order.FirstOrDefault(o => o.ID == id);
            var status = Request.Form["status"];
            if (order == null)
            {
                return View("Error");
            }
            if (ModelState.IsValid)
            {
                order.Status = status;
                _bookShopDbContext.Entry(order).State = EntityState.Modified;
                await _bookShopDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }          
            return Json(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
           var order = _bookShopDbContext.Order.FirstOrDefault(o => o.ID == id);
            if (ModelState.IsValid)
            {
                order.Delete = true;
                _bookShopDbContext.Entry(order).State = EntityState.Modified;
                await _bookShopDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return Json(order);
        }
    }
}
