using Microsoft.EntityFrameworkCore;
using SapsanApp.Models.Entities;

namespace SapsanApp.Models
{
    public class SapsanContext : DbContext
    {
        public DbSet<AdminCriterion> AdminCriteria { get; set; }
        public DbSet<CashboxCenter> CashboxCenters { get; set; }
        public DbSet<CashboxProgram> CashboxPrograms { get; set; }
        public DbSet<CashboxRetail> CashboxRetails { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<CenterUser> CenterUsers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<MainCriterion> MainCriteria { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<ReferenceBook> ReferenceBooks { get; set; }
        public DbSet<Retail> Retails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<SubCriterion> SubCriteria { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<TrainingMaterial> TrainingMaterials { get; set; }
        public DbSet<TrainingProgram> TrainingPrograms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLanguage> UserLanguages { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public SapsanContext(DbContextOptions<SapsanContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CenterUser>()
                .HasKey(bc => new { bc.UserId, bc.CenterId });
            modelBuilder.Entity<CenterUser>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.CenterUsers)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<CenterUser>()
                .HasOne(bc => bc.Center)
                .WithMany(c => c.CenterUsers)
                .HasForeignKey(bc => bc.CenterId);

            modelBuilder.Entity<UserLanguage>()
                .HasKey(bc => new { bc.UserId, bc.LanguageId });
            modelBuilder.Entity<UserLanguage>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserLanguages)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserLanguage>()
                .HasOne(bc => bc.Language)
                .WithMany(c => c.UserLanguages)
                .HasForeignKey(bc => bc.LanguageId);

            modelBuilder.Entity<StudentGroup>()
                .HasKey(bc => new { bc.StudentId, bc.GroupId });
            modelBuilder.Entity<StudentGroup>()
                .HasOne(bc => bc.Student)
                .WithMany(b => b.StudentGroups)
                .HasForeignKey(bc => bc.StudentId);
            modelBuilder.Entity<StudentGroup>()
                .HasOne(bc => bc.Group)
                .WithMany(c => c.StudentGroups)
                .HasForeignKey(bc => bc.GroupId);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Supervizor", Description = "Супер Админ" },
                new Role { Id = 2, Name = "Administrator", Description = "Управленец" },
                new Role { Id = 3, Name = "Center Administrator", Description = "Администратор центра" },
                new Role { Id = 4, Name = "Partner", Description = "Партнер" },
                new Role { Id = 5, Name = "Trainer", Description = "Тренер/Преподаватель" },
                new Role { Id = 6, Name = "Tech", Description = "Тех. поддержка" });

            modelBuilder.Entity<Language>().HasData(
                new Language { Id = 1, Name = "Русский" },
                new Language { Id = 2, Name = "English" },
                new Language { Id = 3, Name = "Qazaq" },
                new Language { Id = 4, Name = "العربية" },
                new Language { Id = 5, Name = "Deutsche" },
                new Language { Id = 6, Name = "Le français" },
                new Language { Id = 7, Name = "Türkçe" },
                new Language { Id = 8, Name = "Română" },
                new Language { Id = 9, Name = "中国的" },
                new Language { Id = 10, Name = "čeština" });

            modelBuilder.Entity<Country>().HasData(
              new Country { Id = 1, Name = "Казахстан" },
              new Country { Id = 2, Name = "Российская Федерация" });

            modelBuilder.Entity<City>().HasData(
              new City { Id = 1, Name = "Алматы", CountryId = 1 },
              new City { Id = 2, Name = "Нурсултан", CountryId = 1 },
              new City { Id = 3, Name = "Караганда", CountryId = 1 },
              new City { Id = 4, Name = "Москва", CountryId = 2 });
        }
    }
}
