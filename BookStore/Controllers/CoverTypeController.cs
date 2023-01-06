using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using BookStore.StaticDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

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
            var coverType = new CoverType();
            if (id is null)
            {
                //This is create
                return View(coverType);
            }
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType is null)
            {
                return NotFound();
            }

            return View(coverType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    _unitOfWork.CoverType.Add(coverType);
                }
                else 
                {
                    _unitOfWork.CoverType.Update(coverType);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeToDelete = _unitOfWork.CoverType.Get(id);
            if (coverTypeToDelete is not null)
            {
                _unitOfWork.CoverType.Remove(coverTypeToDelete);
                _unitOfWork.Save();
                return Json( new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var coverTypes = _unitOfWork.CoverType.GetAll();

            return Json(new { data = coverTypes });
        }

        #endregion
    }
}
