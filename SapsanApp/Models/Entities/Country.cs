using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<City> Cities { get; set; }
    }
}
