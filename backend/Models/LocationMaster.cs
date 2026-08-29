using System;
using System.ComponentModel.DataAnnotations;

namespace ArthSetu.Models
{
    public class LocationMaster
    {
        [Key]
        public Guid LocationId { get; set; }
        public string StateOrUT { get; set; }
        public string District { get; set; }
        
        public string OfficialCode { get; set; }
        public bool ActiveStatus { get; set; }
        
        public Guid? ParentLocationId { get; set; }
    }
}
