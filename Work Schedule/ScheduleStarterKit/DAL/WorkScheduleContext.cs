namespace ScheduleStarterKit.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using ScheduleStarterKit.Entities;

    internal partial class WorkScheduleContext : DbContext
    {
        public WorkScheduleContext()
            : base("name=WSdb")
        {
        }

        public virtual DbSet<ContractSkill> ContractSkills { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<PlacementContract> PlacementContracts { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.HomePhone)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeSkills)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Schedules)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EmployeeSkill>()
                .Property(e => e.HourlyWage)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Location>()
                .Property(e => e.Province)
                .IsFixedLength();

            modelBuilder.Entity<Location>()
                .Property(e => e.Phone)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.PlacementContracts)
                .WithRequired(e => e.Location)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlacementContract>()
                .HasMany(e => e.ContractSkills)
                .WithRequired(e => e.PlacementContract)
                .HasForeignKey(e => e.ContractID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlacementContract>()
                .HasMany(e => e.Shifts)
                .WithRequired(e => e.PlacementContract)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Schedule>()
                .Property(e => e.HourlyWage)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Shift>()
                .HasMany(e => e.Schedules)
                .WithRequired(e => e.Shift)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.ContractSkills)
                .WithRequired(e => e.Skill)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Skill>()
                .HasMany(e => e.EmployeeSkills)
                .WithRequired(e => e.Skill)
                .WillCascadeOnDelete(false);
        }
    }
}
