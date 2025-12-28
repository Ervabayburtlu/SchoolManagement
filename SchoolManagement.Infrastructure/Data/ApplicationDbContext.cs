using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;

namespace SchoolManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Advisor> Advisors { get; set; }
    public DbSet<Academician> Academicians { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<StudentExam> StudentExams { get; set; }
    public DbSet<StudentSubject> StudentSubjects { get; set; }
    public DbSet<Excuse> Excuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student Configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentNo);
            entity.HasIndex(e => e.StudentMail).IsUnique();
            
            entity.HasOne(s => s.Advisor)
                .WithMany(a => a.Students)
                .HasForeignKey(s => s.AdvisorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Advisor Configuration
        modelBuilder.Entity<Advisor>(entity =>
        {
            entity.HasKey(e => e.AdvisorId);
            entity.HasIndex(e => e.AdvisorMail).IsUnique();
        });

        // Academician Configuration
        modelBuilder.Entity<Academician>(entity =>
        {
            entity.HasKey(e => e.AcademicianId);
            entity.HasIndex(e => e.AcademicianEmail).IsUnique();
        });

        // Subject Configuration
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId);
            
            entity.HasOne(s => s.Academician)
                .WithMany(a => a.Subjects)
                .HasForeignKey(s => s.AcademicianId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Exam Configuration
        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId);
            
            entity.HasOne(e => e.Subject)
                .WithMany(s => s.Exams)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // StudentExam Configuration
        modelBuilder.Entity<StudentExam>(entity =>
        {
            entity.HasKey(e => e.StudentNoExamId);
            
            entity.HasOne(se => se.Student)
                .WithMany(s => s.StudentExams)
                .HasForeignKey(se => se.StudentNo)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(se => se.Exam)
                .WithMany(e => e.StudentExams)
                .HasForeignKey(se => se.ExamId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // StudentSubject Configuration
        modelBuilder.Entity<StudentSubject>(entity =>
        {
            entity.HasKey(e => e.StudentNoSubjectId);
            
            entity.HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.StudentNo)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Excuse Configuration
        modelBuilder.Entity<Excuse>(entity =>
        {
            entity.HasKey(e => e.ExcuseId);
            
            entity.HasOne(e => e.Student)
                .WithMany(s => s.Excuses)
                .HasForeignKey(e => e.StudentNo)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Advisor)
                .WithMany()
                .HasForeignKey(e => e.AdvisorId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}

