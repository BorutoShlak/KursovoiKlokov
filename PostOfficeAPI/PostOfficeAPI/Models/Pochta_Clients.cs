using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Clients
    {
        public int id {  get; set; }
        public string lastname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public string address { get; set; }
        public string phone { get; set; }


        public virtual ICollection<Pochta_Shipments> Pochta_Shipments { get; set; }
        public virtual ICollection<Pochta_Shipments> Pochta_Shipments1 { get; set; }
    }
}
