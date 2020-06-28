using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
            public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db); //must have this or else reference to NullException will pop up
            Company = new CompanyRepository(_db); //must have this or else reference to NullException will pop up
            CoverType = new CoverTypeRepository(_db); //must have this or else reference to NullException will pop up
            ApplicationUser = new ApplicationUserRepository(_db); //must have this or else reference to NullException will pop up
            OrderHeader = new OrderHeaderRepository(_db); //must have this or else reference to NullException will pop up
            OrderDetails = new OrderDetailsRepository(_db); //must have this or else reference to NullException will pop up
            ShoppingCart = new ShoppingCartRepository(_db); //must have this or else reference to NullException will pop up
            Product = new ProductRepository(_db); //must have this or else reference to NullException will pop up
            SP_Call = new SP_Call(_db); //must have this or else reference to NullException will pop up

        }

        public ICategoryRepository Category { get; private set; }
        public ICompanyRepository Company { get; private set; }

        public ICoverTypeRepository CoverType { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IProductRepository Product { get; private set; }
        public ISP_Call SP_Call { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            //save changes
            _db.SaveChanges();
        }
        
    }
}
