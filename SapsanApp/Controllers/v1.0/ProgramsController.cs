using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Enums;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProgramsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public ProgramsController(
            SapsanContext context
            )
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = new
                {
                    Programs = from c in _context.TrainingPrograms
                               join b in _context.Tariffs on c.TariffId equals b.Id
                               join a in _context.Subjects on c.SubjectId equals a.Id
                               select new
                               {
                                   Id = c.Id,
                                   Name = c.Name,
                                   Tariff = b.Name,
                                   Subject = a.Name,
                                   CountryId = b.Country.Id,
                                   CountryName = b.Country.Name,
                                   CityId = b.CityId,
                                   CityName = b.City.Name,
                                   Currency = b.Currency,
                                   Comment = c.Comment,
                                   BasePrice = a.Price,
                                   //Discount = d.Procent * a.Price / 100,
                                   //PriceWithDiscount = a.Price - d.Procent * a.Price / 100,
                                   LessonCount = a.LessonsCount
                               },
                    Countries = _context.Countries.AsNoTracking().ToList(),
                    Cities = _context.Cities.AsNoTracking().ToList(),
                    Currencies = Enum.GetNames(typeof(CurrencyEnum)),
                    Subjects = _context.Subjects.AsNoTracking().ToList(),
                    Tariffs = _context.Tariffs.AsNoTracking().ToList(),
                    Discounts = _context.Discounts.AsNoTracking().ToList(),
                };

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of programs by {User.Identity.Name}",
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
        [Route("programs/{centerId}")]
        public async Task<IActionResult> GetProgramsByCenterId(int centerId)
        {
            if (_context.Centers.FirstOrDefault(x => x.Id == centerId) == null)
                return NotFound();

            try
            {
                var _list = _context.Groups
                .Where(x => x.CenterId == centerId)
                .Select(x => x.Id);

                var cityId = _context.Centers.Where(x => x.Id == centerId).FirstOrDefault().CityId;

                var _tariffIds = _context.Tariffs
                    .Where(x => x.CityId == cityId)
                    .Select(x => x.Id).ToList();

                var _listProgramIds = _context.Groups
                    .Where(x => _list.Contains(x.Id))
                    .Select(x => x.TrainingProgramId);

                var programs = _context.TrainingPrograms
                    .Where(x => /*_listProgramIds.Contains(x.Id) &&*/ _tariffIds.Contains(x.TariffId))
                    .Select(x => new 
                    { 
                        Id = x.Id, 
                        Name = x.Name,
                        TariffId = x.TariffId,
                        Tariff = x.Tariff.Name,
                        SubjectId = x.SubjectId,
                        Subject = x.Subject.Name,
                        CountryId = _context.Tariffs.Where(y => y.Id == x.TariffId).FirstOrDefault().CountryId,
                        CountryName = _context.Tariffs.Where(y => y.Id == x.TariffId).FirstOrDefault().Country.Name,
                        CityId = _context.Tariffs.Where(y => y.Id == x.TariffId).FirstOrDefault().CityId,
                        CityName = _context.Tariffs.Where(y => y.Id == x.TariffId).FirstOrDefault().City.Name,
                        Currency = _context.Tariffs.Where(y => y.Id == x.TariffId).FirstOrDefault().Currency,
                        Comment = x.Comment,
                        BasePrice = _context.Subjects.Where(y => y.Id == x.SubjectId).FirstOrDefault().Price,
                        LessonCount = _context.Subjects.Where(y => y.Id == x.SubjectId).FirstOrDefault().LessonsCount
                    })
                    .ToList();

                var response = new
                {
                    Programs = programs,
                    Countries = _context.Countries.AsNoTracking().ToList(),
                    Cities = _context.Cities.AsNoTracking().ToList(),
                    Currencies = Enum.GetNames(typeof(CurrencyEnum)),
                    Subjects = _context.Subjects.AsNoTracking().ToList(),
                    Tariffs = _context.Tariffs.AsNoTracking().ToList(),
                    Discounts = _context.Discounts.AsNoTracking().ToList(),
                };

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of programs by center ID by {User.Identity.Name}",
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
        [Route("regions/{cityId}")]
        public async Task<IActionResult> GetProgramsByCityId(int cityId)
        {
            try
            {
                var _tariffIds = _context.Tariffs
                    .Where(x => x.CityId == cityId)
                    .Select(x => x.Id).ToList();

                var response = _context.TrainingPrograms
                    .Where(x => _tariffIds.Contains(x.TariffId))
                    .Select(x => new { x.Id, x.Name });

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of programs by city ID by {User.Identity.Name}",
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
        [Route("simple/{centerId}")]
        public async Task<IActionResult> GetSimpleProgramsByCenterId(int centerId)
        {
            if (_context.Centers.FirstOrDefault(x => x.Id == centerId) == null)
                return NotFound();

            try
            {
                var cityId = _context.Centers.Where(x => x.Id == centerId).FirstOrDefault().CityId;

                var _tariffIds = _context.Tariffs
                    .Where(x => x.CityId == cityId)
                    .Select(x => x.Id).ToList();

                var response = _context.TrainingPrograms
                    .Where(x => _tariffIds.Contains(x.TariffId))
                    .Select(x => new { x.Id, x.Name });

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of programs by center ID by {User.Identity.Name}",
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
        [Route("trainers/{trainerId}")]
        public async Task<IActionResult> GetProgramsByTrainerId(Guid trainerId)
        {
            try
            {
                var _list = _context.Groups
                .Where(x => x.UserId == trainerId)
                .Select(x => x.TrainingProgramId)
                .ToList();

                var response = _context.TrainingPrograms
                    .Where(x => _list.Contains(x.Id))
                    .Select(x => new { x.Id, x.Name });

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of programs by trainer ID by {User.Identity.Name}",
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
            if (_context.TrainingPrograms.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a program by ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = await _context.TrainingPrograms.FindAsync(id);

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.TrainingPrograms.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a programs data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] TrainingProgram model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Update(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a programs data by {User.Identity.Name}",
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
            var model = _context.TrainingPrograms.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.TrainingPrograms.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a programs data by {User.Identity.Name}",
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
