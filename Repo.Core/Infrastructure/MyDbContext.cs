using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Models;
using Task = Repo.Core.Models.Task;
using User = Repo.Core.Models.User;
using Type = Repo.Core.Models.Type;
namespace Repo.Core.Infrastructure;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AbsenceDay> AbsenceDays { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Benefit> Benefits { get; set; }

    public virtual DbSet<BenefitType> BenefitTypes { get; set; }

    public virtual DbSet<Contribution> Contributions { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<HireHelper> HireHelpers { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RelatedTask> RelatedTasks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Target> Targets { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkTable> WorkTables { get; set; }

    public virtual DbSet<WorkTask> WorkTasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Tmp;User=sa;Password=Haslo1234*;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AbsenceDay>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("AbsenceDay_pk");

            entity.ToTable("AbsenceDay");

            entity.Property(e => e.ID).ValueGeneratedNever();

            entity.HasOne(d => d.WorkTable).WithMany(p => p.AbsenceDays)
                .HasForeignKey(d => d.WorkTable_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SickDay_WorkTable");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Application_pk");

            entity.ToTable("Application");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Template).HasColumnType("xml");

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Applications)
                .HasForeignKey(d => d.DocumentType_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Application_DocumentType");

            entity.HasMany(d => d.Users).WithMany(p => p.Applications)
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicationUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ApplicationUser_User"),
                    l => l.HasOne<Application>().WithMany()
                        .HasForeignKey("Application_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ApplicationUser_Application"),
                    j =>
                    {
                        j.HasKey("Application_ID", "User_ID").HasName("ApplicationUser_pk");
                        j.ToTable("ApplicationUser");
                    });
        });

        modelBuilder.Entity<Benefit>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Benefit_pk");

            entity.ToTable("Benefit");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Cost).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.BenefitType).WithMany(p => p.Benefits)
                .HasForeignKey(d => d.BenefitType_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Benefit_BenefitType");

            entity.HasMany(d => d.Users).WithMany(p => p.Benefits)
                .UsingEntity<Dictionary<string, object>>(
                    "BenefitUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("BenefitUser_User"),
                    l => l.HasOne<Benefit>().WithMany()
                        .HasForeignKey("Benefit_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("BenefitUser_Benefit"),
                    j =>
                    {
                        j.HasKey("Benefit_ID", "User_ID").HasName("BenefitUser_pk");
                        j.ToTable("BenefitUser");
                    });
        });

        modelBuilder.Entity<BenefitType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("BenefitType_pk");

            entity.ToTable("BenefitType");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<Contribution>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Contributions_pk");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Value).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Course_pk");

            entity.ToTable("Course");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasMany(d => d.Users).WithMany(p => p.Courses)
                .UsingEntity<Dictionary<string, object>>(
                    "CourseUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("CourseUser_User"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("Course_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("CourseUser_Course"),
                    j =>
                    {
                        j.HasKey("Course_ID", "User_ID").HasName("CourseUser_pk");
                        j.ToTable("CourseUser");
                    });
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Document_pk");

            entity.ToTable("Document");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Documents)
                .HasForeignKey(d => d.DocumentType_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Document_DocumentType");

            entity.HasOne(d => d.User).WithMany(p => p.Documents)
                .HasForeignKey(d => d.User_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Document_User");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("DocumentType_pk");

            entity.ToTable("DocumentType");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Grade_pk");

            entity.ToTable("Grade");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Grade1)
                .HasColumnType("decimal(2, 2)")
                .HasColumnName("Grade");

            entity.HasMany(d => d.Users).WithMany(p => p.Grades)
                .UsingEntity<Dictionary<string, object>>(
                    "GradeUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("GradeUser_User"),
                    l => l.HasOne<Grade>().WithMany()
                        .HasForeignKey("Grade_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("GradeUser_Grade"),
                    j =>
                    {
                        j.HasKey("Grade_ID", "User_ID").HasName("GradeUser_pk");
                        j.ToTable("GradeUser");
                    });
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Group_pk");

            entity.ToTable("Group");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasMany(d => d.Tasks).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupTask",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("Task_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("GroupTask_Task"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("Group_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("GroupTask_Group"),
                    j =>
                    {
                        j.HasKey("Group_ID", "Task_ID").HasName("GroupTask_pk");
                        j.ToTable("GroupTask");
                    });

            entity.HasMany(d => d.Users).WithMany(p => p.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("GroupUser_User"),
                    l => l.HasOne<Group>().WithMany()
                        .HasForeignKey("Group_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("GroupUser_Group"),
                    j =>
                    {
                        j.HasKey("Group_ID", "User_ID").HasName("GroupUser_pk");
                        j.ToTable("GroupUser");
                    });
        });

        modelBuilder.Entity<HireHelper>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("HireHelper_pk");

            entity.ToTable("HireHelper");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Hourly_Rate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Netto_Salary).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.HireHelpers)
                .HasForeignKey(d => d.User_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("HireHelper_User");

            entity.HasMany(d => d.Contributions).WithMany(p => p.HireHelpers)
                .UsingEntity<Dictionary<string, object>>(
                    "ContributionHire",
                    r => r.HasOne<Contribution>().WithMany()
                        .HasForeignKey("Contributions_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ContributionHire_Contributions"),
                    l => l.HasOne<HireHelper>().WithMany()
                        .HasForeignKey("HireHelper_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ContributionHire_HireHelper"),
                    j =>
                    {
                        j.HasKey("HireHelper_ID", "Contributions_ID").HasName("ContributionHire_pk");
                        j.ToTable("ContributionHire");
                    });
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Priority_pk");

            entity.ToTable("Priority");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Priority1)
                .HasMaxLength(100)
                .HasColumnName("Priority");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("RefreshToken_pk");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.RevokedAt).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.User_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RefreshToken_User");
        });

        modelBuilder.Entity<RelatedTask>(entity =>
        {
            entity.HasKey(e => new { e.Main_Task_ID, e.Related_Task_ID }).HasName("RelatedTasks_pk");

            entity.HasOne(d => d.Main_Task).WithMany(p => p.RelatedTaskMain_Tasks)
                .HasForeignKey(d => d.Main_Task_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RelatedTasks_MainTask");

            entity.HasOne(d => d.Related_Task).WithMany(p => p.RelatedTaskRelated_Tasks)
                .HasForeignKey(d => d.Related_Task_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RelatedTasks_RelatedTask");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Role_pk");

            entity.ToTable("Role");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Role_Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Skills_pk");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Type).WithMany(p => p.Skills)
                .HasForeignKey(d => d.Type_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Skills_Type");

            entity.HasMany(d => d.Users).WithMany(p => p.Skills)
                .UsingEntity<Dictionary<string, object>>(
                    "SkillUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("SkillUser_User"),
                    l => l.HasOne<Skill>().WithMany()
                        .HasForeignKey("Skills_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("SkillUser_Skills"),
                    j =>
                    {
                        j.HasKey("Skills_ID", "User_ID").HasName("SkillUser_pk");
                        j.ToTable("SkillUser");
                    });
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Status_pk");

            entity.ToTable("Status");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Status1)
                .HasMaxLength(100)
                .HasColumnName("Status");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Tag_pk");

            entity.ToTable("Tag");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(15);
        });

        modelBuilder.Entity<Target>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Target_pk");

            entity.ToTable("Target");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Finish_Time).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Start_Time).HasColumnType("datetime");

            entity.HasOne(d => d.Tag).WithMany(p => p.Targets)
                .HasForeignKey(d => d.Tag_ID)
                .HasConstraintName("Target_Tag");

            entity.HasOne(d => d.User).WithMany(p => p.Targets)
                .HasForeignKey(d => d.User_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Target_User");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Task_pk");

            entity.ToTable("Task");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Start_Time).HasColumnType("datetime");

            entity.HasOne(d => d.Priority).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Priority_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Task_Priority");

            entity.HasOne(d => d.Status).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Status_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Task_Status");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("Type_pk");

            entity.ToTable("Type");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Type1)
                .HasMaxLength(100)
                .HasColumnName("Type");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("User_pk");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Login).HasMaxLength(15);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Nickname).HasMaxLength(20);
            entity.Property(e => e.Password).HasMaxLength(300);
            entity.Property(e => e.Salt)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.Surname).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("Role_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("UserRole_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("UserRole_User"),
                    j =>
                    {
                        j.HasKey("User_ID", "Role_ID").HasName("UserRole_pk");
                        j.ToTable("UserRole");
                    });

            entity.HasMany(d => d.Tasks).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserTask",
                    r => r.HasOne<Task>().WithMany()
                        .HasForeignKey("Task_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("UserTask_Task"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User_ID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("UserTask_User"),
                    j =>
                    {
                        j.HasKey("User_ID", "Task_ID").HasName("UserTask_pk");
                        j.ToTable("UserTask");
                    });
        });

        modelBuilder.Entity<WorkTable>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("WorkTable_pk");

            entity.ToTable("WorkTable");

            entity.Property(e => e.ID).ValueGeneratedNever();
            entity.Property(e => e.Account_Number).HasMaxLength(28);
            entity.Property(e => e.Hourly_Rate).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.WorkTables)
                .HasForeignKey(d => d.User_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("WorkTable_User");
        });

        modelBuilder.Entity<WorkTask>(entity =>
        {
            entity.HasKey(e => new { e.WorkTable_ID, e.Task_ID }).HasName("WorkTask_pk");

            entity.ToTable("WorkTask");

            entity.HasOne(d => d.Task).WithMany(p => p.WorkTasks)
                .HasForeignKey(d => d.Task_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("WorkTask_Task");

            entity.HasOne(d => d.WorkTable).WithMany(p => p.WorkTasks)
                .HasForeignKey(d => d.WorkTable_ID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("WorkTask_WorkTable");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
