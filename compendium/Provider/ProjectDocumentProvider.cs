using Compendium.Database;

namespace Compendium.Provider
{
    public class ProjectDocumentProvider<T> : Provider<T> where T : ProjectKeyedDocument
    {
        private ProjectProvider _projectProvider;

        public ProjectDocumentProvider(IDatabaseConnection db) : base(db)
        { }

        public void RegisterProjectProvider(ProjectProvider projectProvider)
        {
            _projectProvider = projectProvider;
        }

        public override T Store(T obj)
        {
            base.Store(obj);
            if (obj.ProjectTags != null)
            {
                foreach (var proj in obj.ProjectTags)
                {
                    var project = _projectProvider.Get(proj);
                    if (project == null)
                        continue;
                    project.StoreItem(obj);
                    _projectProvider.Store(project);
                    var pDb = JsonDatabaseConnection.GetProjectDb(project.Name);
                    pDb.Store(obj);
                }
            }
            return obj;
        }
    }
}
