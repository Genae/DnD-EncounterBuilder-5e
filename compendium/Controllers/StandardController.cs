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

        public StandardController(Provider<T> provider)
        {
            this.provider = provider;
        }

        [HttpGet]
        [Route("")]
        public List<T> GetAll()
        {
            return provider.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public T GetById(string id)
        {
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
