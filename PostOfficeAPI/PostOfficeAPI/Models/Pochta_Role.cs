using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Role
    {
        public int id {  get; set; }
        public string name { get; set; }

        public virtual ICollection<Pochta_Users> Pochta_Users { get; set; }
    }
}
