using ecommerceRazor.Data;
using ecommerceRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ecommerceRazor.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        // form verilerini pagemodel propertysine baðlar onpost metoduna ayrý parametere almak zorunda kalmazs
        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if(id != null && id != 0)
            {
                Category = _db.Categories.Find(id);
            }


        }
        public IActionResult OnPost()
        {
            Category? obj = _db.Categories.Find(Category.CategoryId);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToPage("Index");
        }

    }
}
