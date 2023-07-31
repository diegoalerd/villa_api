using System.Linq.Expressions;
namespace MagicVilla_API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task Create(T entidad);

        Task <List<T>> ObtainAll(Expression<Func<T,bool>>? filtro = null);

        Task <T> Obtain(Expression<Func<T,bool>> filtro = null, bool tracked=true);

        Task Remove(T entidad);

        Task Record();
    }
}