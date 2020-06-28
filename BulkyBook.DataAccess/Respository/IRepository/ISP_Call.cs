using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Respository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        //implement Repository for Stored Procedure, name and parameters if any.
        //implement Dapper nugget package.
        T Single<T>(string procedureName, DynamicParameters param = null);
        //T Single return a boolean or integer value, best for first column of first row

        void Execute(string procedureName, DynamicParameters param = null);

        T OneRecord<T>(string procedureName, DynamicParameters param = null);
        //OneRecord returns an entire row

        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);
        //return an entire table

        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
        //returns multiples tables



    }
}
