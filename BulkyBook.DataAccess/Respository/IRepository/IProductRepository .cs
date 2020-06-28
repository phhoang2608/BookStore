using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface IProductRepository : IRepository<Product> //inherit all methods of IRepository
    {
        void Update(Product product); //add in update
    }
}
