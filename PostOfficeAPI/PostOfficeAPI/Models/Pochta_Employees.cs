using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Employees
    {
        public int id { get; set; }
        public string lastname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public int dolzhnost { get; set; }
        public string phone { get; set; }

        public virtual Pochta_Dolzhonst Pochta_Dolzhonst { get; set; }
        public virtual ICollection<Pochta_Shipments> Pochta_Shipments { get; set; }
        public virtual ICollection<Pochta_Users> Pochta_Users { get; set; }
    }
}
