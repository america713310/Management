using System;
using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class Center
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        // Номера телефонов центра
        public string Phone { get; set; } = string.Empty;
        // Ссылка на социальную сеть
        public string Social { get; set; } = string.Empty;
        public double? Royalti { get; set; } = 0;
        // Паушальный взнос
        public int? Lump { get; set; } = 0;
        // Площадь центра
        public string Square { get; set; } = string.Empty;
        // Платеж
        public int? Revenue { get; set; } = 0;
        // Задолженность
        public int? Debt { get; set; } = 0;
        // Посещаемость
        public int? Visits { get; set; } = 0;
        // Оплаченные
        public int? Paid { get; set; } = 0;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public List<Lead> Leads { get; set; }
        public List<Group> Groups { get; set; }
        public List<Schedule> Schedules { get; set; }
        public List<Student> Students { get; set; }
        public List<CenterUser> CenterUsers { get; set; }
    }
}
