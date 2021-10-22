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
    // Апи Админ критерий
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class AdminCriteriaController : ControllerBase
    {
        private readonly SapsanContext _context;
        public AdminCriteriaController(
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
                var response = _context.AdminCriteria.Select(x => new
                {
                    Id = x.Id,
                    CenterId = x.CenterId,
                    CenterName = x.Center.Name,
                    TrainingProgramId = x.TrainingProgramId,
                    TrainingProgramName = x.TrainingProgram.Name,
                    MainCriterionId = x.MainCriterionId,
                    MainCriterionName = x.MainCriterion.Name,
                    SubCriterionId = x.SubCriterionId,
                    SubCriterionName = x.SubCriterion.Name,
                    Created = x.Created
                }).ToList();

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting an admin criteria data by {User.Identity.Name}",
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
            if (_context.AdminCriteria.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting an admin criteria data by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.AdminCriteria.Where(x => x.Id == id).Select(x => new
                {
                    Id = x.Id,
                    CenterId = x.CenterId,
                    CenterName = x.Center.Name,
                    TrainingProgramId = x.TrainingProgramId,
                    TrainingProgramName = x.TrainingProgram.Name,
                    MainCriterionId = x.MainCriterionId,
                    MainCriterionName = x.MainCriterion.Name,
                    SubCriterionId = x.SubCriterionId,
                    SubCriterionName = x.SubCriterion.Name,
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
        public async Task<IActionResult> Post([FromBody] AdminCriterion model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.Centers.FirstOrDefault(x => x.Id == model.CenterId) == null)
                return BadRequest(new { errorText = "Invalid CenterID!" });

            if (_context.TrainingPrograms.FirstOrDefault(x => x.Id == model.TrainingProgramId) == null)
                return BadRequest(new { errorText = "Invalid TrainingProgramID!" });

            if (_context.MainCriteria.FirstOrDefault(x => x.Id == model.MainCriterionId) == null)
                return BadRequest(new { errorText = "Invalid MainCriterionID!" });

            if (_context.SubCriteria.FirstOrDefault(x => x.Id == model.SubCriterionId) == null)
                return BadRequest(new { errorText = "Invalid SubCriterionID!" });

            try
            {
                _context.AdminCriteria.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted an admin criteria data by {User.Identity.Name}",
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
        public async Task<IActionResult> Put([FromBody] AdminCriterion model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (_context.AdminCriteria.AsNoTracking().FirstOrDefault(x => x.Id == model.Id) == null)
                return NotFound();

            try
            {
                _context.Entry(model).State = EntityState.Modified;

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited an admin criteria data by {User.Identity.Name}",
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
            var model = await _context.AdminCriteria.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.AdminCriteria.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed an admin criteria data by {User.Identity.Name}",
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
