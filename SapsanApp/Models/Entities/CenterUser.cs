using System;

namespace SapsanApp.Models.Entities
{
    public class CenterUser
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
    }
}
