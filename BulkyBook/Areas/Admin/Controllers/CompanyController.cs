using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")] //first thing to do is put in Area on top of controller
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if(id == null)
            {
                return View(company);
                //means this is for insert(CREATE)
            }
            //this is for edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());

            if(company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //writes the unique value to an HTTP only cookie and the same value if written to the form
                                    //when the page is submitted, an error is reaised if cookie value is not matched with the form value
                                    //prevent cross site forgery.
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid) //check data annotation and validation in model to see if everything is valid or not
            {
                if (company.Id == 0)
                {
                    //create new company
                    _unitOfWork.Company.Add(company);
                    //since we have SaveChanges in Update but not in Add
 
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
           
        }


        #region API CALLS
        [HttpGet]
        //IActionResult allows for multiple return values like NotFound, Ok.
        //ActionResult only return pre qualified return type like ActionResult, View, PartialView
        //if we need to return a XML then we have to implement one from IActionResult interface.
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);
            if(objFromDb==null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //if id is found then proceed to delete.
            _unitOfWork.Company.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful!" });
        }
        #endregion
    }

}