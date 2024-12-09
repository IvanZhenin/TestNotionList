using Microsoft.EntityFrameworkCore;
using Notion.DataAccess.Models;

namespace Notion.DataAccess.Data
{
    public class NotionDb : DbContext
    {
        public NotionDb(DbContextOptions<NotionDb> options) 
            : base(options) 
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserNotion> UserNotions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.Id);
            });

            modelBuilder.Entity<UserNotion>(notion =>
            {
                notion.HasKey(n => n.Id);
                notion.HasOne(n => n.User).WithMany(u => u.UserNotions).HasForeignKey(n => n.UserId);
            });
        }
    }
}