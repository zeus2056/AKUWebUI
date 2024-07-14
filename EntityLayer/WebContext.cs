using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer
{
    public class WebContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public DbSet<Branch> Branches
        {
            get; set;
        }
        public DbSet<Permission> Permissions
        {
            get; set;
        }
        public DbSet<FrontPayment> FrontPayments
        {
            get; set;
        }
        public DbSet<Bank> Banks
        {
            get; set;
        }
        public DbSet<Expens> Expenses
        {
            get; set;
        }
        public DbSet<HistoryPayment> HistoryPayments
        {
            get; set;
        }
        public DbSet<RatePaymentInfo> RatePaymentInfos
        {
            get; set;
        }
        public DbSet<ExamStudent> ExamStudents
        {
            get; set;
        }
        public DbSet<Payment> Payments
        {
            get; set;
        }
        public DbSet<StaffPermission> StaffPermissions
        {
            get; set;
        }
        public DbSet<PaymentType> PaymentTypes
        {
            get; set;
        }
        public DbSet<DiscountRate> DiscountRates
        {
            get; set;
        }
        public DbSet<Exam> Exams
        {
            get; set;
        }
        public DbSet<ExamResult> ExamResults
        {
            get; set;
        }
        public DbSet<Student> Students
        {
            get; set;
        }
        public DbSet<Parent> Parents
        {
            get; set;
        }
        public DbSet<AgeGroup> AgeGroups
        {
            get; set;
        }
        public DbSet<RateStudent> RateStudents
        {
            get; set;
        }
        public DbSet<Rate> Rates
        {
            get; set;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-4O3O8J5;Initial Catalog=DenemeWeBuıDb;Integrated Security = true;TrustServerCertificate=TRUE");
        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<Branch>().HasMany(b => b.Students).WithOne(s => s.Branch).HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<Exam>().HasOne(b => b.RateStudent).WithOne(s => s.Exam).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<FrontPayment>().HasOne(b => b.RateStudent).WithMany(s => s.FrontPayments).HasForeignKey(s => s.RateStudentId).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<Payment>().HasOne(b => b.RateStudent).WithMany(s => s.Payments).HasForeignKey(s => s.RateStudentId).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<RatePaymentInfo>().HasOne(b => b.RateStudent).WithMany(s => s.RatePaymentInfos).HasForeignKey(s => s.RateStudentId).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<ExamResult>().HasOne(b => b.Rate).WithMany(s => s.ExamResults).HasForeignKey(s => s.ExamResultId).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<Exam>().HasOne(b => b.Student).WithMany(s => s.Exams).HasForeignKey(s => s.ExamId).OnDelete(DeleteBehavior.NoAction);
        //    builder.Entity<IdentityUserLogin<int>>().HasNoKey();
        //    builder.Entity<IdentityUserRole<int>>().HasNoKey();
        //    builder.Entity<IdentityUserToken<int>>().HasNoKey();
        //}
    }
}
