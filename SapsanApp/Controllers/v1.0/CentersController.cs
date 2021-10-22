using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    // Апи центров
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CentersController : ControllerBase
    {
        private readonly SapsanContext _context;
        public CentersController(
            SapsanContext context)
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetCentersByUser()
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);

            if (user == null)
                return BadRequest("No users found");

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting centers data by authorized user {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                if (_context.CenterUsers.Where(x => x.UserId == user.Id).Select(y => y.CenterId).ToList().Count == 0 && user.RoleId != 1 && user.RoleId != 2)
                    return Ok(new string[] { });

                var _listOfCenters = _context.CenterUsers.Where(x => x.UserId == user.Id).Select(y => y.CenterId).ToList();

                if (user.RoleId == 1 || user.RoleId == 2)
                {
                    return Ok(_context.Centers.Select(x => new
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CountryId = x.CountryId,
                        CountryName = x.Country.Name,
                        CityId = x.CityId,
                        CityName = x.City.Name,
                        Address = x.Address,
                        Schedule = x.Schedule,
                        Phone = x.Phone,
                        Social = x.Social,
                        Royalti = x.Royalti,
                        Lump = x.Lump,
                        Square = x.Square,
                        Revenue = x.Revenue,
                        Debt = x.Debt,
                        Visits = x.Visits,
                        Paid = x.Paid,
                        Created = x.Created
                    }).ToList());
                }    
                else
                {
                    var centers = _context.CenterUsers.Where(x => x.UserId == user.Id).Select(y => y.CenterId).ToList();

                    return Ok(_context.Centers.Where(x => centers.Contains(x.Id)).Select(x => new
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CountryId = x.CountryId,
                        CountryName = x.Country.Name,
                        CityId = x.CityId,
                        CityName = x.City.Name,
                        Address = x.Address,
                        Schedule = x.Schedule,
                        Phone = x.Phone,
                        Social = x.Social,
                        Royalti = x.Royalti,
                        Lump = x.Lump,
                        Square = x.Square,
                        Revenue = x.Revenue,
                        Debt = x.Debt,
                        Visits = x.Visits,
                        Paid = x.Paid,
                        Created = x.Created,
                    }).ToList());
                }
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = _context.Centers.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    CityId = x.CityId,
                    CityName = x.City.Name,
                    Address = x.Address,
                    Schedule = x.Schedule,
                    Phone = x.Phone,
                    Social = x.Social,
                    Royalti = x.Royalti,
                    Lump = x.Lump,
                    Square = x.Square,
                    Revenue = x.Revenue,
                    Debt = x.Debt,
                    Visits = x.Visits,
                    Paid = x.Paid,
                    Created = x.Created
                }).ToList();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of centers by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

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
            if (_context.Centers.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a center by center ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Centers.Where(x => x.Id == id).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    CountryId = x.CountryId,
                    CountryName = x.Country.Name,
                    CityId = x.CityId,
                    CityName = x.City.Name,
                    Address = x.Address,
                    Schedule = x.Schedule,
                    Phone = x.Phone,
                    Social = x.Social,
                    Royalti = x.Royalti,
                    Lump = x.Lump,
                    Square = x.Square,
                    Revenue = x.Revenue,
                    Debt = x.Debt,
                    Visits = x.Visits,
                    Paid = x.Paid,
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
        public async Task<IActionResult> Post([FromBody] Center model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.Countries.FirstOrDefault(x => x.Id == model.CountryId) == null)
                return BadRequest(new { errorText = "Invalid CountryID!" });

            if (_context.Cities.FirstOrDefault(x => x.Id == model.CityId) == null)
                return BadRequest(new { errorText = "Invalid CityID!" });

            try
            {
                _context.Centers.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a centers data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] Center model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.Countries.FirstOrDefault(x => x.Id == model.CountryId) == null)
                return BadRequest(new { errorText = "Invalid CountryID!" });

            if (_context.Cities.FirstOrDefault(x => x.Id == model.CityId) == null)
                return BadRequest(new { errorText = "Invalid CityID!" });

            try
            {
                _context.Update(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a centers data by {User.Identity.Name}",
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
            var model = _context.Centers.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.Centers.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a centers data by {User.Identity.Name}",
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
