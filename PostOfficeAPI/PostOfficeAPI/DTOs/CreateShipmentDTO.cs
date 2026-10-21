using System.ComponentModel.DataAnnotations;

namespace PostOfficeAPI.DTOs
{
    public class CreateShipmentDTO
    {
        [Required(ErrorMessage = "Тип отправления обязателен")]
        [RegularExpression(@"^(Письмо|Ценное письмо|Посылка|Ценная посылка|Бандероль|Ценная бандероль)$",
            ErrorMessage = "Недопустимый тип отправления")]
        public string shipmentType { get; set; }

        [Required(ErrorMessage = "Вес обязателен")]
        [Range(1, 99999, ErrorMessage = "Вес должен быть от 1 до 99999 грамм")]
        public int weight { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Некорректная объявленная ценность")]
        public decimal? declaredValue { get; set; }

        [Required(ErrorMessage = "Телефон отправителя обязателен")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Телефон отправителя должен состоять из 11 цифр")]
        public string senderPhone { get; set; }

        [Required(ErrorMessage = "Телефон получателя обязателен")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Телефон получателя должен состоять из 11 цифр")]
        public string receiverPhone { get; set; }
    }

    public class ShipmentCreatedResponseDTO
    {
        public string trackNumber { get; set; }
        public string tariffName { get; set; }
        public decimal finalCost { get; set; }
        public string message { get; set; }
    }
}