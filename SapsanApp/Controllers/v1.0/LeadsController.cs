using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Enums;
using SapsanApp.Helpers;
using SapsanApp.Models;
using SapsanApp.Models.DTOs;
using SapsanApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class LeadsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public LeadsController(
            SapsanContext context
            )
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get(int? centerId, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of leads by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                if (centerId == null)
                    return Ok(_context.Leads.Where(x => from <= x.Created && x.Created <= to)
                        .AsNoTracking()
                        .Select(x => new
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Phone = x.Phone,
                            CenterId = x.CenterId,
                            CenterName = x.Center.Name,
                            TrainingProgramId = x.TrainingProgramId,
                            TrainingProgramName = x.TrainingProgram.Name,
                            ChildFirstName = x.ChildFirstName,
                            ChildLastName = x.ChildLastName,
                            ChildAgeGroup = x.ChildAgeGroup,
                            SourceId = x.SourceId,
                            SourceName = x.Source.Name,
                            Status = x.Status,
                            Created = x.Created
                        }).ToList());

                return Ok(_context.Leads.Where(x => x.CenterId == centerId && from <= x.Created && x.Created <= to)
                        .AsNoTracking()
                        .Select(x => new
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Phone = x.Phone,
                            CenterId = x.CenterId,
                            CenterName = x.Center.Name,
                            TrainingProgramId = x.TrainingProgramId,
                            TrainingProgramName = x.TrainingProgram.Name,
                            ChildFirstName = x.ChildFirstName,
                            ChildLastName = x.ChildLastName,
                            ChildAgeGroup = x.ChildAgeGroup,
                            SourceId = x.SourceId,
                            SourceName = x.Source.Name,
                            Status = x.Status,
                            Created = x.Created
                        }).ToList());
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
            if (_context.Leads.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                var response = _context.Leads.Where(x => x.Id == id)
                        .AsNoTracking()
                        .Select(x => new
                        {
                            Id = x.Id,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            Phone = x.Phone,
                            CenterId = x.CenterId,
                            CenterName = x.Center.Name,
                            TrainingProgramId = x.TrainingProgramId,
                            TrainingProgramName = x.TrainingProgram.Name,
                            ChildFirstName = x.ChildFirstName,
                            ChildLastName = x.ChildLastName,
                            ChildAgeGroup = x.ChildAgeGroup,
                            SourceId = x.SourceId,
                            SourceName = x.Source.Name,
                            Status = x.Status,
                            Created = x.Created
                        }).FirstOrDefault();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a lead data by ID by {User.Identity.Name}",
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
        [Route("statistics")]
        public async Task<IActionResult> GetStatistics(int centerId, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting leads statistics by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                List<StatisticsDTO> _listSource = new List<StatisticsDTO>();
                List<StatisticsDTO> _listStatus = new List<StatisticsDTO>();
                List<StatisticsDTO> _listPrograms = new List<StatisticsDTO>();

                var _listOfSources = _context.Sources.Distinct().ToList();
                var _listOfLeads = _context.Leads.Select(x => x.Status).Distinct().ToList();
                var _listOfPrograms = _context.TrainingPrograms.Distinct().ToList();

                foreach (var item in _listOfSources)
                    _listSource.Add(new StatisticsDTO() { Name = item.Name, Count = _context.Leads.Where(x => x.CenterId == centerId && x.SourceId == item.Id && x.Created >= from && x.Created <= to).Count() });

                double _count = _context.Leads.Where(x => x.CenterId == centerId && x.Created >= from && x.Created <= to).Count() + 0.00001;
                foreach (var item in _listOfLeads)
                {
                    LeadStatusEnum status = item;
                    int _someCount = _context.Leads.Where(x => x.CenterId == centerId && x.Status == item && x.Created >= from && x.Created <= to).Count() * 100;
                    _listStatus.Add(new StatisticsDTO() { Name = status.ToString(), Key = (int)item, Count = (int)(_someCount / _count) });
                }

                for (int i = 1; i <= Enum.GetNames(typeof(LeadStatusEnum)).Length; i++)
                {
                    LeadStatusEnum status = (LeadStatusEnum)i;
                    if (!_listStatus.Select(x => x.Key).Contains(i))
                        _listStatus.Add(new StatisticsDTO() { Name = status.ToString(), Key = i, Count = 0 });
                }
                _listStatus.Sort((x, y) => x.Key.CompareTo(y.Key));

                foreach (var item in _listOfPrograms)
                    _listPrograms.Add(new StatisticsDTO() { Name = item.Name, Count = _context.Leads.Where(x => x.CenterId == centerId && x.SourceId == item.Id && x.Created >= from && x.Created <= to).Count() });

                var response = await Task.Run(() => new { Sources = _listSource, Conversion = _listStatus, Programs = _listPrograms });

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Lead model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Leads.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a leads data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] Lead model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Update(model);

                if (model.Status == LeadStatusEnum.Invited)
                {
                    var student = new Student();

                    student.FirstName = model.ChildFirstName;
                    student.LastName = model.ChildLastName;
                    student.LeadId = model.Id;
                    student.LoginId = new string(Enumerable.
                        Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", 10).
                        Select(s => s[GlobalVariables.RANDOM.Next(s.Length)]).
                        ToArray());
                    student.ParentOneFirstName = model.FirstName;
                    student.ParentOneLastName = model.LastName;
                    student.ParentOnePhone = model.Phone;
                    student.CenterId = model.CenterId;
                    student.SourceId = model.SourceId;

                    await _context.Students.AddAsync(student);

                    _context.Logs.Add(new Log()
                    {
                        UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                        Description = $"Posted a students data after changed lead status to Invited by {User.Identity.Name}",
                        Type = "Post",
                        Created = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
                    });
                }

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a leads data by {User.Identity.Name}",
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
            var model = _context.Leads.FirstOrDefault(x => x.Id == id);

            if (_context.Leads.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Leads.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a leads data by {User.Identity.Name}",
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

        [EnableCors()]
        [HttpPost]
        [Route("excel")]
        public async Task<IActionResult> Excel(int centerId, IFormFile excelFile)
        {
            if (excelFile == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                var dtContent = FileHelpers.GetDataTableFromExcelLoadMode(excelFile);

                Lead lead = new Lead();

                foreach (DataRow dr in dtContent.Rows)
                {
                    lead.Id = Guid.NewGuid();
                    lead.FirstName = dr["FirstName"].ToString();
                    lead.LastName = dr["LastName"].ToString();
                    lead.Phone = dr["Phone"].ToString();
                    lead.CenterId = centerId;
                    lead.ChildFirstName = dr["ChildFirstName"].ToString();
                    lead.ChildLastName = dr["ChildLastName"].ToString();
                    lead.ChildAgeGroup = dr["ChildAge"].ToString();
                    //lead.SourceId = int.Parse(dr["SourceId"].ToString());
                    //lead.Status = (LeadStatusEnum)Enum.Parse(typeof(LeadStatusEnum), dr["Status"].ToString());
                    //lead.Comment = dr["Comment"].ToString();
                    //lead.RefusalReason = dr["RefusalReason"].ToString();

                    await _context.Leads.AddAsync(lead);

                    _context.Logs.Add(new Log()
                    {
                        UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                        Description = $"Posted by excel leads data by {User.Identity.Name}",
                        Type = "Post"
                    });

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch
            {
                return BadRequest(new { errorText = "Posting data from excel file has been failed!" });
            }
        }
    }
}
