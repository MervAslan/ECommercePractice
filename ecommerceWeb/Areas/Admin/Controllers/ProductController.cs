
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
        private readonly IWebHostEnvironment _webhostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webhostEnvironment)
        { 
            _unitOfWork = unitOfWork;
            _webhostEnvironment = webhostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
            return View(objProductList); //viewda listenecek
        }
        
        public IActionResult Upsert(int? id) //update-insert işlemini tek çatı altında topladık
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
            if(id== null || id == 0)
            {
                //insert
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.ProductId == id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
           
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webhostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //random filename verir
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //eski image i sil
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if(productVM.Product.ProductId == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product); //yeni ürün ekleme
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product); //var olan ürünü güncelleme
                    TempData["success"] = "Product updated successfully";
                }
               
                _unitOfWork.Save();
                
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
