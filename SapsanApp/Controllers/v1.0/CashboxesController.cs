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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CashboxesController : ControllerBase
    {
        private readonly SapsanContext _context;
        public CashboxesController(
            SapsanContext context
            )
        {
            _context = context;
        }

        // Начальный гет по центру вкладки "Выручка"
        [EnableCors]
        [HttpGet]
        [Route("cashboxes/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var response = new
                {
                    Students = _context.Students.
                    Where(x => x.CenterId == id).
                    Select(x => new { x.Id, x.FirstName, x.LastName }).
                    ToList(),
                    ItemTypes = _context.Retails.
                    Select(x => x.Type).
                    ToList(),
                    ItemNames = _context.Retails.
                    Include(x => x.Country).
                    ToList()
                };

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a cashboxes data by center by {User.Identity.Name}",
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

        // Выручка-Ритейл, получение названий товаров по типу товара
        [EnableCors]
        [HttpGet]
        [Route("{type}")]
        public async Task<IActionResult> Get(string type)
        {
            try
            {
                var response = new
                {
                    ItemNames = _context.Retails.Where(x => x.Type == type).Include(x => x.Country).ToList()
                };

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting retails data by type by {User.Identity.Name}",
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

        // Выручка-Программы, получение данных по ID студента
        [EnableCors]
        [HttpGet]
        [Route("students/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var student = _context.Students.FirstOrDefault(x => x.Id == id);

            if (student == null)
                return NotFound();

            try
            {
                var countryId = _context.Centers.Where(x => x.Id == student.CenterId).FirstOrDefault().CountryId;

                var response = new
                {
                    Balance = Math.Round(_context.Students.Find(id).Balance, 2),
                    Programs = await Task.Run(() =>
                    from c in _context.Students.Where(x => x.Id == id).ToList()
                    join b in _context.StudentGroups on c.Id equals b.StudentId
                    join a in _context.Groups.Include(x => x.TrainingProgram) on b.GroupId equals a.Id
                    select new
                    {
                        TrainingProgramId = a.TrainingProgramId,
                        TrainingProgram = a.TrainingProgram.Name,
                        Price = _context.Subjects.Where(x => x.Id == a.TrainingProgram.SubjectId).FirstOrDefault().Price,
                    }),
                    TrainingMaterials = _context.TrainingMaterials.Where(x => x.CountryId == countryId).ToList()
                };

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a cashbox programs data by student by {User.Identity.Name}",
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

        // Получение данных по ID центра
        [EnableCors]
        [HttpGet]
        [Route("centers/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a cashbox data by center by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.CashboxCenters.Where(x => x.CenterId == id);

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        // Расходы
        [EnableCors]
        [HttpPost]
        [Route("centers")]
        public async Task<IActionResult> Post([FromBody] CashboxCenter model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.Centers.FirstOrDefault(x => x.Id == model.CenterId) == null)
                return BadRequest(new { errorText = "Invalid CenterID!" });

            try
            {
                _context.CashboxCenters.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a cashbox expenses data by {User.Identity.Name}",
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

        // Выручка-Программы отправка формы
        [EnableCors]
        [HttpPost]
        [Route("programs")]
        public async Task<IActionResult> Post([FromBody] CashboxProgram model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.Students.FirstOrDefault(x => x.Id == model.StudentId) == null)
                return BadRequest(new { errorText = "Invalid StudentID!" });

            if (_context.Centers.FirstOrDefault(x => x.Id == model.CenterId) == null)
                return BadRequest(new { errorText = "Invalid CenterID!" });

            try
            {
                double trainingMaterialPrice = 0;
                //if (model.ProgramId != null)
                //    value += _context.Subjects.Where(x => x.Id == _context.ProgramEntities.Where(x => x.Id == model.ProgramId).FirstOrDefault().SubjectId).FirstOrDefault().Price;

                if (model.TrainingMaterialId != null)
                    trainingMaterialPrice += _context.TrainingMaterials.Where(x => x.Id == model.TrainingMaterialId).FirstOrDefault().Price;

                if (model.PaymentSum != null)
                    _context.Students.Find(model.StudentId).Balance += (double)model.PaymentSum - trainingMaterialPrice;

                _context.CashboxPrograms.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a cashbox programs data by {User.Identity.Name}",
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

        // Выручка-Ритейл отправка формы
        [EnableCors]
        [HttpPost]
        [Route("retails")]
        public async Task<IActionResult> Post([FromBody] CashboxRetail model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.Centers.FirstOrDefault(x => x.Id == model.CenterId) == null)
                return BadRequest(new { errorText = "Invalid CenterID!" });

            try
            {
                _context.CashboxRetails.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a cashbox retails data by {User.Identity.Name}",
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

        // Статистика
        [EnableCors]
        [HttpGet]
        [Route("statistics")]
        public async Task<IActionResult> Get(int centerId, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            try
            {
                double? _programSum = _context.CashboxPrograms.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(x => x.PaymentSum);
                double? _trainingSum = _context.CashboxPrograms.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(y => y.TrainingMaterial.Price);
                double? _retailSum = _context.CashboxRetails.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(x => x.PaymentSum);
                double? _expenseSum = _context.CashboxCenters.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(x => x.AnotherSum + x.CoachEducation + x.Communication + x.Transport + x.Marketing + x.Rent + x.Stock + x.Taxes + x.Utilities + x.WageFund);

                var _expenseDiag = new
                {
                    AnotherSum = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.AnotherSum),
                    CoachEducation = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.CoachEducation),
                    Communication = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Communication),
                    Marketing = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Marketing),
                    Rent = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Rent),
                    Stock = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Stock),
                    Taxes = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Taxes),
                    Transport = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Transport),
                    Utilities = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.Utilities),
                    WageFund = _context.CashboxCenters.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.WageFund)
                };

                var _revenue = new
                {
                    Programs = _context.CashboxPrograms.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.PaymentSum),
                    TrainingMaterial = _context.TrainingMaterials.Where(x => _context.CashboxPrograms.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Select(x => x.TrainingMaterialId).Contains(x.Id)).Sum(x => x.Price),
                    Retail = _context.CashboxRetails.Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).Sum(y => y.PaymentSum)
                };

                var _programs = from c in _context.TrainingPrograms
                                join b in _context.Tariffs on c.TariffId equals b.Id
                                join a in _context.Subjects on c.SubjectId equals a.Id
                                select new
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    Tariff = b.Name,
                                    Subject = a.Name,
                                    Currency = b.Currency,
                                    BasePrice = a.Price,
                                    PaymentSum = _context.CashboxPrograms.Where(x => x.TrainingProgramId == c.Id).Sum(y => y.PaymentSum),
                                    //Discount = d.Procent * a.Price / 100,
                                    //PriceWithDiscount = a.Price - d.Procent * a.Price / 100,
                                    LessonCount = a.LessonsCount
                                };

                List<int?> _listProgramIds = new List<int?>();
                var _listProgramIdsVar = _context.CashboxPrograms.
                    Where(x => x.Created >= from && x.Created <= to && x.Created <= to && x.CenterId == centerId).
                    Select(x => x.TrainingProgramId);

                foreach (var item in _listProgramIdsVar)
                {
                    _listProgramIds.Add(item);
                }

                double? _programSum2 = 0;
                foreach (var item in _listProgramIds)
                {
                    int _subjectId = _context.TrainingPrograms.Where(x => x.Id == item).FirstOrDefault().SubjectId;

                    _programSum2 += _context.Subjects.Where(x => x.Id == _subjectId).FirstOrDefault().Price;
                }

                double? _trainingSum2 = _context.CashboxPrograms.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(y => y.TrainingMaterial.Price);
                double? _retailSum2 = _context.CashboxRetails.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(x => x.PaymentSum);
                double? _expenseSum2 = _context.CashboxCenters.
                    Where(x => x.Created >= from && x.Created <= to && x.CenterId == centerId).
                    Sum(x => x.AnotherSum + x.CoachEducation + x.Communication + x.Marketing + x.Rent + x.Stock + x.Taxes + x.Utilities + x.WageFund);

                double? _saldoStart = _programSum + _trainingSum + _retailSum - _expenseSum;
                double? _saldoEnd = _programSum2 + _trainingSum2 + _retailSum2 - _expenseSum2;

                var response = await Task.Run(() => new
                {
                    SaldoStart = _saldoStart,
                    SaldoEnd = _saldoEnd,
                    ProgramsSum = _programSum2,
                    TrainingsSum = _trainingSum2,
                    RetailsSum = _retailSum2,
                    ExpenseSum = _expenseSum2,
                    ExpensesDiagram = _expenseDiag,
                    RevenueDiagram = _revenue,
                    ProgramsDiagrams = _programs
                });

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a cashbox statistics data by {User.Identity.Name}",
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

        // История операций
        [EnableCors]
        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> GetHistory(int id, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            try
            {
                var _programs = _context.CashboxPrograms
                    .Where(x => x.Created >= from && x.Created <= to && x.CenterId == id)
                    .OrderByDescending(x => x.Created)
                    .Include(y => y.Student)
                    .Include(y => y.TrainingProgram)
                    .Include(y => y.TrainingMaterial)
                    .Select(z => new
                    {
                        Student = z.Student.FirstName + z.Student.LastName,
                        PaymentType = z.PaymentType,
                        TrainingProgram = z.TrainingProgram.Name,
                        TrainingMaterial = z.TrainingMaterial.Name,
                        PaymentSum = z.PaymentSum,
                        Created = z.Created
                    });

                var _retails = _context.CashboxRetails
                    .Where(x => x.Created >= from && x.Created <= to && x.CenterId == id)
                    .OrderByDescending(x => x.Created)
                    .ToList();

                var _expenses = _context.CashboxCenters
                    .Where(x => x.Created >= from && x.Created <= to && x.CenterId == id)
                    .OrderByDescending(x => x.Created)
                    .ToList();

                var response = await Task.Run(() => new
                {
                    Programs = _programs,
                    Retails = _retails,
                    Expenses = _expenses
                });

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a cashbox history data by {User.Identity.Name}",
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
