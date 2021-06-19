using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnet5_jwt_starter.Api.Repository
{
    public interface IRepository<T>
    {
        public Task<T> Create(T _object);

        public void Update(T _object);

        public IEnumerable<T> GetAll();

        public T GetByIdAsync(int Id);

        public void Delete(T _object);
    }
}
