namespace SapsanApp.Models.DTOs
{
    public class NotificationsDTO
    {
        public int[] Roles { get; set; }
        public int[] Centers { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
