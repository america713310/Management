using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SapsanApp.Helpers;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    // Апи пользователей
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json")
              .Build();
        private readonly SapsanContext _context;
        public UsersController(
            SapsanContext context
            )
        {
            _context = context;
        }

        [EnableCors]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> Get()
        {
            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of users by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = from c in _context.Users
                           select new
                           {
                               Id = c.Id,
                               FirstName = c.FirstName,
                               MiddleName = c.MiddleName,
                               LastName = c.LastName,
                               Avatar = c.Avatar,
                               Email = c.Email,
                               Password = c.Password,
                               RoleId = c.RoleId,
                               Role = c.Role.Description,
                               Birthday = c.Birthday,
                               Age = DateTime.Today.Year - c.Birthday.Year,
                               Phone = c.Phone,
                               IIN = c.IIN,
                               Address = c.Address,
                               WorkStatus = c.WorkStatus,
                               WorkExperience = c.WorkExperience,
                               Speciality = c.Speciality,
                               MaritalStatus = c.MaritalStatus,
                               HasChild = c.HasChild,
                               Created = c.Created,
                               LastEntry = c.LastEntry,
                               CheckNumber = c.CheckNumber,
                               CenterUsers = c.CenterUsers.Select(x => x.Center.Name).ToList(),
                               UserLanguages = c.UserLanguages.Select(x => x.Language.Name).ToList()
                           };

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        // Список пользователей по определенному центру
        [EnableCors]
        [HttpGet]
        [Route("{centerId}")]
        public async Task<IActionResult> Get(int centerId)
        {
            // Список айдиш пользователей по определенному центру
            var userIds = _context.CenterUsers.Where(x => x.CenterId == centerId).Select(y => y.UserId);

            try
            {
                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Getting a list of students by center ID by {User.Identity.Name}",
                    Type = "Get"
                });

                await _context.SaveChangesAsync();

                var response = from c in _context.Users.Where(x => userIds.Contains(x.Id))
                            select new
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                MiddleName = c.MiddleName,
                                LastName = c.LastName,
                                Avatar = c.Avatar,
                                Email = c.Email,
                                Password = c.Password,
                                RoleId = c.RoleId,
                                Role = c.Role.Description,
                                Birthday = c.Birthday,
                                Age = DateTime.Today.Year - c.Birthday.Year,
                                Phone = c.Phone,
                                IIN = c.IIN,
                                Address = c.Address,
                                WorkStatus = c.WorkStatus,
                                WorkExperience = c.WorkExperience,
                                Speciality = c.Speciality,
                                MaritalStatus = c.MaritalStatus,
                                HasChild = c.HasChild,
                                Created = c.Created,
                                LastEntry = c.LastEntry,
                                CheckNumber = c.CheckNumber,
                                CenterUsers = c.CenterUsers.Select(x => x.Center.Name).ToList(),
                                UserLanguages = c.UserLanguages.Select(x => x.Language.Name).ToList()
                            };

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        // Возвращает пользователя с помощью токена
        [EnableCors]
        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var response = await Task.Run(() => _context.Users
                .Include(user => user.UserLanguages)
                .Include(user => user.CenterUsers)
                .Where(x => x.Email == User.Identity.Name)
                .FirstOrDefaultAsync());

                await _context.SaveChangesAsync();

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Getting data has been failed!" });
            }
        }

        // Возвращает определенного пользователя с помощью параметра userId
        [EnableCors]
        [HttpGet]
        [Route("user/{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            try
            {
                var response = await Task.Run(() => _context.Users
                    .Include(user => user.UserLanguages)
                    .Include(user => user.CenterUsers)
                    .FirstOrDefaultAsync(x => x.Id == userId));

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
        public async Task<IActionResult> GetTrainers(int centerId)
        {
            try
            {
                var _list = _context.CenterUsers.Where(x => x.CenterId == centerId).Select(x => x.UserId).ToList();

                var response = _context.Users.Where(x => x.RoleId == 3 && _list.Contains(x.Id)).Select(x => new { x.Id, x.FirstName, x.LastName });

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
        public async Task<IActionResult> Post([FromBody] User model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            if (model.Email == null)
                return BadRequest(new { errorText = "Email is null" });

            // Валидация на бек энде
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            if (!_context.Users.Select(x => x.Email).ToList().Contains(model.Email))
            {
                string _newPass = new string(Enumerable.Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", 10).
                Select(s => s[GlobalVariables.RANDOM.
                Next(s.Length)]).
                ToArray());

                model.Password = _newPass;

                SendEmail(model);

                if (model.Password != null)
                    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                _context.Users.Add(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Posted a users data by {User.Identity.Name}",
                    Type = "Post"
                });

                await _context.SaveChangesAsync();

                return Ok(model);
            }

            else return BadRequest(new { errorText = "This Email is already used" });
        }

        [EnableCors]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody] User model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                // https://stackoverflow.com/questions/42993860/entity-framework-core-update-many-to-many
                _context.CenterUsers.
                RemoveRange(_context.CenterUsers.
                Where(x => x.UserId == new Guid(model.Id.ToString())).
                ToList());

                model.MiddleName = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).MiddleName;
                model.Avatar = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Avatar;
                model.Email = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Email;
                model.Password = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Password;
                model.Birthday = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Birthday;
                model.Phone = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Phone;
                model.IIN = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).IIN;
                model.Address = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Address;
                model.WorkStatus = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).WorkStatus;
                model.WorkExperience = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).WorkExperience;
                model.Speciality = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Speciality;
                model.MaritalStatus = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).MaritalStatus;
                model.HasChild = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).HasChild;
                model.Created = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).Created;
                model.LastEntry = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).LastEntry;
                model.CheckNumber = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == model.Id).CheckNumber;

                _context.Update(model);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a user datas by {User.Identity.Name}",
                    Type = "Patch"
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
        [HttpPatch]
        [Route("status")]
        public async Task<IActionResult> PatchStatus([FromBody] User model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                _context.Users.FirstOrDefault(x => x.Id == model.Id).WorkStatus = model.WorkStatus;

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a user datas by {User.Identity.Name}",
                    Type = "Patch"
                });

                await _context.SaveChangesAsync();

                var response = _context.Users.Find(model.Id);

                return Ok(response);
            }
            catch
            {
                return BadRequest(new { errorText = "Editing data has been failed!" });
            }
        }

        [EnableCors]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] User model)
        {
            if (model == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            try
            {
                var _oldPassword = _context.Users
                .AsNoTracking()
                .Where(x => x.Email == model.Email)
                .FirstOrDefault().Password;

                if (model.Password == null)
                    model.Password = _oldPassword;

                else if (model.Password.Length == 0)
                    model.Password = _oldPassword;

                else model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                _context.UserLanguages.
                    RemoveRange(_context.UserLanguages.
                    Where(x => x.UserId == new Guid(model.Id.ToString())).
                    ToList());

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Edited a user datas by {User.Identity.Name}",
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
        public async Task<IActionResult> Delete(Guid id)
        {
            if (_context.Users.FirstOrDefault(x => x.Id == id) == null)
                return NotFound();

            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Id == id);

                _context.Users.Remove(user);

                _context.Logs.Add(new Log()
                {
                    UserId = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id,
                    Description = $"Removed a users data by {User.Identity.Name}",
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

        private void SendEmail(User user)
        {
            var senderEmail = new MailAddress(configuration.GetSection("Smtp")["Email"], "");
            var EmailReceiver = new MailAddress(user.Email, "Пользователь");

            string _password = configuration.GetSection("Smtp")["Password"];
            string _sub = "Создание пользователя";
            string _body = string.Format("<h1>Доброго времени суток, {0}!</h1><br><hr>" +
                "<p>Ваша учетная запись успешно создана! </p>" +
                "<p>Ваш логин: <h1>{0}</h1></p>" +
                "<p>Ваш пароль: <h1>{1}</h1></p>" +
                "<p>Перейдите по ссылке <a href='https://some.com'>some.com</a></p>" +
                "<p>С уважением, Система учета управления!</p>", user.Email, user.Password);

            var smtp = new SmtpClient
            {
                Host = configuration.GetSection("Smtp")["Host"],
                Port = int.Parse(configuration.GetSection("Smtp")["Port"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, _password)
            };

            using (var message = new MailMessage(senderEmail, EmailReceiver))
            {
                message.Subject = _sub;
                message.Body = _body;
                message.IsBodyHtml = true;
                ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                smtp.Send(message);
            }
            smtp.Dispose();
        }
    }
}
