using BookStore.Data.Data;
using BookStore.Data.Models;
using BookStore.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var objFromDb = _db.Categories.FirstOrDefault(s => s.CategoryId == category.CategoryId);
            if (objFromDb != null)
            {
                objFromDb.Name = category.Name;
            }
        }
    }
}
