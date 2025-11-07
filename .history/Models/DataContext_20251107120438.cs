using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LuyenThiTracNghiem.Areas.Admin.Models;
namespace LuyenThiTracNghiem.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<tblUser> Users { get; set; }
        public DbSet<tblAdminMenu> AdminMenus { get; set; }
        public DbSet<tblSubject> Subjects { get; set; }
        public DbSet<tblExam> Exams { get; set; }
        public DbSet<tblQuestion> Questions { get; set; }
        public DbSet<tblAnswer> Answers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblQuestion>()
                .Property(q => q.Level)
                .HasConversion<byte>();
        }
        public DbSet<tblQuestionInExam> QuestionInExams { get; set; }
        public DbSet<tblPayment> Payments { get; set; }
        public db
    }
}