using Compendium.Database;
using LiteDB;
using System.Linq.Expressions;

namespace Compendium.Provider
{
    public class Provider<T> where T : KeyedDocument
    {
        protected readonly IDatabaseConnection _db;
        public Provider(IDatabaseConnection db)
        {
            _db = db;
        }

        public virtual List<T> GetAll()
        {
            return _db.GetQueryable<T>().ToList();
        }

        public virtual List<T> Get(Expression<Func<T, bool>> query)
        {
            return _db.GetQueryable<T>().Where(query).ToList();
        }

        public T? Get(ObjectId objectId)
        {
            return Get(o => o.Id.Equals(objectId)).FirstOrDefault();
        }

        public virtual T Store(T obj)
        {
            return _db.Store(obj);
        }

        public virtual bool Delete(T obj)
        {
            var old = Get(obj.Id);
            if (old == null)
                return false;
            _db.Remove(obj);
            return true;
        }
    }
}
