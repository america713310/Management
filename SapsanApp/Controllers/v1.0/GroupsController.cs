using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    // Апи групп
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public GroupsController(
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
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of groups by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Groups.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name == null ? "" : x.Name,
                    TrainingProgramId = x.TrainingProgramId,
                    TrainingProgramName = x.TrainingProgram.Name,
                    UserId = x.UserId,
                    UserFirstName = x.User.FirstName,
                    UserLastName = x.User.LastName,
                    UserRole = x.User.Role.Name,
                    Level = x.Level == null ? "" : x.Level,
                    Capacity = x.Capacity == null ? 0 : x.Capacity,
                    LanguageId = x.LanguageId,
                    LanguageName = x.Language.Name,
                    AgeGroup = x.AgeGroup == null ? "" : x.AgeGroup,
                    CenterId = x.CenterId,
                    CenterName = x.Center.Name,
                    Students = x.StudentGroups.ToList().Select(y => new
                    {
                        StudentId = y.StudentId,
                        StudentFirstName = y.Student.FirstName == null ? "" : y.Student.FirstName,
                        StudentLastName = y.Student.LastName == null ? "" : y.Student.LastName,
                        StudentAge =  DateTime.Today.Year - y.Student.Birthday.Value.Year
                    })
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
            if (_context.Groups.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a group data by group ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Groups.Where(x => x.Id == id).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name == null ? "" : x.Name,
                    TrainingProgramId = x.TrainingProgramId,
                    TrainingProgramName = x.TrainingProgram.Name,
                    UserId = x.UserId,
                    UserFirstName = x.User.FirstName,
                    UserLastName = x.User.LastName,
                    UserRole = x.User.Role.Name,
                    Level = x.Level == null ? "" : x.Level,
                    Capacity = x.Capacity == null ? 0 : x.Capacity,
                    LanguageId = x.LanguageId,
                    LanguageName = x.Language.Name,
                    AgeGroup = x.AgeGroup == null ? "" : x.AgeGroup,
                    CenterId = x.CenterId,
                    CenterName = x.Center.Name,
                    Students = x.StudentGroups.ToList().Select(y => new
                    {
                        StudentId = y.StudentId,
                        StudentFirstName = y.Student.FirstName == null ? "" : y.Student.FirstName,
                        StudentLastName = y.Student.LastName == null ? "" : y.Student.LastName,
                        StudentAge = DateTime.Today.Year - y.Student.Birthday.Value.Year
                    })
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
        public async Task<IActionResult> Post([FromBody] Group model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Groups.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a groups data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] Group model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.StudentGroups.RemoveRange(_context.StudentGroups
                    .Where(x => x.GroupId == model.Id)
                    .ToList());

                await _context.SaveChangesAsync();

                _context.Update(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a groups data by {User.Identity.Name}",
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
            var model = _context.Groups.Include(x => x.StudentGroups).FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.Groups.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a groups data by {User.Identity.Name}",
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