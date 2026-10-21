using System.Collections.Generic;

namespace PostOfficeAPI.Models
{
    public class Pochta_StatusHistory
    {
        public int id { get; set; }
        public int shipment {  get; set; }
        public int status { get; set; }
        public DateTime date_of_change { get; set; }

        public virtual Pochta_Statuses Pochta_Statuses { get; set; }
        public virtual Pochta_Shipments Pochta_Shipments { get; set; }
    }
}
