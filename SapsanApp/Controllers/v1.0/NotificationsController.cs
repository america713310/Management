using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapsanApp.Models;
using SapsanApp.Models.DTOs;
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
    public class NotificationsController : ControllerBase
    {
        private readonly SapsanContext _context;
        public NotificationsController(
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
                    Description = $"Getting a list of notifications by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = await _context.Notifications.ToListAsync();

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpGet]
        [Route("roles")]
        public async Task<IActionResult> GetByRoles()
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of notifications by roles by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Notifications.Where(x => x.RoleId != 0).Select(x => new { x.Name, x.Message, x.RoleId }).AsEnumerable().GroupBy(x => new { x.Name, x.RoleId }).Select(x => x.FirstOrDefault());

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpGet]
        [Route("centers")]
        public async Task<IActionResult> GetByCenters()
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of notifications by centers by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = _context.Notifications.Where(x => x.CenterId != 0).Select(x => new { x.Name, x.Message, x.CenterId }).AsEnumerable().GroupBy(x => new { x.Name, x.CenterId }).Select(x => x.FirstOrDefault());

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        // Отправка уведомлений всем
        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificationsDTO model)
        {
            try
            {
                if (model.Roles != null)
                {
                    foreach (var role in model.Roles)
                    {
                        var users = _context.Users.Where(x => x.RoleId == role).Select(x => x.Id).ToList();

                        foreach (var user in users)
                            _context.Notifications.Add(new Notification() { UserId = user, RoleId = role, Name = model.Name, Message = model.Message });

                        _context.Logs.Add(new Log()
                        {
                            UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                            Description = $"Posted a notifications data by roles by {User.Identity.Name}",
                            Type = "Post"
                        });

                        await _context.SaveChangesAsync();
                    }
                }

                if (model.Centers != null)
                {
                    foreach (var center in model.Centers)
                    {
                        var users = _context.CenterUsers.Where(x => model.Centers.Contains(x.CenterId)).Select(x => x.UserId).ToList();

                        foreach (var user in users)
                            _context.Notifications.Add(new Notification() { UserId = user, CenterId = center, Name = model.Name, Message = model.Message });

                        _context.Logs.Add(new Log()
                        {
                            UserId = _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                            Description = $"Posted a notifications data by centers by {User.Identity.Name}",
                            Type = "Post"
                        });

                        await _context.SaveChangesAsync();
                    }
                }

                var response = _context.Notifications.Where(x => x.Name == model.Name && x.Message == model.Message);

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Posting data has been failed!" });
            }
        }

        // Get info by token
        [EnableCors]
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of notifications by user ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = await _context.Notifications.Where(x => x.UserId == _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name).Id).ToListAsync();

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Notification model)
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a notifications data by {User.Identity.Name}",
                    Type = "Put"
                });

                _context.Update(model);

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
            var model = _context.Notifications.FirstOrDefault(x => x.Id == id);

            if (model == null)
                return NotFound();

            try
            {
                _context.Notifications.Remove(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a notifications data by {User.Identity.Name}",
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
