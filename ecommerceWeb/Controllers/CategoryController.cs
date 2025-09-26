using ecommerceWeb.Data;
using ecommerceWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) { //DI sayesinde manuel nesne oluşturmamıza gerek yok
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(obj);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.FirstOrDefault(u => u.CategoryId == id);
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
                _db.Categories.Update(obj);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.FirstOrDefault(u => u.CategoryId == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb); //kategori bulunduysa bu kategori nesnesi delete viewına gönderilir



        }

        [HttpPost,ActionName("Delete")] 
        public IActionResult DeletePOST(int? id) //get metodu ile aynı isme sahip olamaz bu yüzden actionname ile ismi eşleştiriyoruz
        {
            Category? obj = _db.Categories.FirstOrDefault(u => u.CategoryId == id);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index"); //Kaydettikten sonra liste sayfasına yönlendirir.
            
            

        }

    }
}
