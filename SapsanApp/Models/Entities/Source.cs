using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Lead> Leads { get; set; }
        public List<Student> Students { get; set; }
    }
}
