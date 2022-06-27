using compendiumOld.Models.ProjectData;
using compendiumOld.Provider;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace compendiumOld.Controllers
{
    [Produces("application/json")]
    [Route("api/project")]
    public class ProjectController : Controller
    {
        private readonly DataProvider _dataProvider;

        public ProjectController(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        [HttpGet]
        [Route("")]
        public IEnumerable<Project> GetAllProjects()
        {
            return _dataProvider.GetAllProjects();
        }

        [HttpGet]
        [Route("{id}")]
        public Project GetProject(string id)
        {
            return _dataProvider.GetAllProjects().FirstOrDefault(m => m.Id.ToString().Equals(id));
        }

        [HttpPost]
        [Route("")]
        public Project CreateProject([FromBody] Project project)
        {
            return _dataProvider.CreateProject(project);// new Project { Name = name, Description = description });
        }

        [HttpPost]
        [Route("{id}")]
        public Project EditProject([FromBody] Project project)
        {
            return _dataProvider.EditProject(project);// new Project { Name = name, Description = description });
        }

        [HttpDelete]
        [Route("{id}")]
        public void DeleteProject(string id)
        {
            _dataProvider.DeleteProject(GetProject(id));// new Project { Name = name, Description = description });
        }
    }
}
