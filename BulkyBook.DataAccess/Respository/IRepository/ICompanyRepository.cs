﻿using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company); //add in update
    }
}
