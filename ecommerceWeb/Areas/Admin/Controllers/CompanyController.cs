
using ecommerce.DataAccess.Data;
using ecommerce.DataAccess.Repository.IRepository;
using ecommerce.Models;
using ecommerce.Models.ViewModels;
using ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
       
        public CompanyController(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            
            return View(objCompanyList); //viewda listenecek
        }
        
        public IActionResult Upsert(int? id) //update-insert işlemini tek çatı altında topladık
        {
            
           
            if(id== null || id == 0)
            {
                //insert
                return View(new Company);

            }
            else
            {
                //update
                Company companyObj = _unitOfWork.Company.Get(u => u.CompanyId == id);
                return View(companyObj);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
           
            if (ModelState.IsValid)
            {
                
                if(companyObj.CompanyId == 0)
                {
                    _unitOfWork.Company.Add(companyObj); //yeni ürün ekleme
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj); //var olan ürünü güncelleme
                    TempData["success"] = "Company updated successfully";
                }
               
                _unitOfWork.Save();
                
                return RedirectToAction("Index"); // kaydettikten sonra liste sayfasına yönlendirir
            }
            else
            {
                
                return View(companyObj);
            }
            

           
        }
       
        [HttpPost,ActionName("Delete")] 
        public IActionResult DeletePOST(int? id) //get metodu ile aynı isme sahip olamaz bu yüzden actionname ile ismi eşleştiriyoruz
        {
            Company? obj = _unitOfWork.Company.Get(u => u.CompanyId == id);
            if(obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction("Index"); //Kaydettikten sonra liste sayfasına yönlendirir.
            
            

        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.CompanyId == id);
            if(CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
           
            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion

    }
}
