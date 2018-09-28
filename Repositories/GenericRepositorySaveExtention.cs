using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericEntityLibrary.Repositories
{
    public partial class GenericRepositorySaveExtention<T,C> : GenericRepository<T,C> where T : Entities.GenericEntity, new() where C : DbContext, new()
    {
        public override int Save()
        {
            List<Tuple<T, EntityState>> ChangedEntities = new List<Tuple<T, EntityState>>();
            foreach (var changeSet in _context.ChangeTracker.Entries())
            {
                if (changeSet.State == EntityState.Added || changeSet.State == EntityState.Modified || changeSet.State == EntityState.Deleted)
                {
                    var type = Type.GetType(changeSet.Entity.GetType().ToString());
                    if (type != null)
                    {
                        var t = changeSet.Entity.GetType();
                        var entity = (T)Convert.ChangeType(changeSet.Entity, type);
                        ChangedEntities.Add(new Tuple<T, EntityState>(entity, changeSet.State));
                        entity.OnSaveChanges(entity);
                    }
                }
            }
            var changes = _context.SaveChanges();
            foreach (var tuple in ChangedEntities)
                tuple.Item1.OnAfterSaved(tuple.Item1, tuple.Item2);
            return changes;
        }
    }
}
