using System.Diagnostics;
using System.Security.Claims;
using ecommerce.DataAccess.Repository.IRepository;
using ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category")
            };
           
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
           var claimsIdentity = (ClaimsIdentity)User.Identity; 
           var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value; // sisteme giriþ yapan kullanýcýyý bulur ve id deðerini çeker
           shoppingCart.ApplicationUserId = userId;                               // kullanýcý giriþ yapmadýysa [Authorize] attribute u sayesinde otomatik olarak login ekranýna yönlendirir

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            if(cartFromDb != null)
            {
                cartFromDb.Count+= shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb); // update metodunu çaðýrmasak bile efcore change tracking(deðiþiklik takibi)
            }                                                // yaptýðýndan dolayý, bir veriyi hafýzada deðiþtirdiðimiz an efcore bunu fark eder ve savechanges dediðimizde veritabanýnda günceller 
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
           
            _unitOfWork.Save();
            TempData["success"] = "Cart Added successfully";
            return RedirectToAction(nameof(Index)); // ilerde indexin adýný deðiþirse diye nameof kullanýyoruz böylece hata yapma olasýlýðýmýz azalýr

        }
        public IActionResult Privacy()
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
