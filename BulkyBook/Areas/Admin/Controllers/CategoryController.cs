using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")] //first thing to do is put in Area on top of controller
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int productPage = 1)
        {
            CategoryVM categoryVM = new CategoryVM()
            {
                Categories = await _unitOfWork.Category.GetAllAsync()
            };
            var count = categoryVM.Categories.Count();
            categoryVM.Categories = categoryVM.Categories.OrderBy(p => p.Name)
                .Skip((productPage-1)*2).Take(2).ToList();
            categoryVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = 2,
                TotalItem = count,
                urlParam = "/Admin/Category/Index?productPage=:"
            };
            return View(categoryVM);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();
            if(id == null)
            {
                return View(category);
                //means this is for insert(CREATE)
            }
            //this is for edit
            category = await _unitOfWork.Category.GetAsync(id.GetValueOrDefault());

            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //writes the unique value to an HTTP only cookie and the same value if written to the form
                                    //when the page is submitted, an error is reaised if cookie value is not matched with the form value
                                    //prevent cross site forgery.
        public async Task<IActionResult> Upsert(Category category)
        {
            if(ModelState.IsValid) //check data annotation and validation in model to see if everything is valid or not
            {
                if (category.Id == 0)
                {
                    //create new category
                    await _unitOfWork.Category.AddAsync(category);
                    //since we have SaveChanges in Update but not in Add
 
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
           
        }


        #region API CALLS
        [HttpGet]
        //IActionResult allows for multiple return values like NotFound, Ok.
        //ActionResult only return pre qualified return type like ActionResult, View, PartialView
        //if we need to return a XML then we have to implement one from IActionResult interface.
        public async Task<IActionResult> GetAll()
        {
            var allObj = await _unitOfWork.Category.GetAllAsync();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var objFromDb = await _unitOfWork.Category.GetAsync(id);
            if(objFromDb==null)
            {
                TempData["Error"] = "Error Deleting Category";
                return Json(new { success = false, message = "Error while deleting" });
            }
            //if id is found then proceed to delete.
            await _unitOfWork.Category.RemoveAsync(id);
            _unitOfWork.Save();
            TempData["Success"] = "Category Succesfully Deleted";
            return Json(new { success = true, message = "Delete Successful!" });
        }
        #endregion
    }

}