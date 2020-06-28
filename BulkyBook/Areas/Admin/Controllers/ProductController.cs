using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")] //first thing to do is put in Area on top of controller
    [Authorize(Roles = SD.Role_Admin)] //Authorization
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;//uploading images in a folder inside wwwroot
        

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
            //get method
        {
            IEnumerable<Category> CatList = await _unitOfWork.Category.GetAllAsync();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = CatList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                //retrieve list to our drop down and pass this ViewModel to our View
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

            };
            if(id == null)
            {
                return View(productVM);
                //means this is for insert(CREATE)
            }
            //this is for edit
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

            if(productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //writes the unique value to an HTTP only cookie and the same value if written to the form
                                   //when the page is submitted, an error is reaised if cookie value is not matched with the form value
                                   //prevent cross site forgery.
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid) //check data annotation and validation in model to see if everything is valid or not
            {
                string webRootPath = _hostEnvironment.WebRootPath; //link to out wwwroot folder
                var files = HttpContext.Request.Form.Files; //file equals to the uploading files from html form
                if(files.Count>0)
                    //if there is a file upload
                {
                    string fileName = Guid.NewGuid().ToString();//global unique identifier
                    var uploads = Path.Combine(webRootPath, @"images\products");//link to our wwroot path to image product
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productVM.Product.ImageUrl!=null)
                    {

                        //this is an edit and we need to remove old image and removethe backward slashes at the start
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            //if the image path already exists we need to delete that
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    //upload new file 
                    using(var fileStreams = new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else 
                {
                    //update when they do not change image
                    if(productVM.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }
                if (productVM.Product.Id == 0)
                {
                    //create new product
                    _unitOfWork.Product.Add(productVM.Product);
                    //since we have SaveChanges in Update but not in Add

                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                IEnumerable<Category> CatList = await _unitOfWork.Category.GetAllAsync();
                productVM.CategoryList = CatList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                //retrieve list to our drop down and pass this ViewModel to our View
                productVM.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                if(productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);

        }


        #region API CALLS
        [HttpGet]
        //IActionResult allows for multiple return values like NotFound, Ok.
        //ActionResult only return pre qualified return type like ActionResult, View, PartialView
        //if we need to return a XML then we have to implement one from IActionResult interface.
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType"); //include properties is for Foreign Key
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if(objFromDb==null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string webRootPath = _hostEnvironment.WebRootPath; //link to out wwwroot folder
                                                               //this is an edit and we need to remove old image and removethe backward slashes at the start
            //we need to physically delete the image inside the folder as well
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                //if the image path already exists we need to delete that
                System.IO.File.Delete(imagePath);
            }
            //if id is found then proceed to delete.
            _unitOfWork.Product.Remove(id);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful!" });
        }
        #endregion
    }

}