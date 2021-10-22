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
    public class CountriesController : ControllerBase
    {
        private readonly SapsanContext _context;
        private IGenericRepository<Country, int> _rep;
        public CountriesController(
            IGenericRepository<Country, int> rep,
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
                    Description = $"Getting a list of countries by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                return Ok(await _rep.Get());
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
            if (_context.Countries.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a country by country ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                return Ok(await _rep.GetById(id));
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Country model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                await _rep.Post(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a countries data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] Country model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                await _rep.Put(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a countries data by {User.Identity.Name}",
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
            if (_context.Countries.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                await _rep.Delete(id);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a countries data by {User.Identity.Name}",
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
