using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SapsanApp.Helpers;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    // Апи аккаунта
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly SapsanContext _context;
        private readonly IConfiguration _configuration;
        public AccountsController(
            SapsanContext context,
            IConfiguration configuration
            )
        {
            _context = context;
            _configuration = configuration;
        }

        [EnableCors()]
        [HttpGet]
        [Authorize]
        [Route("token")]
        public async Task<IActionResult> Token()
        {
            if (_context.Users.FirstOrDefault(x => x.Email == User.Identity.Name) == null)
                return BadRequest(new { errorText = "Invalid token" });

            var role = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value).FirstOrDefault();

            var response = await Task.Run(() => new
            {
                Id = _context.Users.FirstOrDefault(j => j.Email == User.Identity.Name).Id,
                Email = User.Identity.Name,
                Role = role,
            });

            return Ok(response);
        }

        [EnableCors()]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { errorText = "Invalid input data!" });

            var identity = GetIdentity(user.Email, user.Password);

            if (identity == null)
                return BadRequest(new { errorText = "Invalid Email or Password!" });

            // Создание JWT-токена
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: DateTime.Now,
                    claims: identity.Claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = await Task.Run(() => new
            {
                Token = encodedJwt,
                UserId = _context.Users.FirstOrDefault(x => x.Email == user.Email).Id,
                Email = identity.Name,
                Role = identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault(),
                Centers = _context.Centers
                    .Where(x => _context.CenterUsers
                    .Where(x => x.UserId == _context.Users
                    .FirstOrDefault(x => x.Email == user.Email).Id)
                    .Select(x => x.CenterId)
                    .Contains(x.Id))
                    .Select(x => new { x.Id, x.Name })
            });

            _context.Users.FirstOrDefault(x => x.Email == user.Email).LastEntry = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            _context.Logs.Add(new Log() 
            { 
                UserId = response.UserId, 
                Description = $"Logged in as {response.Email}",
                Type = "Post"
            });

            _context.Logs.RemoveRange(_context.Logs.Where(x => x.Created < DateTime.Now.AddMonths(-6)));

            await _context.SaveChangesAsync();

            return Ok(response);
        }
        private ClaimsIdentity GetIdentity(string email, string password)
        {
            // Проверка на наличие пользователя с такой почтой
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, _context.Roles.FirstOrDefault(x => x.Id == user.RoleId).Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }

        [EnableCors()]
        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> Reset([FromBody] User user)
        {
            // Генерация нового пароля
            string _newPass = new string(Enumerable.Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", 10)
                .Select(s => s[GlobalVariables.RANDOM
                .Next(s.Length)])
                .ToArray());

            var senderEmail = new MailAddress(_configuration.GetSection("Smtp")["Email"], "");
            var EmailReceiver = new MailAddress(user.Email, "Пользователь");
            string _password = _configuration.GetSection("Smtp")["Password"];
            string _sub = "Сброс пароля";
            string _body = string.Format("<h1>Доброго времени суток!</h1><br><hr>" +
                "<p>Так как вы забыли пароль, ваш пароль был сброшен и был сгенерирован новый: <h1>{0}</h1></p>" +
                "<p>С уважением, Система учета управления!</p>", _newPass);

            _context.Users.FirstOrDefault(x => x.Email == user.Email).Password = BCrypt.Net.BCrypt.HashPassword(_newPass);

            await _context.SaveChangesAsync();

            var smtp = new SmtpClient
            {
                Host = _configuration.GetSection("Smtp")["Host"],
                Port = int.Parse(_configuration.GetSection("Smtp")["Port"]),
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

            return Ok("The Password has been reseted!");
        }
    }
}
