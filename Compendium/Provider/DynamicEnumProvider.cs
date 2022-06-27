using Compendium.Database;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Provider
{
    public partial class DynamicEnumProvider
    {
        private readonly IDatabaseConnection _db;

        public DynamicEnumProvider(IDatabaseConnection db)
        {
            _db = db;
            EnsureDefaultValues();
        }

        public DynamicEnum GetEnumValues(string name)
        {
            return _db.GetQueryable<DynamicEnum>().FirstOrDefault(d => d.Name.Equals(name));
        }

        public void EnsureDefaultValues()
        {
            foreach (var defaultEnum in DynamicEnumList.GetDefaults())
            {
                if (GetEnumValues(defaultEnum.Name) == null)
                    _db.Add(defaultEnum);
            }
        }
    }
}