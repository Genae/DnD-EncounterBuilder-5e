using Compendium.Models.ProjectData;
using Compendium.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.Controllers
{
    [Route("api/project")]
    public class ProjectController : StandardController<Project>
    {
        public ProjectController(ProjectProvider projectProvider, DynamicEnumProvider dep) : base(projectProvider, dep)
        { }
    }
}
