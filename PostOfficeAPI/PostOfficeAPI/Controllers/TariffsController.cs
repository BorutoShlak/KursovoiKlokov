using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostOfficeAPI.Models;
using PostOfficeAPI.DTOs;

namespace PostOfficeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TariffsController : ControllerBase
    {
        private readonly PostOfficeContext _context;

        public TariffsController(PostOfficeContext context)
        {
            _context = context;
        }

        // GET: api/tariffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TariffDTO>>> GetTariffs()
        {
            var tariffs = await _context.Pochta_Tarrifs
                .Select(t => new TariffDTO
                {
                    id = t.id,
                    tip = t.tip,
                    weight = t.weight,
                    cost = t.cost,
                    name = t.name
                })
                .ToListAsync();

            return Ok(tariffs);
        }

        // GET: api/tariffs/id
        [HttpGet("{id}")]
        public async Task<ActionResult<TariffDTO>> GetTariff(int id)
        {
            var tariff = await _context.Pochta_Tarrifs
                .Where(t => t.id == id)
                .Select(t => new TariffDTO
                {
                    id = t.id,
                    tip = t.tip,
                    weight = t.weight,
                    cost = t.cost,
                    name = t.name
                })
                .FirstOrDefaultAsync();

            if (tariff == null)
            {
                return NotFound("Тариф не найден");
            }

            return Ok(tariff);
        }
    }
}