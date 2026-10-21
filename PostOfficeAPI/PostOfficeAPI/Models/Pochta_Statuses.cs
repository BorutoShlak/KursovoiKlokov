using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Statuses
    {
        public int id { get; set; }
        public string name { get; set; }
        
        public virtual ICollection<Pochta_Shipments> Pochta_Shipments { get; set; }
        public virtual ICollection<Pochta_StatusHistory> Pochta_StatusHistory { get; set; }
    }
}
