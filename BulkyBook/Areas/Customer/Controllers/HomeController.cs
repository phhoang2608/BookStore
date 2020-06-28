using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Respository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity; //get the information of the user that 
                                                                //associated with this action
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                   .GetAll(c => c.ApplicationUserId == claim.Value)
                   .ToList()
                   .Count();
                //store anything you want
                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObj);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.
                GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] //you have to log in as a user to purchase
        public IActionResult Details(ShoppingCart CartObj)
        {

            CartObj.Id = 0;
            if(ModelState.IsValid)
            {
                //then we will add the cart
                var claimsIdentity = (ClaimsIdentity)User.Identity; //get the information of the user that 
                                                                    //associated with this action
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObj.ApplicationUserId = claim.Value; //storing the UserId of the log in user

                //inside the ShoppingCart object we will load the Id that is inside the Product Id.
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart
                    .GetFirstOrDefault(u => u.ApplicationUserId == CartObj.ApplicationUserId && u.ProductId == CartObj.ProductId,
                    includeProperties: "Product"); 
                //product is eager loading in Shopping Cart
                if(cartFromDb == null)
                {
                    //no records exists in database for that product for that user so we will add a new shopping cart
                    _unitOfWork.ShoppingCart.Add(CartObj);

                }
                else
                {
                    cartFromDb.Count += CartObj.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();
                //get all entities based on user Id
                var count = _unitOfWork.ShoppingCart.GetAll(c=>c.ApplicationUserId==CartObj.ApplicationUserId)
                    .ToList().Count();
                //store anything you want
                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObj);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
                //var obj = HttpContext.Session.GetObject<ShoppingCart>(SD.ssShoppingCart);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _unitOfWork.Product.
                GetFirstOrDefault(u => u.Id == CartObj.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }

            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
