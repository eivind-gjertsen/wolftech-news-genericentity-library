using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Threading;

namespace GenericEntityLibrary.Entities
{
    public abstract class GenericEntity
    {
        public GenericEntity()
        {
            CreatedOn = ModifiedOn = DateTime.UtcNow;
            CreatedByName = ModifiedByName = Thread.CurrentPrincipal.Identity.Name;
        }
        [Key]
        public int OID { get; set; }
        [JsonIgnore]
        public string CreatedByName { get; set; }
        [JsonIgnore]
        public string ModifiedByName { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
