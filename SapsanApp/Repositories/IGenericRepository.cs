using System.Threading.Tasks;

namespace SapsanApp.Repositories
{
    // Репозиторий
    public interface IGenericRepository<T, U> where T : class where U : struct
    {
        public Task Post(T entity);
        public Task<object> Get();
        public Task<T> GetById(U id);
        public Task Delete(U id);
        public Task Put(T entity);
    }
}
