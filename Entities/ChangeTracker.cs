using GenericEntityLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericApiLibrary.Entities
{
    public class ChangeTracker<T> where T : GenericEntity
    {
        public T Entity { get; set; }
        public EntityState EntityState { get; set; }
        public DbPropertyValues OriginalValues { get; set; }
        public DbPropertyValues CurrentValues { get; set; }
    }
}
