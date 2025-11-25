using ecommerce.DataAccess.Data;
using ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ecommerce.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();  //mesela T product ise dbSet otomatik olarak _db.Products olur
            _db.Products.Include(u=> u.Category); //product ile category i join et(product gelir + category de gelir)

        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false) // Get(x=> x.Id == id) istediğimiz şeye göre filtreleme yapabiliriz(id,name)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query= dbSet;
               
            }else
            {
                query = dbSet.AsNoTracking();
            }
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            
            return query.FirstOrDefault();

        }

        public IEnumerable<T> GetAll(string? includeProperties=null)
        {
            IQueryable<T> query = dbSet;
            
            if(!string.IsNullOrEmpty(includeProperties))
            {
               foreach(var includeProp in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
           dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities) //birden fazla ürün silmek için vs. kullanabiliriz
        {
            dbSet.RemoveRange(entities);
        }
    }
}
