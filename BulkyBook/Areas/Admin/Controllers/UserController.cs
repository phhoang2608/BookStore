using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")] //first thing to do is put in Area on top of controller
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        //IActionResult allows for multiple return values like NotFound, Ok.
        //ActionResult only return pre qualified return type like ActionResult, View, PartialView
        //if we need to return a XML then we have to implement one from IActionResult interface.
        public IActionResult GetAll()
        {
            //userList: holds list of user
            var userList = _db.ApplicationUsers.Include(u=>u.Company).ToList(); //Include is for eager loading 
                                                                                //when directly using DbContext
                                                                                //cause ApplicationUser has 
                                                                                //a foreign key relationship with Company
                                                                                //so we need to load the Company object

            //userRole: mapping of userList with that role                                                                  
            var userRole = _db.UserRoles.ToList();
            //roles
            var roles = _db.Roles.ToList();

            foreach(var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId; //get the roleId from user
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;//select Name of the role
                if(user.Company == null)
                {
                    //if the user doesn't belong to any company, we just display an empty name rather
                    //than being thrown a ObjectReference not Found
                    
                    user.Company = new Company()
                    {
                        //initialize company
                        Name = ""
                    };

                }
            }

            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u=>u.Id==id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message="Error while locking/unlocking" });
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently being locked, we will unlock them.
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                //user is current NOT being locked, we will lock them with the below statement
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Successfully locked/unlocked user." });
        }

        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var objFromDb = _unitOfWork.User.Get(id);
        //    if(objFromDb==null)
        //    {
        //        return Json(new { success = false, message = "Error while deleting" });
        //    }
        //    //if id is found then proceed to delete.
        //    _unitOfWork.User.Remove(id);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successful!" });
        //}
        #endregion
    }

}