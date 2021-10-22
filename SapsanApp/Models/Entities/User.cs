using SapsanApp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SapsanApp.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime Birthday { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public string Phone { get; set; } = string.Empty;
        public string IIN { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public WorkStatusEnum? WorkStatus { get; set; } = WorkStatusEnum.NoWork;
        public int? WorkExperience { get; set; } = 0;
        public string Speciality { get; set; } = string.Empty;
        public MaritalStatusEnum? MaritalStatus { get; set; } = MaritalStatusEnum.NotMarried; // по умолчанию Холост/Не замужем
        public bool? HasChild { get; set; } = false;
        public DateTime? LastEntry { get; set; } = DateTime.MinValue;
        public string CheckNumber { get; set; } = string.Empty;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public List<CenterUser> CenterUsers { get; set; }
        public List<UserLanguage> UserLanguages { get; set; }
    }
}
