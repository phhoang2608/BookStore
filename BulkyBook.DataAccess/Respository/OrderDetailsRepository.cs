using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BulkyBook.DataAccess.Respository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailsRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails orderDetails)
        {
            //using linq to retrieve first or default ID, for generic entity s, the Id should match
            //the orderDetails ID being passed, for every match, retrieve the first one of them.
            //if not Found return null
            //var objFromDb = _db.Categories.FirstOrDefault(s => s.Id == orderDetails.Id);
            //if (objFromDb != null)
            //{
            //    objFromDb.Name = orderDetails.Name;
            //}

            _db.Update(orderDetails);
        }
    }
}
