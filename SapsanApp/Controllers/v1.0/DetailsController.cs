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
    // Апи деталей
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class DetailsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public DetailsController(SapsanContext context)
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get(Guid studentId, int trainingProgramId)
        {
            if (_context.Students.FirstOrDefault(x => x.Id == studentId) == null)
                return BadRequest(new { errorText = "Invalid StudentID!" });

            if (_context.TrainingPrograms.FirstOrDefault(x => x.Id == trainingProgramId) == null)
                return BadRequest(new { errorText = "Invalid TrainingProgramID!" });

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting details by student and program IDs by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var _list = _context.TrainingPrograms.Where(x => x.Id == trainingProgramId).Select(x => x.SubjectId);

                var response = new
                {
                    Program = _context.CashboxPrograms.Include(x => x.TrainingProgram).Where(x => x.StudentId == studentId && x.TrainingProgramId == trainingProgramId).FirstOrDefault().TrainingProgram.Name,
                    Sum = _context.Subjects.Where(x => _list.Contains(x.Id)).Select(x => x.Price).Sum(),
                    Check = _context.CashboxPrograms.Where(x => x.StudentId == studentId && x.TrainingProgramId == trainingProgramId).Average(x => x.PaymentSum),
                    LastDate = _context.CashboxPrograms.Where(x => x.StudentId == studentId && x.TrainingProgramId == trainingProgramId).OrderBy(x => x.Created).LastOrDefault().Created,
                    LastSum = _context.CashboxPrograms.Where(x => x.StudentId == studentId && x.TrainingProgramId == trainingProgramId).OrderBy(x => x.Created).LastOrDefault().PaymentSum,
                    Balance = _context.CashboxPrograms.Where(x => x.StudentId == studentId && x.TrainingProgramId == trainingProgramId).OrderBy(x => x.Created).LastOrDefault().Balance,
                    Deposit = _context.CashboxPrograms.Include(y => y.TrainingMaterial).Where(x => x.StudentId == studentId && x.TrainingProgramId == trainingProgramId).OrderBy(x => x.Created).LastOrDefault().TrainingMaterial.Price
                };

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }
    }
}
