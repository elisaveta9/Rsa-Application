using Rsa_Application.Database.Entities.Base;

namespace Rsa_Application.Database
{
    internal interface IRepository<T> where T : Entity
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        T Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
