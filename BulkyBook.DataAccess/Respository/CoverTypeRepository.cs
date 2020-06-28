using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Respository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public CoverTypeRepository(ApplicationDbContext db):base (db)
        {
            //must have :base because Repository.cs has a db object
            _db = db;
        }
        public void Update(CoverType coverType)
        {
            //using linq to retrieve first or default ID, for generic entity s, the Id should match
            //the cover type ID being passed, for every match, retrieve the first one of them.
            //if not Found return null
            var objFromDb = _db.CoverTypes.FirstOrDefault(s => s.Id == coverType.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = coverType.Name;
            }
        }
    }
}
