using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICompanyRepository Company { get; }

        ICoverTypeRepository CoverType { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IProductRepository Product { get; }
        ISP_Call SP_Call { get; }

        IApplicationUserRepository ApplicationUser { get; }

        void Save();
    }
}
