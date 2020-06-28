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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            //using linq to retrieve first or default ID, for generic entity s, the Id should match
            //the category ID being passed, for every match, retrieve the first one of them.
            //if not Found return null
            _db.Update(shoppingCart);
        }
    }
}
