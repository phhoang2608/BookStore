using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart> //inherit all methods of IRepository
    {
        void Update(ShoppingCart shoppingCart); //add in update
    }
}
