using ecommerceRazor.Models;
using ecommerceRazor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ecommerceRazor.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        // form verilerini pagemodel propertysine baðlar onpost metoduna ayrý parametere almak zorunda kalmazs
        public Category Category { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            _db.Categories.Add(Category);
            _db.SaveChanges();
            TempData["success"] = "Category created successfully";
            return RedirectToPage("Index");
        }
       


    }
}