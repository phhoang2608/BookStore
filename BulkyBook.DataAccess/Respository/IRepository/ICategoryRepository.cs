using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface ICategoryRepository : IRepositoryAsync<Category> //inherit all methods of IRepository
    {
        void Update(Category category); //add in update
    }
}
