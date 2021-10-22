using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Supervizor, Administrator")]
    public class LogsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public LogsController(
            SapsanContext context
            )
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        [Route("watched")]
        public async Task<IActionResult> GetWatched(Guid userId, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            var response = await _context.Logs
                .Where(x => x.UserId == userId && from <= x.Created && x.Created <= to && x.Type == "Get")
                .OrderByDescending(x => x.Created)
                .AsNoTracking()
                .ToListAsync();

            return Ok(response);
        }

        [EnableCors]
        [HttpGet]
        public async Task<IActionResult> Get(Guid userId, DateTime from, DateTime to)
        {
            if (to.Year == 1)
                to = DateTime.MaxValue;

            var response = await _context.Logs
                .Where(x => x.UserId == userId && from <= x.Created && x.Created <= to)
                .OrderByDescending(x => x.Created)
                .AsNoTracking()
                .ToListAsync();

            return Ok(response);
        }
    }
}
