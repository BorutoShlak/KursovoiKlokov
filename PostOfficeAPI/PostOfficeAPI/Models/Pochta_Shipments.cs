using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_Shipments
    {
        public int id {  get; set; }
        public string track_number { get; set; }
        public string tip {  get; set; }
        public string fact_weight { get; set; }
        public int sender {  get; set; }
        public int receiver { get; set; }
        public DateTime date_priema { get; set; }
        public int tarif { get; set; }
        public decimal cost { get; set; }
        public int status { get; set; }
        public int employee {  get; set; }
        public virtual Pochta_Clients Pochta_Clients { get; set; }
        public virtual Pochta_Clients Pochta_Clients1 { get; set; }
        public virtual Pochta_Tarrifs Pochta_Tarrifs { get; set; }
        public virtual Pochta_Employees Pochta_Employees { get; set; }
        public virtual Pochta_Statuses Pochta_Statuses { get; set; }
        public virtual ICollection<Pochta_StatusHistory> Pochta_StatusHistory { get; set; }
    }
}
