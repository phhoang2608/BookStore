using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Respository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BulkyBook.DataAccess.Respository
{
    public class Repository<T> : IRepository<T> where T : class //implement the IRepository interface (class of type IRepository)
    {
        private readonly ApplicationDbContext _db; //we need a DbContext and Internal DbSet of generic type T to implement interface repository
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if(includeProperties != null)
            {
                //if you have an entity with a Foreign Key reference, you want it to be eagerly loading
                //example: Product Catogory and Category ID needed to be load in a single load.
                //pass all tables name relate to the product into a string, separate by comma.
                foreach(var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp); //include all properties as a string separate by comma.
                }
            }
 
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                //if you have an entity with a Foreign Key reference, you want it to be eagerly loading
                //example: Product Catogory and Category ID needed to be load in a single load.
                //pass all tables name relate to the product into a string, separate by comma.
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp); //include all properties as a string separate by comma.
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(int id)
        {
            T entity = dbSet.Find(id);
            Remove(entity);

        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
