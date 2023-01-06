using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using BookStore.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BookStore.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
        public IActionResult Upsert(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                //Changing or adding the image
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + "_" + files[0].FileName;
                    var photoPath = Path.Combine(webRootPath, @"images\product");
                    var filePath = Path.Combine(photoPath, fileName);
                    if (productViewModel.Product.ImageUrl is not null)
                    {
                        //This is Edit
                        var imagePath = Path.Combine(photoPath, productViewModel.Product.ImageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    productViewModel.Product.ImageUrl = fileName;

                }
                else
                {
                    //Update when not changing the image
                    if (productViewModel.Product.Id != 0)
                    {
                        var product = _unitOfWork.Product.Get(productViewModel.Product.Id);
                        productViewModel.Product.ImageUrl = product.ImageUrl;
                    }
                }

                if (productViewModel.Product.Id == 0)
                {

                    _unitOfWork.Product.Add(productViewModel.Product);
                }
                else 
                {
                    _unitOfWork.Product.Update(productViewModel.Product);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
                productViewModel.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
                if (productViewModel.Product.Id != 0)
                {
                    productViewModel.Product = _unitOfWork.Product.Get(productViewModel.Product.Id);
                }
            }
            return View(productViewModel);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productToDelete = _unitOfWork.Product.Get(id);
            if (productToDelete is not null)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var photoPath = Path.Combine(webRootPath, @"images\product");
                //This is Edit
                var imagePath = Path.Combine(photoPath, productToDelete.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
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
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category, CoverType");

            return Json(new { data = products });
        }

        #endregion
    }
}
