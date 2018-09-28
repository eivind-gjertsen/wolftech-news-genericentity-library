using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace GenericEntityLibrary.Context
{
    public class GenericContext : DbContext
    {
        public GenericContext(string nameorconnectionstring)
            : base(nameorconnectionstring)
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<GenericContext>());
        }
    }

    public class GenericIdentityContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser
    {
        public GenericIdentityContext(string nameorconnectionstring)
            : base(nameorconnectionstring)
        {
            //Database.SetInitializer(new CreateDatabaseIfNotExists<GenericContext>());
        }
    }
}
