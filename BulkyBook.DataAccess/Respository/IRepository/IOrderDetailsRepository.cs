using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface IOrderDetailsRepository : IRepository<OrderDetails> //inherit all methods of IRepository
    {
        void Update(OrderDetails orderDetails); //add in update
    }
}
