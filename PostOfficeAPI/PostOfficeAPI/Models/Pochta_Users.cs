using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Users
    {
        public int id {  get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int role {  get; set; }
        public int employee {  get; set; }

        public virtual Pochta_Employees Pochta_Employees { get; set; }
        public virtual Pochta_Role Pochta_Role { get; set; }
    }
}
