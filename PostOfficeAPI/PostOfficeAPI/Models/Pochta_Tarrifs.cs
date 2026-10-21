using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Tarrifs
    {
        public int id {  get; set; }
        public string tip { get; set; }
        public int weight { get; set; }
        public decimal cost { get; set; }
        public string name { get; set; }
        public virtual ICollection<Pochta_Shipments> Pochta_Shipments { get; set; }
    }
}
