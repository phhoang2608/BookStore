using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader> //inherit all methods of IRepository
    {
        void Update(OrderHeader orderHeader); //add in update
    }
}
