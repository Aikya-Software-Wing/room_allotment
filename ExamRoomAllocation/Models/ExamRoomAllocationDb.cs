namespace ExamRoomAllocation.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ExamRoomAllocationDb : DbContext
    {
        public ExamRoomAllocationDb()
            : base("name=ExamRoomAllocationDb")
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Stream> Streams { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>()
                .HasMany(e => e.Exams)
                .WithOptional(e => e.Department)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Students)
                .WithOptional(e => e.Department)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Teachers)
                .WithOptional(e => e.Department)
                .HasForeignKey(e => e.Department_Id);

            modelBuilder.Entity<Designation>()
                .HasMany(e => e.Teachers)
                .WithOptional(e => e.Designation)
                .HasForeignKey(e => e.Designation_Id)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Exam>()
                .HasMany(e => e.Rooms)
                .WithMany(e => e.Exams)
                .Map(m => m.ToTable("ExamInRoom").MapLeftKey("Code").MapRightKey("Id"));

            modelBuilder.Entity<Exam>()
                .HasMany(e => e.Students)
                .WithMany(e => e.Exams)
                .Map(m => m.ToTable("StudentWriteExam").MapLeftKey("Code").MapRightKey("USN"));

            modelBuilder.Entity<Exam>()
                .HasMany(e => e.Teachers)
                .WithMany(e => e.Exams)
                .Map(m => m.ToTable("TeacherSupervisesExam").MapLeftKey("Code").MapRightKey("Id"));

            modelBuilder.Entity<Stream>()
                .HasMany(e => e.Departments)
                .WithOptional(e => e.Stream)
                .HasForeignKey(e => e.Stream_Id)
                .WillCascadeOnDelete();
        }
    }
}
