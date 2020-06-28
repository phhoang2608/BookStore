using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]  //first thing to do is put in Area on top of controller
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                return View(coverType);
                //means this is for insert(CREATE)
            }
            //this is for edit
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id); //@Id from the stored procedures command
            coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);

            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //writes the unique value to an HTTP only cookie and the same value if written to the form
                                   //when the page is submitted, an error is reaised if cookie value is not matched with the form value
                                   //prevent cross site forgery.
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid) //check data annotation and validation in model to see if everything is valid or not
            {
                var parameter = new DynamicParameters(); //whenever we work with Stored Procedures we need a dynamic parameters
                parameter.Add("@Name", coverType.Name); //@Name from the stored procedures command
                if (coverType.Id == 0)
                {
                    //create new cover Type
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Create, parameter); 
                    //since we have SaveChanges in Update but not in Add

                }
                else
                {
                    //update new cover Type
                    parameter.Add("@Id", coverType.Name);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);

        }


        #region API CALLS
        [HttpGet]
        //IActionResult allows for multiple return values like NotFound, Ok.
        //ActionResult only return pre qualified return type like ActionResult, View, PartialView
        //if we need to return a XML then we have to implement one from IActionResult interface.
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);

            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id); //@Id from the stored procedures command
            var objFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter); //call one record function from stored procedure
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //if id is found then proceed to delete.
            _unitOfWork.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter); //execute functions inside Storec Procedures call class
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful!" });
        }
        #endregion
    }
}