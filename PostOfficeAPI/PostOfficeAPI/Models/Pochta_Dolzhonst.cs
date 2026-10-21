using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Dolzhonst
    {
        public int id {  get; set; }
        public string name { get; set; }

        public virtual ICollection<Pochta_Employees> Pochta_Employees { get; set; }
    }
}
