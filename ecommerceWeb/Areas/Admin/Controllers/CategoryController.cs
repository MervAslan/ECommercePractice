
using ecommerce.DataAccess.Data;
using ecommerce.DataAccess.Repository.IRepository;
using ecommerce.Models;
using ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork) { //DI sayesinde manuel nesne oluşturmamıza gerek yok
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList); //viewda listenecek
        }
        
        public IActionResult Create() {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully"; 
                return RedirectToAction("Index"); // kaydettikten sonra liste sayfasına yönlendirir
            }
            return View();
                

           
        }
        
        public IActionResult Edit(int? id) // get isteğidir kategori verisini form alanlarına doldururuz
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb); //kategori bulunduysa bu kategori nesnesi edit viewına gönderilir



        }

        [HttpPost] // kullanıcıdan gelen form verilerini alıp işliyoruz
        public IActionResult Edit(Category obj)
        {
           
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
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
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb); //kategori bulunduysa bu kategori nesnesi delete viewına gönderilir



        }

        [HttpPost,ActionName("Delete")] 
        public IActionResult DeletePOST(int? id) //get metodu ile aynı isme sahip olamaz bu yüzden actionname ile ismi eşleştiriyoruz
        {
            Category? obj = _unitOfWork.Category.Get(u => u.CategoryId == id);
            if(obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index"); //Kaydettikten sonra liste sayfasına yönlendirir.
            
            

        }

    }
}
