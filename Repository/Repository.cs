using System.Linq.Expressions;
using MagicVilla_API.Datos;
using Microsoft.EntityFrameworkCore;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Repository
{
    public class Repository<T>: IRepository<T> where T: class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public  Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task Create(T entidad)
        {
            await dbSet.AddAsync(entidad);
            await Record();
        }
        public async Task Record()
        {
            await _db.SaveChangesAsync();
        }
        public async Task <T> Obtain(Expression<Func<T,bool>> filtro = null, bool tracked=true)
        {
            IQueryable<T> query = dbSet;
            if(!tracked)
            {
                query = query.AsNoTracking();
            }
            if(filtro != null)
            {
                query = query.Where(filtro);
            }
            return await query.FirstOrDefaultAsync();
        }

       public async Task <List<T>> ObtainAll(Expression<Func<T,bool>>? filtro = null)
        {
            IQueryable<T> query = dbSet;
            if(filtro != null)
            {
                query = query.Where(filtro);
            }
            return await query.ToListAsync();
        }

        public async Task Remove(T entidad)
        {
            dbSet.Remove(entidad);
            await Record();

        }

    }
}