using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using BookStore.Data.ViewModels;
using BookStore.StaticDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //var listOfCartsForUser = 
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                CartsList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            shoppingCartViewModel.OrderHeader.ApplicationUser = await _userManager.FindByIdAsync(userId);

            foreach (var cart in shoppingCartViewModel.CartsList)
            {
                cart.Price = SD.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.price100);
                shoppingCartViewModel.OrderHeader.OrderTotal += cart.Price * cart.Count;

                if (cart.Product?.Description?.Length > 100)
                {
                    cart.Product.Description = cart.Product.Description?.Substring(0, 99) + "...";
                }
            }

            return View(shoppingCartViewModel);
        }

        [HttpPost]
        public IActionResult Plus(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.Id == cartId, includeProperties: "Product");

            shoppingCart.Count++;
            shoppingCart.Price = SD.GetPriceBaseOnQuantity(shoppingCart.Count, shoppingCart.Product.Price,                                                                              shoppingCart.Product.Price50, shoppingCart.Product.price100);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult Minus(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.Id == cartId, includeProperties: "Product");

            if (shoppingCart.Count == 1)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == shoppingCart.ApplicationUserId).Count();
                _unitOfWork.ShoppingCart.Remove(cartId);
                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count - 1);
            }
            else
            {
                shoppingCart.Count--;
            }

            shoppingCart.Price = SD.GetPriceBaseOnQuantity(shoppingCart.Count, shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.price100);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult Remove(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(s => s.Id == cartId, includeProperties: "Product");

            if (shoppingCart is not null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == shoppingCart.ApplicationUserId).Count();
                _unitOfWork.ShoppingCart.Remove(cartId);
                HttpContext.Session.SetInt32(SD.SessionShoppingCart, count - 1);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }
    }
}
