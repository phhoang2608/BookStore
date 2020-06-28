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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db): base(db)
        {   
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            //using linq to retrieve first or default ID, for generic entity s, the Id should match
            //the orderHeader ID being passed, for every match, retrieve the first one of them.
            //if not Found return null
            _db.Update(orderHeader);
        }
    }
}
