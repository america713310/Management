using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SapsanApp.Models.Entities
{
    public class Student
    {
        [Key]
        public Guid Id { get; set; }
        public string LoginId { get; set; } = string.Empty;
        public Guid? LeadId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public string Address { get; set; } = string.Empty;
        public int? CenterId { get; set; }
        public Center Center { get; set; }
        public int? SourceId { get; set; }
        public Source Source { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public double Balance { get; set; } = 0;
        public string Grade { get; set; } = string.Empty;
        public string ParentOneFirstName { get; set; } = string.Empty;
        public string ParentOneLastName { get; set; } = string.Empty;
        public string ParentOnePhone { get; set; } = string.Empty;
        public string ParentOneEmail { get; set; } = string.Empty;
        public string ParentTwoFirstName { get; set; } = string.Empty;
        public string ParentTwoLastName { get; set; } = string.Empty;
        public string ParentTwoPhone { get; set; } = string.Empty;
        public string ParentTwoEmail { get; set; } = string.Empty;
        public bool IsStudent { get; set; } = false;
        public DateTime Created { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public List<StudentGroup> StudentGroups { get; set; }
    }
}
