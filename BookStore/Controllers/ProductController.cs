using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var productViewModel = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };
            if (id is null)
            {
                return View(productViewModel);
            }

            productViewModel.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (id is null)
            {
                return NotFound();
            }

            return View(productViewModel);

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
