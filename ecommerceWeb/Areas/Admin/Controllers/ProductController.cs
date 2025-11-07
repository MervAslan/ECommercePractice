
using ecommerce.DataAccess.Data;
using ecommerce.DataAccess.Repository.IRepository;
using ecommerce.Models;
using ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork) { //DI sayesinde manuel nesne oluşturmamıza gerek yok
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            
            return View(objProductList); //viewda listenecek
        }
        
        public IActionResult Create() 
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                }),
                Product = new Product()

            };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
           
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully"; 
                return RedirectToAction("Index"); // kaydettikten sonra liste sayfasına yönlendirir
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.CategoryId.ToString()
                });
                return View(productVM);
            }
            

           
        }
        
        public IActionResult Edit(int? id) // get isteğidir kategori verisini form alanlarına doldururuz
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.ProductId == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb); //kategori bulunduysa bu kategori nesnesi edit viewına gönderilir



        }

        [HttpPost] // kullanıcıdan gelen form verilerini alıp işliyoruz
        public IActionResult Edit(Product obj)
        {
           
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index"); // kaydettikten sonra liste sayfasına yönlendirir
            }
            return View();

        }
        public IActionResult Delete(int? id) // kategori verisini form alanlarına dolduruyoruz
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? categoryFromDb = _unitOfWork.Product.Get(u => u.ProductId == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb); //kategori bulunduysa bu kategori nesnesi delete viewına gönderilir



        }

        [HttpPost,ActionName("Delete")] 
        public IActionResult DeletePOST(int? id) //get metodu ile aynı isme sahip olamaz bu yüzden actionname ile ismi eşleştiriyoruz
        {
            Product? obj = _unitOfWork.Product.Get(u => u.ProductId == id);
            if(obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index"); //Kaydettikten sonra liste sayfasına yönlendirir.
            
            

        }

    }
}
