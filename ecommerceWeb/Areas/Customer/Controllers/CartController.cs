using ecommerce.DataAccess.Repository.IRepository;
using ecommerce.Models;
using ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")] // area belirtmeyi unutma yoksa sayfalar açılmaz routingi belirtmek gerekiyor
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value; // sisteme giriş yapan kullanıcıyı bulur ve id değerini çeker
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
                
            };
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                 cart.Price =GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }
        [Authorize]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.Name=ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(ShoppingCartVM);
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 100) 
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
        public IActionResult Plus(int CartId)
        {

            var shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == CartId);
            shoppingCart.Count += 1;
            _unitOfWork.ShoppingCart.Update(shoppingCart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));


        }
        public IActionResult Minus(int CartId)
        {

            var shoppingCart = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == CartId);
            if (shoppingCart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCart);
                TempData["success"] = "Product deleted successfully";

            }
            else
            {
                shoppingCart.Count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCart);
            }


            _unitOfWork.Save();
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int CartId) 
        {
            var shoppingCart = _unitOfWork.ShoppingCart.Get(u=>u.ShoppingCartId== CartId);
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        
        
    }
}
