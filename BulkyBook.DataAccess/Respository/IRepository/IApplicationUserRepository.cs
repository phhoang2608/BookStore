﻿using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser> //inherit all methods of IRepository
    {
    }
}
