using Microsoft.EntityFrameworkCore;

namespace MaybeExam
{
    public class MaybeDbContext : DbContext
    {
        static readonly string connectionString =
            "Server=db4free.net; User ID=mikhail; Password=S8nluvzu; Database=mikhail_db";
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}
