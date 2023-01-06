using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using BookStore.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var category = new Category();
            if (id is null)
            {
                //This is create
                return View(category);
            }
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category is null)
            {
                return NotFound();
            }

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else 
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryToDelete = _unitOfWork.Category.Get(id);
            if (categoryToDelete is not null)
            {
                _unitOfWork.Category.Remove(categoryToDelete);
                _unitOfWork.Save();
                return Json( new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoris = _unitOfWork.Category.GetAll();

            return Json(new { data = categoris });
        }

        #endregion
    }
}
