using compendium.Models.ProjectData;
using compendium.Provider;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace compendium.Controllers
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
        public Project GetAllMonsters(string id)
        {
            return _dataProvider.GetAllProjects().FirstOrDefault(m => m.Id.ToString().Equals(id));
        }

        [HttpPost]
        [Route("")]
        public Project CreateProject([FromBody] Project project)
        {
            return _dataProvider.CreateProject(project);// new Project { Name = name, Description = description });
        }
    }
}
