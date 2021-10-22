using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Models.DTOs;
using SapsanApp.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly SapsanContext _context;
        public SchedulesController(
            SapsanContext context
            )
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        [Route("groups/{centerId}")]
        public async Task<IActionResult> GetGroups(int centerId)
        {
            try
            {
                var response = _context.Groups.Where(x => x.CenterId == centerId).Select(x => new
                {
                    GroupName = x.Name,
                    AgeGroup = x.AgeGroup,
                    Capacity = x.Capacity,
                    LanguageName = x.Language.Name,
                    Level = x.Level,
                    TrainingProgramName = x.TrainingProgram.Name,
                    UserFirstName = x.User.FirstName,
                    UserLastName = x.User.LastName
                }).ToListAsync();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of groups by center ID by {User.Identity.Name}",
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
        [Route("trainers/{centerId}")]
        public async Task<IActionResult> GetUsers(int centerId)
        {
            if (_context.Centers.Find(centerId) == null)
                return NotFound();

            try
            {
                var response = _context.Users.Where(x => _context.CenterUsers
                .Where(x => x.CenterId == centerId)
                .Select(x => x.UserId)
                .ToList()
                .Contains(x.Id) && x.RoleId == 5)
                .ToList();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of trainers by center ID by {User.Identity.Name}",
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
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SchedulesDTO schedulesDTO)
        {
            if (schedulesDTO.From == null)
                schedulesDTO.From = DateTime.MinValue;

            if (schedulesDTO.To == null)
                schedulesDTO.To = DateTime.MaxValue;

            try
            {
                var _groups = _context.Groups.Where(x => x.CenterId == schedulesDTO.CenterId).Select(x => x.Id).ToList();

                if (schedulesDTO.TrainingProgramId != null)
                    _groups = _context.Groups.Where(x => _groups.Contains(x.Id) && x.TrainingProgramId == schedulesDTO.TrainingProgramId).Select(x => x.Id).ToList();

                if (schedulesDTO.TrainerId != null)
                    _groups = _context.Groups.Where(x => _groups.Contains(x.Id) && x.UserId == schedulesDTO.TrainerId).Select(x => x.Id).ToList();

                var response = _context.Schedules.Where(x => _groups.Contains(x.GroupId)).Where(x => x.Created >= schedulesDTO.From && x.Created <= schedulesDTO.To && x.CenterId == schedulesDTO.CenterId);

                if (schedulesDTO.GroupId != null)
                    response = response.Where(x => x.GroupId == schedulesDTO.GroupId);

                if (schedulesDTO.Date != null)
                    response = response.Where(x => x.Date == schedulesDTO.Date);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a schedules data by {User.Identity.Name}",
                    Type = "Post"
                });

                await _context.SaveChangesAsync();

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Posting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPost]
        [Route("schedules")]
        public async Task<IActionResult> Post([FromBody] Schedule model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Schedules.Add(new Schedule()
                {
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Date = model.Date,
                    Color = model.Color,
                    Icon = model.Icon,
                    Description = model.Description,
                    GroupId = model.GroupId,
                    UserId = _context.Users.FirstOrDefault(j => j.Email == User.Identity.Name).Id,
                    CenterId = model.CenterId
                });

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a schedules data by {User.Identity.Name}",
                    Type = "Post"
                });

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest(new { errorText = "Posting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Schedule model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Schedules.Update(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a schedules data by {User.Identity.Name}",
                    Type = "Put"
                });

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest(new { errorText = "Editing data has been failed!" });
            }
        }

        [EnableCors]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var model = _context.Schedules.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.Schedules.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a schedules data by {User.Identity.Name}",
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
