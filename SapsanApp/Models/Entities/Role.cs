using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Pages { get; set; } = string.Empty;

        public List<User> Users { get; set; }
    }
}
