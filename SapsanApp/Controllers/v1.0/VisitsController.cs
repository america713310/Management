using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    // Апи посещений
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class VisitsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public VisitsController(
            SapsanContext context
            )
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        [Route("groups/{centerId}")]
        public async Task<IActionResult> Get(int centerId)
        {
            _context.Logs.Add(new Log()
            {
                UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                Description = $"Getting a list of groups by center ID by {User.Identity.Name}",
                Type = "Get"
            });

            await _context.SaveChangesAsync();

            return Ok(await _context.Groups.Where(x => x.CenterId == centerId).ToListAsync());
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get(int groupId, DateTime from, DateTime to, int? type)
        {
            if (type == null)
                type = 1;

            try
            {
                int _month = from.Month;
                int _year = from.Year;
                Enums.VisitEnum enumType = (Enums.VisitEnum)type;

                foreach (var item in await _context.Students.
                    Where(x => _context.StudentGroups.
                    Where(x => x.GroupId == groupId).
                    Select(x => x.StudentId).
                    ToList().
                    Contains(x.Id)).
                    ToListAsync())
                {
                    if (_context.Visits.
                        Where(x => x.StudentId == item.Id).
                        Distinct().
                        ToList().
                        Exists(x => x.VisitDate.Month == from.Month && x.VisitDate.Year == from.Year && x.GroupId == groupId))
                        break;

                    for (int i = 1; i <= DateTime.DaysInMonth(from.Year, from.Month); i++)
                    {
                        _context.Update(new Visit()
                        {
                            StudentId = item.Id,
                            VisitDate = new DateTime(from.Year, from.Month, i).Date,
                            VisitValue = null,
                            GroupId = groupId
                        });
                        await _context.SaveChangesAsync();
                    }
                }

                var response = await Task.Run(() =>
                    from c in _context.Students
                    join b in _context.StudentGroups on c.Id equals b.StudentId
                    where b.GroupId == groupId
                    select new
                    {
                        StudentId = c.Id,
                        FIO = c.LastName + " " + c.FirstName + " " + c.MiddleName,
                        StudentStatus = b.StudentStatusEnum,
                        Balance = c.Balance - c.Balance * (_context.Discounts.FirstOrDefault(x => x.Id == b.DiscountId).Procent / 100),
                        Visits = _context.Visits.Where(x => x.GroupId == groupId && x.StudentId == c.Id && x.VisitDate.Month == _month && x.VisitDate.Year == _year).ToList(),
                        Count = _context.Visits.Where(x => x.GroupId == groupId && x.StudentId == c.Id && x.VisitDate.Month == _month && x.VisitDate.Year == _year).Where(x => x.VisitValue == enumType).Select(x => x.VisitValue).Count()
                    });

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
        public async Task<IActionResult> GetById(int id)
        {
            if (_context.Languages.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a visits data by ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                return Ok(await _context.Visits.FindAsync(id));
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Visit model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Visits.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a visits data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] Visit model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                if (model.VisitValue == Enums.VisitEnum.Lesson || model.VisitValue == Enums.VisitEnum.Omission || model.VisitValue == Enums.VisitEnum.WriteOffBalance)
                {
                    Student student = _context.Students.Where(x => x.Id == model.StudentId).FirstOrDefault();
                    int trainingProgramId = _context.Groups.Where(x => x.Id == model.GroupId).FirstOrDefault().TrainingProgramId;
                    int subjectId = _context.TrainingPrograms.Where(x => x.Id == trainingProgramId).FirstOrDefault().SubjectId;
                    double priceForLesson = _context.Subjects.Where(x => x.Id == subjectId).FirstOrDefault().Price / _context.Subjects.Where(x => x.Id == subjectId).FirstOrDefault().LessonsCount;
                    student.Balance -= priceForLesson;
                }

                Visit visit = _context.Visits.Where(x => x.StudentId == model.StudentId && x.VisitDate == model.VisitDate).FirstOrDefault();
                visit.VisitValue = model.VisitValue;

                _context.Update(visit);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a visits data by {User.Identity.Name}",
                    Type = "Put"
                });

                await _context.SaveChangesAsync();

                return Ok(visit);
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
            if (_context.Visits.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Visits.Remove(_context.Visits.Find(id));

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a visits data by {User.Identity.Name}",
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
        [Route("statistics")]
        public async Task<IActionResult> GetById(int centerId, Guid? trainerId, int? trainingProgramId, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            var _listOfGroupsByCenter = _context.Groups.Where(x => x.CenterId == centerId).Select(x => x.Id).ToList();
            List<int> _listOfGroups = new List<int>();
            int _visitsCount = 0;
            int _missesCount = 0;
            int _workingsCount = 0;
            int _studentsCount = 0;
            int _avgVisitsCount = 0;
            int _avgMissesCount = 0;
            int _avgWorkingsCount = 0;
            int _commonCount = 0;
            double _visitsProcent = 0;
            double _missesProcent = 0;
            double _workingsProcent = 0;
            int _giftsCount = 0;
            int _writeOffCount = 0;
            List<int> _list = new List<int>();
            int _maxLessons = 0;
            int _minLessons = 0;
            var _listOfIds = _context.Visits.Where(x => x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Select(x => x.StudentId).Distinct();
            List<Guid> _listOfGuidIds = new List<Guid>();

            if (trainerId != null && trainingProgramId != null)
            {
                _listOfGroups.AddRange(_context.Groups.Where(x => x.TrainingProgramId == trainingProgramId && x.UserId == trainerId).Select(x => x.Id).ToList());

                // Общее количество посещений
                _visitsCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.Lesson && x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Count();
                // Общее количество пропусков
                _missesCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.Omission && x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Count();
                // Общее количество отработок
                _workingsCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.WorkingOff && x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Count();
                // Общее количество учеников
                _studentsCount = _context.Visits.Where(x => x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Select(x => x.StudentId).Distinct().Count();
                
                if (_studentsCount != 0)
                {
                    // Среднее количество посещений на ученика
                    _avgVisitsCount = _visitsCount / _studentsCount;
                    // Среднее количество пропусков на ученика
                    _avgMissesCount = _missesCount / _studentsCount;
                    // Среднее количество отработок на ученика
                    _avgWorkingsCount = _workingsCount / _studentsCount;
                }
                else
                {
                    _avgVisitsCount = 0;
                    _avgMissesCount = 0;
                    _avgWorkingsCount = 0;
                }

                // Общее количество занятий
                _commonCount = _context.Visits.Where(x => x.VisitValue != null && x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Count();

                if (_commonCount != 0)
                {
                    // % посещений
                    _visitsProcent = _visitsCount / _commonCount * 100;
                    // % пропусков
                    _missesProcent = _missesCount / _commonCount * 100;
                    // % отработок
                    _workingsProcent = _workingsCount / _commonCount * 100;
                }
                else
                {
                    _visitsProcent = 0;
                    _missesProcent = 0;
                    _workingsProcent = 0;
                }
                
                // Общее количество подаренных уроков
                _giftsCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.GiftLesson && x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Count();
                // Общее количество списанных уроков
                _writeOffCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.WorkingOff && x.VisitDate >= from && x.VisitDate <= to && _listOfGroups.Contains(x.GroupId)).Count();

                // Максимальное количество посещений на одного ученика
                foreach (var item in _listOfIds)
                    _listOfGuidIds.Add(item);

                foreach (var item in _listOfGuidIds)
                {
                    int count = _context.Visits.Where(x => x.StudentId == item && x.VisitValue == Enums.VisitEnum.Lesson && _listOfGroupsByCenter.Contains(x.GroupId)).Count();
                    _list.Add(count);
                }

                _maxLessons = _list.Max();
                // Минимальное количество посещений на одного ученика
                _minLessons = _list.Min();
            }
            
            else
            {
                // Общее количество посещений
                _visitsCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.Lesson && x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Count();
                // Общее количество пропусков
                _missesCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.Omission && x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Count();
                // Общее количество отработок
                _workingsCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.WorkingOff && x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Count();
                // Общее количество учеников
                _studentsCount = _context.Visits.Where(x => x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Select(x => x.StudentId).Distinct().Count();
                
                if (_studentsCount != 0)
                {
                    // Среднее количество посещений на ученика
                    _avgVisitsCount = _visitsCount / _studentsCount;
                    // Среднее количество пропусков на ученика
                    _avgMissesCount = _missesCount / _studentsCount;
                    // Среднее количество отработок на ученика
                    _avgWorkingsCount = _workingsCount / _studentsCount;
                }
                else
                {
                    _avgVisitsCount = 0;
                    _avgMissesCount = 0;
                    _avgWorkingsCount = 0;
                }

                // Общее количество занятий
                _commonCount = _context.Visits.Where(x => x.VisitValue != null && x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Count();

                if (_commonCount != 0)
                {
                    // % посещений
                    _visitsProcent = _visitsCount / _commonCount * 100;
                    // % пропусков
                    _missesProcent = _missesCount / _commonCount * 100;
                    // % отработок
                    _workingsProcent = _workingsCount / _commonCount * 100;
                }
                else
                {
                    _visitsProcent = 0;
                    _missesProcent = 0;
                    _workingsProcent = 0;
                }
                // Общее количество подаренных уроков
                _giftsCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.GiftLesson && x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Count();
                // Общее количество списанных уроков
                _writeOffCount = _context.Visits.Where(x => x.VisitValue == Enums.VisitEnum.WorkingOff && x.VisitDate >= from && x.VisitDate <= to && _listOfGroupsByCenter.Contains(x.GroupId)).Count();

                if (_listOfIds.Count() != 0)
                {
                    // Максимальное количество посещений на одного ученика
                    foreach (var item in _listOfIds)
                        _listOfGuidIds.Add(item);

                    foreach (var item in _listOfGuidIds)
                    {
                        int count = _context.Visits.Where(x => x.StudentId == item && x.VisitValue == Enums.VisitEnum.Lesson && _listOfGroupsByCenter.Contains(x.GroupId)).Count();
                        _list.Add(count);
                    }

                    _maxLessons = _list.Max();
                    // Минимальное количество посещений на одного ученика
                    _minLessons = _list.Min();
                }
                else
                {
                    _maxLessons = 0;
                    _minLessons = 0;
                }
            }

            var response = new
            {
                VisitsCount = _visitsCount,
                MissesCount = _missesCount,
                WorkingCount = _workingsCount,
                StudentsCount = _studentsCount,
                AvgVisitsCount = _avgVisitsCount,
                AvgMissesCount = _avgMissesCount,
                AvgWorkingsCount = _avgWorkingsCount,
                VisitsProcent = _visitsProcent,
                MissesProcent = _missesProcent,
                WorkingProcent = _workingsProcent,
                GiftsCount = _giftsCount,
                WriteOffCount = _writeOffCount,
                MaxLessons = _maxLessons,
                MinLessons = _minLessons
            };

            _context.Logs.Add(new Log()
            {
                UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                Description = $"Getting visits statisitcs by {User.Identity.Name}",
                Type = "Get"
            });

            await _context.SaveChangesAsync();

            return Ok(response);
        }
    }
}
