using System;

namespace PostOfficeAPI.DTOs
{
    //для полной информации об отправлении
    public class ShipmentDTO
    {
        public string track_number { get; set; }
        public string tip { get; set; }
        public string fact_weight { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public DateTime date_priema { get; set; }
        public string tariff { get; set; }
        public decimal cost { get; set; }
        public string status { get; set; }
        public string employee { get; set; }
    }

    //для краткой информации (при поиске по телефону)
    public class ShipmentBriefDTO
    {
        public string track_number { get; set; }
        public string tip { get; set; }
        public DateTime date_priema { get; set; }
        public string status { get; set; }
    }

    // для истории статусов
    public class StatusHistoryDTO
    {
        public string status { get; set; }
        public DateTime date_of_change { get; set; }
    }
}