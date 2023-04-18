using Assignment.DataAccess;
using Assignment.Models;
using Assignment.Models.DBO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Web.Helpers;

namespace Assignment.Controllers
{
    [Authorize]
    public class MyCartController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        const string SessionKeyName = "_Cart";
        private readonly BookShopDbContext _bookShopDbContext;

        public MyCartController(BookShopDbContext bookShopDbContext, UserManager<AppUser> userManager)
        {
            _bookShopDbContext = bookShopDbContext;
            _userManager = userManager;
        }

        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(SessionKeyName) ?? new List<CartViewModel>();
            var userName = User.Identity.Name;
            var user = _userManager.Users.FirstOrDefault(u => u.FullName == userName);
            ViewBag.Account = user;
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id)
        {

            var product = _bookShopDbContext.Book.Where(p => p.ID == id).FirstOrDefault();
            if(product != null)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(SessionKeyName) ?? new List<CartViewModel>();
                var cartItem = cart.FirstOrDefault(item => item.ProductId == id);
                if(cartItem == null)
                {
                   cartItem = new CartViewModel
                   {
                       ProductId = id,
                       ProductName = product.Name,
                       Author = product.Author,
                       Price = product.DiscountPrice,
                       ImageUrl = product.Image,
                       AvailableNum = product.Quantity,
                       Quantity = 1
                   };
                    cart.Add(cartItem);
                }
                else
                {
                    if (product.Quantity < cartItem.Quantity)
                    {
                        cartItem.Quantity += 1;
                    }
                    else
                    {
                        TempData["msg"] = "We don't have more than your desire quantity";
                    }
                }
                HttpContext.Session.SetObjectAsJson<List<CartViewModel>>(SessionKeyName, cart);
            }
            return RedirectToAction("ViewCart");
          
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {      
            var product = _bookShopDbContext.Book.Where(p => p.ID == id).FirstOrDefault();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(SessionKeyName);
            var cartItem = cart.FirstOrDefault(p => p.ProductId == id);
            if ( cartItem != null)
            {
                if (product.Quantity > cartItem.Quantity)
                {
                    cartItem.Quantity = quantity;
                HttpContext.Session.SetObjectAsJson<List<CartViewModel>>(SessionKeyName, cart);
                }
                else
                {
                    TempData["msg"] = "We don't have more than your desire quantity";
                }
            }
            return RedirectToAction("ViewCart");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.SetObjectAsJson<List<CartViewModel>>(SessionKeyName, null);
            return RedirectToAction("ViewCart");
        }
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(SessionKeyName);
            var itemCart = cart.FirstOrDefault(p => p.ProductId == id);
            if(itemCart != null)
            {
                cart.Remove(itemCart);
                HttpContext.Session.SetObjectAsJson<List<CartViewModel>>(SessionKeyName, cart);
            }
            return RedirectToAction("ViewCart");
        }


        [HttpPost]
        public async Task<IActionResult> PerformOrder(decimal total, string userId, string payment)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartViewModel>>(SessionKeyName);
            Order order = new Order();
            if (ModelState.IsValid)
            {
                order.Total = total;
                order.Status = "Pending";
                order.Delete = false;
                order.OrderDate = DateTime.Now;
                order.Note = Request.Form["note"];
                order.UserID = userId;
                order.PaymentMethod = payment;
                await _bookShopDbContext.Order.AddAsync(order);
                await _bookShopDbContext.SaveChangesAsync();
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderID = order.ID,
                        ProductID = item.ProductId,
                        BookID = item.ProductId,
                        Quantity = item.Quantity,
                        Total = item.Price * item.Quantity
                    };
                _bookShopDbContext.OrderDetail.Add(orderDetail);
                var book = _bookShopDbContext.Book.FirstOrDefault(x => x.ID == item.ProductId);
                book.Quantity -= item.Quantity;
                _bookShopDbContext.Entry(book).State = EntityState.Modified;
                }
                await _bookShopDbContext.SaveChangesAsync();
                HttpContext.Session.SetObjectAsJson<List<CartViewModel>>(SessionKeyName, null);
                //foreach (var error in result.DebugView.ShortView)
                //{
                //    TempData["msg"] = error;
                //}
                return RedirectToAction("ViewCart");
                //return Json(result.DebugView.LongView);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
