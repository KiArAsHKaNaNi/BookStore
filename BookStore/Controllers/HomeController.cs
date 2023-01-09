using BookStore.Data;
using BookStore.Data.Data;
using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using BookStore.Models;
using BookStore.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
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
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category, CoverType");

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                //Session for home index page
                var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count);
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id ,includeProperties: "Category, CoverType");
            var shoppingCart = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;

            

            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                var shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.ApplicationUserId == shoppingCart.ApplicationUserId && s.ProductId == shoppingCart.ProductId, includeProperties: "Product");

                if (shoppingCartFromDb is null)
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                }
                else
                {
                    shoppingCartFromDb.Count += shoppingCart.Count;
                    _unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
                }
                _unitOfWork.Save();

                //Session for home details page
                var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count);


                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == shoppingCart.ProductId, includeProperties: "Category, CoverType");
                shoppingCart = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
            }



            return View(shoppingCart);
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
