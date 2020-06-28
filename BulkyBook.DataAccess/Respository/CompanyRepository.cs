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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

        public void Update(Company company)
        {
            //EF
            _db.Update(company);



            //using linq to retrieve first or default ID, for generic entity s, the Id should match
            //the category ID being passed, for every match, retrieve the first one of them.
            //if not Found return null
            //var objFromDb = _db.Companies.FirstOrDefault(s => s.Id == company.Id);
            //if (objFromDb != null)
            //{
            //    remember to include all properties from model
            //    objFromDb.Name = company.Name;
            //    objFromDb.StreetAddress = company.StreetAddress;
            //    objFromDb.City = company.City;
            //    objFromDb.State = company.State;
            //    objFromDb.PostalCode = company.PostalCode;
            //    objFromDb.PhoneNumber = company.PhoneNumber;
            //    objFromDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
            //}
        }
    }
}
