using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET INDEX
        public async Task<IActionResult> Index()
        {
            return View(await _db.SubCategory.Include(s=>s.Category).ToListAsync());
        }

        //GET CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel();
            model.StatusMessage = "";
            model.SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p=>p.Name).Distinct().ToListAsync();
            model.CategoryList = await _db.Category.ToListAsync();
            model.SubCategory = new SubCategory();

            return View(model);
        }

        //POST CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);
                
                if(doesSubCategoryExist.Count() > 0)
                {
                    //error
                    StatusMessage = "Error : Sub Category Already exist under " + doesSubCategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> SubCategories = new List<SubCategory>();
            SubCategories = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            return Json(new SelectList(SubCategories, "Id", "Name"));
        }

        //GET EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel();
            model.StatusMessage = "";
            model.SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync();
            model.CategoryList = await _db.Category.ToListAsync();
            model.SubCategory = subCategory;

            return View(model);
        }

        //POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

                if (doesSubCategoryExist.Count() > 0)
                {
                    //error
                    StatusMessage = "Error : Sub Category Already exist under " + doesSubCategoryExist.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    var subFromDb = await _db.SubCategory.FindAsync(id);
                    if(subFromDb == null)
                    {
                        return NotFound();
                    }
                    subFromDb.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };

            modelVM.SubCategory.Id = id;

            return View(modelVM);
        }

        //GET DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            SubCategory model = await _db.SubCategory.FindAsync(id);
            model.Category = await _db.Category.FindAsync(model.CategoryId);

            return View(model);

        }

        //GET DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubCategory model = await _db.SubCategory.FindAsync(id);
            model.Category = await _db.Category.FindAsync(model.CategoryId);
            return View(model);
        }

        //POST DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SubCategory subCategory)
        {
            if (subCategory == null)
            {
                return NotFound();
            }

            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}