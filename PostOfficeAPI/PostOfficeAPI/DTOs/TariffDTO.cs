namespace PostOfficeAPI.DTOs
{
    public class TariffDTO
    {
        public int id { get; set; }
        public string tip { get; set; }
        public int weight { get; set; }
        public decimal cost { get; set; }
        public string name { get; set; }
    }
}