using ELTest.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ELTask> ELTasks { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ELTask>().ToTable("ELTasks");
            modelBuilder.Entity<ELTask>().HasOne(t => t.ActivityType).WithMany(a => a.ELTasks).HasForeignKey(t => t.ActivityTypeID); ;
        }
    }
}
