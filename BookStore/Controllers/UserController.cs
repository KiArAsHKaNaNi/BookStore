using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region Api Calls
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = _userManager.Users.Include(u => u.Company);
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault();
                if (user.Company is null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlockAsync([FromBody] string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (user.LockoutEnd is null || user.LockoutEnd <= DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            await _userManager.UpdateAsync(user);
            return Json(new { success = true, message = "Operation successful" });
        }

        #endregion
    }
}
