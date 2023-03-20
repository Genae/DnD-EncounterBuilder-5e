using Compendium.Database;
using Compendium.Provider;
using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.Controllers
{
    [Produces("application/json")]
    public abstract class StandardController<T> : Controller where T : KeyedDocument
    {
        protected readonly Provider<T> provider;
        private readonly DynamicEnumProvider dep;

        public StandardController(Provider<T> provider, DynamicEnumProvider dep)
        {
            this.provider = provider;
            this.dep = dep;
        }

        [HttpGet]
        [Route("")]
        public virtual List<T> GetAll()
        {
            return provider.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public T GetById(string id)
        {
            if (id == "0")
            {
                var obj = Activator.CreateInstance<T>();
                obj.Init(dep);
                return obj;
            }
            var objId = new ObjectId(id);
            return provider.Get(objId);
        }

        [HttpPost]
        [Route("")]
        public T Store([FromBody] T obj)
        {
            return provider.Store(obj);
        }

        [HttpDelete]
        [Route("{id}")]
        public void DeleteProject(string id)
        {
            var obj = GetById(id);
            provider.Delete(obj);
        }

        [HttpPost]
        [Route("fromIds")]
        public List<T> GetAllMonsters([FromBody] string[] ids)
        {
            return provider.Get(o => ids.Contains(o.Id.ToString()));
        }
    }

}
