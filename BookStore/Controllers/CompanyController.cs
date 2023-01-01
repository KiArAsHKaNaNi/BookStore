using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            var company = new Company();
            if (id is null)
            {
                //This is create
                return View(company);
            }
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company is null)
            {
                return NotFound();
            }

            return View(company);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else 
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyToDelete = _unitOfWork.Company.Get(id);
            if (companyToDelete is not null)
            {
                _unitOfWork.Company.Remove(companyToDelete);
                _unitOfWork.Save();
                return Json( new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll();

            return Json(new { data = companies });
        }

        #endregion
    }
}
