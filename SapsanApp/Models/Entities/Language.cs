using System.Collections.Generic;

namespace SapsanApp.Models.Entities
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Group> Groups { get; set; }
        public List<UserLanguage> UserLanguages { get; set; }
    }
}
