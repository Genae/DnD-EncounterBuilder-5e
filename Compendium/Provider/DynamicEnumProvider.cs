using Compendium.Database;
using Compendium.Models.CoreData.Enums;

namespace Compendium.Provider
{
    public partial class DynamicEnumProvider : Provider<DynamicEnum>
    {
        public DynamicEnumProvider(IDatabaseConnection db) : base(db)
        {
            EnsureDefaultValues();
        }

        public DynamicEnum? GetEnumValues(string name)
        {
            return Get(d => d.Name.Equals(name)).FirstOrDefault();
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