using Microsoft.EntityFrameworkCore;

namespace EmailSender
{
    public partial class EmailSenderDbContext : DbContext
    {
        public EmailSenderDbContext()
        {
        }

        public EmailSenderDbContext(DbContextOptions<EmailSenderDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EmailSender.Db;Trusted_Connection=True;");
            }
        }
    }
}
