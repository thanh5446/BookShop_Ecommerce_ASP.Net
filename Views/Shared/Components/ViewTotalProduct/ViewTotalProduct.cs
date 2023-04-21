using Assignment.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Views.Home.Component.ViewTotalProduct
{
    public class ViewTotalProduct : ViewComponent
    {
        private readonly BookShopDbContext _bookShopDbContext;

        public ViewTotalProduct(BookShopDbContext bookShopDbContext)
        {
            _bookShopDbContext = bookShopDbContext;
        }

        public IViewComponentResult Invoke()
        {
            return View(_bookShopDbContext.Book.Count());
        }
    }
}
