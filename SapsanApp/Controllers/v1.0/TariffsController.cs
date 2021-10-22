using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using SapsanApp.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class TariffsController : ControllerBase
    {
        private readonly SapsanContext _context;
        private IGenericRepository<Tariff, int> _rep;
        public TariffsController(
            IGenericRepository<Tariff, int> rep,
            SapsanContext context
            )
        {
            _rep = rep;
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of tariffs by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Tariffs.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name == null ? "" : x.Country.Name,
                    CityId = x.CityId,
                    CityName = x.City.Name == null ? "" : x.City.Name,
                    Currency = x.Currency,
                    Created = x.Created
                }).ToList();

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (_context.Tariffs.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a tariffs data by ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Tariffs.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name == null ? "" : x.Country.Name,
                    CityId = x.CityId,
                    CityName = x.City.Name == null ? "" : x.City.Name,
                    Currency = x.Currency,
                    Created = x.Created
                }).FirstOrDefault();

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Tariff model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                await _rep.Post(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a tariffs data by {User.Identity.Name}",
                    Type = "Post"
                });

                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { errorText = "Posting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Tariff model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                await _rep.Put(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a tariffs data by {User.Identity.Name}",
                    Type = "Put"
                });

                await _context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { errorText = "Editing data has been failed!" });
            }
        }

        [EnableCors]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Tariffs.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                await _rep.Delete(id);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a tariffs data by {User.Identity.Name}",
                    Type = "Delete"
                });

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch
            {
                return BadRequest(new { errorText = "Deleting data has been failed!" });
            }
        }
    }
}
