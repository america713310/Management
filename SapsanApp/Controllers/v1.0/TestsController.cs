using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SapsanApp.Models;
using SapsanApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SapsanApp.Controllers.v1._0
{
    // Не забыть убрать в accounts/reset убрать код на 123456
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TestsController : ControllerBase
    {
        private SapsanContext _context;
        private IConfiguration _configuration;
        private IWebHostEnvironment _appEnvironment;
        public TestsController(
            SapsanContext context
            )
        {
            _context = context;
        }

        /// <summary>
        ///  Test
        /// </summary>
        /// <returns></returns>
        [EnableCors()]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetDauren()
        {
            return Ok("Timur");
        }

        [EnableCors()]
        [HttpPost]
        public ActionResult Post()
        {
            // Post countries
            //List<CenterUser> _models = new List<CenterUser>();
            //_models.Add(new CenterUser() { UserId = new Guid("08d9818e-1365-4a5c-8fd6-c62f53e83d0a"), CenterId = 1 });
            //_models.Add(new CenterUser() { UserId = new Guid("08d9818e-1365-4a12-87c6-905a49651e94"), CenterId = 19 });
            //_models.Add(new CenterUser() { UserId = new Guid("08d9818e-1365-4a1e-8ab9-f2f775921cb2"), CenterId = 19 });
            //_models.Add(new CenterUser() { UserId = new Guid("08d9818e-1365-4a2e-82ce-d2c8657b095c"), CenterId = 19 });
            //_models.Add(new CenterUser() { UserId = new Guid("08d9818e-1365-4a3a-8a17-17716abd5b48"), CenterId = 18 });
            //_models.Add(new CenterUser() { UserId = new Guid("08d9818e-1365-4a46-8c7d-3505b07f6611"), CenterId = 16 });


            //_models.Add(new Center() { Name = "SAPSAN Essentai city", CountryId = 1, CityId = 1, Address = "Аль-Фараби 116/21" });
            //_models.Add(new Center() { Name = "Астана 1", CountryId = 1, CityId = 2, Address = "Кабанбай батыра 5/1" });
            //_models.Add(new Center() { Name = "Астана 2", CountryId = 1, CityId = 2, Address = "ул.Алихан Бокейханова дом 29 Б н.п 15" });
            //_models.Add(new Center() { Name = "Актобе 1", CountryId = 1, CityId = 11, Address = "г. Актобе, район Батыс 2, пр. А. Молдагуловой, д.57Д" });
            //_models.Add(new Center() { Name = "Актобе 2", CountryId = 1, CityId = 11, Address = "г. Актобе, Батыс 2, дом 3Г" });
            //_models.Add(new Center() { Name = "Актау 1", CountryId = 1, CityId = 12, Address = "16 мкр 33/7 дом ЖК каспий" });
            //_models.Add(new Center() { Name = "Семей 1", CountryId = 1, CityId = 8, Address = "ул. Уранхаева, 28" });
            //_models.Add(new Center() { Name = "Жезгазган 1", CountryId = 1, CityId = 15, Address = "ул. Есенберлина 3" });
            //_models.Add(new Center() { Name = "Костанай 1", CountryId = 1, CityId = 10, Address = "ул. 1Мая, д. 126" });
            //_models.Add(new Center() { Name = "Талдыкорган 1", CountryId = 1, CityId = 14, Address = "Ул. Биржан Сал 87" });
            //_models.Add(new Center() { Name = "Петропавловск 1", CountryId = 1, CityId = 25, Address = "улица Хименко, 11, каб. 7" });
            //_models.Add(new Center() { Name = "Кокшетау 1", CountryId = 1, CityId = 26, Address = "Момышулы 41, 211 каб" });
            //_models.Add(new Center() { Name = "Шымкент 1", CountryId = 1, CityId = 9, Address = "ул. Гани Иляева 39а, 2 этаж" });
            //_models.Add(new Center() { Name = "Ростов 1", CountryId = 2, CityId = 18, Address = "пр. Космонавтова, 32в/21в" });
            //_models.Add(new Center() { Name = "Тюмень 1", CountryId = 2, CityId = 17, Address = "улица Шиллера дом 46 корпус 3" });
            //_models.Add(new Center() { Name = "Волжский 1", CountryId = 2, CityId = 19, Address = "Бульвар Профсоюзов, 15" });
            //_models.Add(new Center() { Name = "Пермь 1", CountryId = 2, CityId = 23, Address = "Луначарского 3/2" });
            //_models.Add(new Center() { Name = "Уфа 1", CountryId = 2, CityId = 16, Address = "" });
            //_models.Add(new Center() { Name = "Бишкек 1", CountryId = 5, CityId = 20, Address = "ул.Боконбаева 98/Тыныстанов, 2 этаж" });
            //_models.Add(new Center() { Name = "Бишкек 2", CountryId = 5, CityId = 20, Address = "Саякбая Каралаева 64" });
            //_models.Add(new Center() { Name = "Караганда 1", CountryId = 1, CityId = 3, Address = "Мухамедхана Сейткулова ст 16а" });

            //_context.AddRange(_models);

            //_context.SaveChanges();

            return Ok(_context.CenterUsers);
        }
        [EnableCors()]
        [HttpPatch]
        public async Task<ActionResult> Update()
        {
            return Ok();
        }
        [EnableCors()]
        [HttpHead]
        public async Task<ActionResult> Update1()
        {
            return Ok();
        }
        [EnableCors()]
        [HttpOptions]
        public async Task<ActionResult> Update2()
        {
            return Ok();
        }
    }
}
