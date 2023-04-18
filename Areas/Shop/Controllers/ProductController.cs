using Assignment.DataAccess;
using Assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using X.PagedList;


namespace Assignment.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ProductController : Controller
    {
        private readonly BookShopDbContext _bookShopDbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(BookShopDbContext bookShopDbContext, IWebHostEnvironment hostEnvironment)
        {
            _bookShopDbContext = bookShopDbContext;
            _hostEnvironment = hostEnvironment;
        }
       
        public IActionResult Index(int? page, string? search, string? status, string? sortPrice)
        {
            var books = _bookShopDbContext.Book.Where(b => b.Status != "Deleted");
            if (!string.IsNullOrEmpty(search))
            {
                books = books.Where(s => s.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(status))
            {
                books = books.Where(s => s.Status == status);
            }
            if (!string.IsNullOrEmpty(sortPrice))
            {
                switch (sortPrice)
                {
                    case "Ascending":
                        books = books.OrderBy(c => c.DiscountPrice);
                        break;
                    case "Descending":
                        books = books.OrderByDescending(c => c.DiscountPrice);
                        break;
                }
            }
            var ListBooks = books.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "InStock", Value = "InStock" });
            items.Add(new SelectListItem { Text = "OutOfStock", Value = "OutOfStock" });
            ViewBag.StatusList = new SelectList(items, "Text", "Value", status);

            List<SelectListItem> sortItems = new List<SelectListItem>();
            sortItems.Add(new SelectListItem { Text = "Ascending", Value = "Ascending" });
            sortItems.Add(new SelectListItem { Text = "Descending", Value = "Descending" });
            ViewBag.PriceSort = new SelectList(sortItems, "Text", "Value", sortPrice);

            if (page == null) page = 1;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            ViewBag.StoredStatus = status;
            ViewBag.StoredPrice = sortPrice;

            return View(ListBooks.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            List<Category> SuitableCategories = _bookShopDbContext.Category.Where(c => c.Status.Equals("Active")).ToList();
            ViewBag.categoryList = new SelectList(SuitableCategories, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("ID,Name,Description,Price,DiscountPrice,Author,Status,Quantity,Image,CategoryID")] BookVM bookVM)
        {
           
            if (ModelState.IsValid)
            {
                string uniqueFile = null;
                if (bookVM.Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/products");
                    string Extension = Path.GetExtension(bookVM.Image.FileName);
                    uniqueFile = Guid.NewGuid().ToString() + Extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFile);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await bookVM.Image.CopyToAsync(fileStream);
                }

                Book book = new Book
                {
                    ID = bookVM.ID,
                    Name = bookVM.Name,
                    Price = bookVM.Price,
                    DiscountPrice = bookVM.DiscountPrice,
                    Description = bookVM.Description,
                    Quantity = bookVM.Quantity,
                    Status = bookVM.Status,
                    Author = bookVM.Author,
                    CategoryID = bookVM.CategoryID,
                    Image = uniqueFile
                };


                _bookShopDbContext.Book.Add(book);
                await _bookShopDbContext.SaveChangesAsync();
                return RedirectToAction("index");
            }
            List<Category> SuitableCategories = _bookShopDbContext.Category.Where(c => c.Status.Equals("Active")).ToList();
            ViewBag.categoryList = new SelectList(SuitableCategories, "ID", "Name");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
        Book book = _bookShopDbContext.Book.FirstOrDefault(p => p.ID == id);
            BookVM bookVM = new BookVM();

        if (book == null)
            {
                return NotFound();
            }
            else
            {
                 bookVM = new BookVM
                {
                    ID = book.ID,
                    Name = book.Name,
                    Price = book.Price,
                    DiscountPrice = book.DiscountPrice,
                    Description = book.Description,
                    Quantity = book.Quantity,
                    Status = book.Status,
                    Author = book.Author,
                    CategoryID = book.CategoryID,
                    Image2 = book.Image
                };
            }
       

            List<Category> SuitableCategories = _bookShopDbContext.Category.Where(c => c.Status.Equals("Active")).ToList();
            ViewBag.categoryList = new SelectList(SuitableCategories, "ID", "Name");
            ViewBag.Image = book.Image;
        return View(bookVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateProduct(BookVM bookVM)
        {
            if (ModelState.IsValid)
            {
                Book searchBook = _bookShopDbContext.Book.FirstOrDefault(p => p.ID == bookVM.ID);
                string uniqueFile = searchBook.Image;
                if (bookVM.Image != null)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images/products");
                    string Extension = Path.GetExtension(bookVM.Image.FileName);
                    uniqueFile = Guid.NewGuid().ToString() + Extension;
                    string filePath = Path.Combine(uploadsFolder, uniqueFile);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    await bookVM.Image.CopyToAsync(fileStream);
                }

                Book book = new Book
                {
                    ID = bookVM.ID,
                    Name = bookVM.Name,
                    Price = bookVM.Price,
                    DiscountPrice = bookVM.DiscountPrice,
                    Description = bookVM.Description,
                    Quantity = bookVM.Quantity,
                    Status = bookVM.Status,
                    Author = bookVM.Author,
                    CategoryID = bookVM.CategoryID,
                    Image = uniqueFile
                };
                var existingBook = _bookShopDbContext.Book.FirstOrDefault(b => b.ID == book.ID);
                if (existingBook != null)
                {
                    _bookShopDbContext.Entry(existingBook).State = EntityState.Detached;
                }
                _bookShopDbContext.Entry(book).State = EntityState.Modified;
                await _bookShopDbContext.SaveChangesAsync();

                ViewBag.Image = book.Image;
                return RedirectToAction("Index");
            }
            List<Category> SuitableCategories = _bookShopDbContext.Category.Where(c => c.Status.Equals("Active")).ToList();
            ViewBag.categoryList = new SelectList(SuitableCategories, "ID", "Name");
            return View(bookVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveConfirmed(int id)
        {
            Book book = _bookShopDbContext.Book.FirstOrDefault(c => c.ID == id);
            if (id == null || book == null)
            {
                return View("Error");
            }
            _bookShopDbContext.Book.Remove(book);
            _bookShopDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
