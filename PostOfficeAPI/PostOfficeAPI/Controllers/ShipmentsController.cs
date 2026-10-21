using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostOfficeAPI.DTOs;
using PostOfficeAPI.Models;
using System.Text;

namespace PostOfficeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentsController : ControllerBase
    {
        private readonly PostOfficeContext _context;

        public ShipmentsController(PostOfficeContext context)
        {
            _context = context;
        }

        [HttpGet("track/{trackNumber}")]
        public async Task<ActionResult<ShipmentDTO>> GetShipmentByTrack(string trackNumber)
        {
            var shipment = await _context.Pochta_Shipments
                .Include(s => s.Pochta_Clients)       // Отправитель
                .Include(s => s.Pochta_Clients1)      // Получатель
                .Include(s => s.Pochta_Tarrifs)       // Тариф
                .Include(s => s.Pochta_Statuses)      // Статус
                .Include(s => s.Pochta_Employees)     // Сотрудник
                .FirstOrDefaultAsync(s => s.track_number == trackNumber);

            if (shipment == null)
            {
                return NotFound("Отправление не найдено");
            }

            var dto = new ShipmentDTO
            {
                track_number = shipment.track_number,
                tip = shipment.tip,
                fact_weight = shipment.fact_weight,
                sender = shipment.Pochta_Clients != null
                    ? $"{shipment.Pochta_Clients.lastname} {shipment.Pochta_Clients.name} {shipment.Pochta_Clients.patronymic}".Trim()
                    : "Неизвестно",
                receiver = shipment.Pochta_Clients1 != null
                    ? $"{shipment.Pochta_Clients1.lastname} {shipment.Pochta_Clients1.name} {shipment.Pochta_Clients1.patronymic}".Trim()
                    : "Неизвестно",
                date_priema = shipment.date_priema,
                tariff = shipment.Pochta_Tarrifs != null ? shipment.Pochta_Tarrifs.name : "Неизвестно",
                cost = shipment.cost,
                status = shipment.Pochta_Statuses != null ? shipment.Pochta_Statuses.name : "Неизвестно",
                employee = shipment.Pochta_Employees != null
                    ? $"{shipment.Pochta_Employees.lastname} {shipment.Pochta_Employees.name} {shipment.Pochta_Employees.patronymic}".Trim()
                    : "Неизвестно"
            };

            return Ok(dto);
        }

        // GET: api/shipments/phone/{phone}
        [HttpGet("phone/{phone}")]
        public async Task<ActionResult<IEnumerable<ShipmentBriefDTO>>> GetShipmentsByPhone(string phone)
        {
            var shipments = await _context.Pochta_Shipments
                .Include(s => s.Pochta_Clients1)  // Получатель
                .Include(s => s.Pochta_Statuses)  // Статус
                .Where(s => s.Pochta_Clients1 != null && s.Pochta_Clients1.phone == phone)
                .Select(s => new ShipmentBriefDTO
                {
                    track_number = s.track_number,
                    tip = s.tip,
                    date_priema = s.date_priema,
                    status = s.Pochta_Statuses != null ? s.Pochta_Statuses.name : "Неизвестно"
                })
                .ToListAsync();

            if (!shipments.Any())
            {
                return NotFound("Отправления для указанного телефона не найдены");
            }

            return Ok(shipments);
        }

        // GET: api/shipments/track/{trackNumber}/history
        [HttpGet("track/{trackNumber}/history")]
        public async Task<ActionResult<IEnumerable<StatusHistoryDTO>>> GetShipmentHistory(string trackNumber)
        {

            var shipment = await _context.Pochta_Shipments
                .FirstOrDefaultAsync(s => s.track_number == trackNumber);

            if (shipment == null)
            {
                return NotFound("Отправление не найдено");
            }


            var history = await _context.Pochta_StatusHistory
                .Include(h => h.Pochta_Statuses)
                .Where(h => h.shipment == shipment.id)
                .OrderBy(h => h.date_of_change)
                .Select(h => new StatusHistoryDTO
                {
                    status = h.Pochta_Statuses.name,
                    date_of_change = h.date_of_change
                })
                .ToListAsync();

            if (!history.Any())
            {
                return NotFound("История статусов не найдена");
            }

            return Ok(history);
        }

        // POST: api/shipments
        [HttpPost]
        public async Task<ActionResult<ShipmentCreatedResponseDTO>> CreateShipment(CreateShipmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Поиск отправителя и получателя по телефону
            var sender = await _context.Pochta_Clients.FirstOrDefaultAsync(c => c.phone == dto.senderPhone);
            if (sender == null)
                return BadRequest($"Отправитель с телефоном {dto.senderPhone} не найден в базе.");

            var receiver = await _context.Pochta_Clients.FirstOrDefaultAsync(c => c.phone == dto.receiverPhone);
            if (receiver == null)
                return BadRequest($"Получатель с телефоном {dto.receiverPhone} не найден в базе.");

            //Проверка объявленной ценности для ценных отправлений
            bool isValuable = dto.shipmentType.Contains("Ценн");
            if (isValuable)
            {
                if (!dto.declaredValue.HasValue || dto.declaredValue <= 0)
                    return BadRequest("Для ценного отправления необходимо указать объявленную ценность > 0");

                decimal maxLimit = dto.shipmentType switch
                {
                    "Ценное письмо" => 500000,
                    "Ценная посылка" => 100000,
                    "Ценная бандероль" => 10000,
                    _ => 0
                };

                if (dto.declaredValue > maxLimit)
                    return BadRequest($"Максимальная объявленная ценность для '{dto.shipmentType}' составляет {maxLimit} руб.");
            }
            else
            {
                //Если отправление не ценное, игнорируем введенную ценность
                dto.declaredValue = 0;
            }

            //Поиск подходящего тарифа
            var matchingTariff = await _context.Pochta_Tarrifs
                .Where(t => t.tip == dto.shipmentType && t.weight >= dto.weight)
                .OrderBy(t => t.weight)
                .FirstOrDefaultAsync();

            if (matchingTariff == null)
            {
                return BadRequest($"Не найден тариф для типа '{dto.shipmentType}' с весом {dto.weight} г. (возможно, превышен максимальный вес)");
            }

            //Расчет итоговой стоимости
            decimal finalCost = matchingTariff.cost;
            if (isValuable && dto.declaredValue > 0)
            {
                finalCost += dto.declaredValue.Value * 0.05m; // Наценка 5%
            }

            //Генерация уникального трек-номера
            string trackNumber = await GenerateUniqueTrackNumberAsync();

            // 7. Создание отправления
            int currentEmployeeId = 2;

            var newShipment = new Pochta_Shipments
            {
                track_number = trackNumber,
                tip = dto.shipmentType,
                fact_weight = dto.weight.ToString(),
                sender = sender.id,
                receiver = receiver.id,
                date_priema = DateTime.Today,
                tarif = matchingTariff.id,
                cost = finalCost,
                status = 1, // Статус "Принято"
                employee = currentEmployeeId
            };

            _context.Pochta_Shipments.Add(newShipment);
            await _context.SaveChangesAsync();

            //Создание записи в истории статусов
            var statusHistory = new Pochta_StatusHistory
            {
                shipment = newShipment.id,
                status = 1,
                date_of_change = DateTime.Today
            };

            _context.Pochta_StatusHistory.Add(statusHistory);
            await _context.SaveChangesAsync();

            //Возврат успешного ответа
            var response = new ShipmentCreatedResponseDTO
            {
                trackNumber = trackNumber,
                tariffName = matchingTariff.name,
                finalCost = finalCost,
                message = "Отправление успешно создано"
            };

            return CreatedAtAction(nameof(GetShipmentByTrack), new { trackNumber = trackNumber }, response);
        }

        private async Task<string> GenerateUniqueTrackNumberAsync()
        {
            Random rnd = new Random();
            string trackNumber;
            bool isUnique;

            do
            {
                var tn = new StringBuilder(14);
                for (int i = 0; i < 14; i++)
                    tn.Append(rnd.Next(0, 10));

                trackNumber = tn.ToString();

                isUnique = !await _context.Pochta_Shipments.AnyAsync(s => s.track_number == trackNumber);
            }
            while (!isUnique);

            return trackNumber;
        }

    }
}