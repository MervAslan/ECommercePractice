using Microsoft.AspNetCore.Mvc;

namespace ecommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")] // area belirtmeyi unutma yoksa sayfalar açılmaz routingi belirtmek gerekiyor
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
