using System.ComponentModel.DataAnnotations;

namespace PostOfficeAPI.DTOs
{
    public class ClientRegistrationDTO
    {
        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Фамилия максимум 20 символов")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Фамилия должна содержать только русские буквы")]
        public string lastname { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Имя максимум 20 символов")]
        [RegularExpression(@"^[А-Яа-яЁё]+$", ErrorMessage = "Имя должно содержать только русские буквы")]
        public string name { get; set; }

        [StringLength(20, ErrorMessage = "Отчество максимум 20 символов")]
        [RegularExpression(@"^[А-Яа-яЁё]*$", ErrorMessage = "Отчество должно содержать только русские буквы")]
        public string? patronymic { get; set; }

        [Required(ErrorMessage = "Адрес обязателен")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Адрес максимум 50 символов")]
        [RegularExpression(@"^[А-Яа-яЁё0-9\s,\.\-]+$", ErrorMessage = "Адрес может содержать только русские буквы")]
        public string address { get; set; }

        [Required(ErrorMessage = "Телефон обязателен")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Телефон должен состоять ровно из 11 цифр")]
        public string phone { get; set; }
    }

    public class ClientUpdateDTO
    {
        [StringLength(50, ErrorMessage = "Адрес максимум 50 символов")]
        [RegularExpression(@"^[А-Яа-яЁё0-9\s,\.\-]+$", ErrorMessage = "Адрес может содержать только русские буквы")]
        public string? address { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Телефон должен состоять ровно из 11 цифр")]
        public string? phone { get; set; }
    }

    public class ClientResponseDTO
    {
        public int id { get; set; }
        public string lastname { get; set; }
        public string name { get; set; }
        public string? patronymic { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
    }
}