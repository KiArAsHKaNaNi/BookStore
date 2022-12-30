using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var product = new Product();
            if (id is null)
            {
                //This is create
                return View(product);
            }
            product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (product is null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Id == 0)
                {
                    _unitOfWork.Product.Add(product);
                }
                else 
                {
                    _unitOfWork.Product.Update(product);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productToDelete = _unitOfWork.Product.Get(id);
            if (productToDelete is not null)
            {
                _unitOfWork.Product.Remove(productToDelete);
                _unitOfWork.Save();
                return Json( new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll();

            return Json(new { data = products });
        }

        #endregion
    }
}
