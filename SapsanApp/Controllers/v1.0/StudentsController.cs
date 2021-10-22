using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Helpers;
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
    public class StudentsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public StudentsController(
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
                var response = _context.Students
                    .AsNoTracking().Select(s => new 
                {
                    Id = s.Id,
                    LoginId = s.LoginId,
                    LeadId = s.LeadId,
                    FirstName = s.FirstName,
                    MiddleName = s.MiddleName,
                    LastName = s.LastName,
                    Birthday = s.Birthday,
                    Address = s.Address,
                    CenterId = s.CenterId == null ? 0 : s.CenterId,
                    CenterName = s.Center.Name,
                    SourceId = s.SourceId == null ? 0 : s.SourceId,
                    SourceName = s.Source.Name,
                    SchoolName = s.SchoolName,
                    Balance = s.Balance,
                    Grade = s.Grade,
                    ParentOneFirstName = s.ParentOneFirstName,
                    ParentOneLastName = s.ParentOneLastName,
                    ParentOnePhone = s.ParentOnePhone,
                    ParentOneEmail = s.ParentOneEmail,
                    ParentTwoFirstName = s.ParentTwoFirstName,
                    ParentTwoLastName = s.ParentTwoLastName,
                    ParentTwoPhone = s.ParentTwoPhone,
                    ParentTwoEmail = s.ParentTwoEmail,
                    IsStudent = s.IsStudent,
                    Created = s.Created,
                    Groups = s.StudentGroups.Select(y => new {
                        GroupId = y.GroupId,
                        GroupName = y.Group.Name,
                        StudentStatus = y.StudentStatusEnum,
                        Level = y.Group.Level,
                        Capacity = y.Group.Capacity,
                        Language = y.Group.Language.Name,
                        AgeGroup = y.Group.AgeGroup,
                        TrainerFirstName = y.Group.User.FirstName,
                        TrainerLastName = y.Group.User.LastName,
                        TrainingProgramName = y.Group.TrainingProgram.Name,
                        DiscountId = y.DiscountId,
                        SubjectName = _context.Subjects.FirstOrDefault(x => x.Id == _context.TrainingPrograms.Where(z => z.Id == y.Group.TrainingProgramId).FirstOrDefault().SubjectId).Name,
                        TariffName = _context.Tariffs.FirstOrDefault(x => x.Id == _context.TrainingPrograms.Where(z => z.Id == y.Group.TrainingProgramId).FirstOrDefault().TariffId).Name
                    })
                }).ToList();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of students by {User.Identity.Name}",
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
        public async Task<IActionResult> GetById(Guid id)
        {
            if (_context.Students.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                var response = _context.Students
                    .Where(x => x.Id == id).AsNoTracking().Select(s => new
                    {
                        Id = s.Id,
                        LoginId = s.LoginId,
                        LeadId = s.LeadId,
                        FirstName = s.FirstName,
                        MiddleName = s.MiddleName,
                        LastName = s.LastName,
                        Birthday = s.Birthday,
                        Address = s.Address,
                        CenterId = s.CenterId == null ? 0 : s.CenterId,
                        CenterName = s.Center.Name,
                        SourceId = s.SourceId == null ? 0 : s.SourceId,
                        SourceName = s.Source.Name,
                        SchoolName = s.SchoolName,
                        Balance = s.Balance,
                        Grade = s.Grade,
                        ParentOneFirstName = s.ParentOneFirstName,
                        ParentOneLastName = s.ParentOneLastName,
                        ParentOnePhone = s.ParentOnePhone,
                        ParentOneEmail = s.ParentOneEmail,
                        ParentTwoFirstName = s.ParentTwoFirstName,
                        ParentTwoLastName = s.ParentTwoLastName,
                        ParentTwoPhone = s.ParentTwoPhone,
                        ParentTwoEmail = s.ParentTwoEmail,
                        IsStudent = s.IsStudent,
                        Created = s.Created,
                        Groups = s.StudentGroups.Select(y => new {
                            GroupId = y.GroupId,
                            GroupName = y.Group.Name,
                            StudentStatus = y.StudentStatusEnum,
                            Level = y.Group.Level,
                            Capacity = y.Group.Capacity,
                            Language = y.Group.Language.Name,
                            AgeGroup = y.Group.AgeGroup,
                            TrainerFirstName = y.Group.User.FirstName,
                            TrainerLastName = y.Group.User.LastName,
                            TrainingProgramName = y.Group.TrainingProgram.Name,
                            DiscountId = y.DiscountId,
                            SubjectName = _context.Subjects.FirstOrDefault(x => x.Id == _context.TrainingPrograms.Where(z => z.Id == y.Group.TrainingProgramId).FirstOrDefault().SubjectId).Name,
                            TariffName = _context.Tariffs.FirstOrDefault(x => x.Id == _context.TrainingPrograms.Where(z => z.Id == y.Group.TrainingProgramId).FirstOrDefault().TariffId).Name
                        })
                    }).FirstOrDefault();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a student by ID by {User.Identity.Name}",
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
        public async Task<IActionResult> Post([FromBody] Student model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                model.LoginId = new string(Enumerable.
                    Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", 10).
                    Select(s => s[GlobalVariables.RANDOM.Next(s.Length)]).
                    ToArray());

                _context.Students.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted students data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] Student model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.StudentGroups.
                RemoveRange(_context.StudentGroups.
                Where(x => x.StudentId == model.Id).
                ToList());

                _context.Update(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a students data by {User.Identity.Name}",
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
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = _context.Students.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.Students.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a students data by {User.Identity.Name}",
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

        [EnableCors]
        [HttpGet]
        [Route("visits/{id}")]
        public async Task<IActionResult> GetVisits(Guid id)
        {
            try
            {
                var response = new
                {
                    FirstLesson = _context.Visits.
                    Where(x => x.StudentId == id).
                    OrderBy(x => x.VisitDate).
                    FirstOrDefault().VisitDate,
                    LastLesson = _context.Visits.
                    Where(x => x.StudentId == id).
                    OrderBy(x => x.VisitDate).
                    LastOrDefault().VisitDate,
                    VisitsCount = _context.Visits.
                    Where(x => x.StudentId == id && x.VisitValue != null).
                    Count(),
                    VisitsPerMonth = _context.Visits.
                    Where(x => x.StudentId == id && x.VisitDate == _context.Visits.
                    Where(x => x.StudentId == id).
                    OrderBy(x => x.VisitDate).
                    FirstOrDefault().VisitDate && x.VisitValue != null)
                };

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a visits data by student ID by {User.Identity.Name}",
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
    }
}
