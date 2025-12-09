using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }
    public DbSet<EducationLevel> EducationLevels { get; set; }
    public DbSet<ProfessionalProfile> ProfessionalProfiles { get; set; }
    public DbSet<Email> Emails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Document)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(e => e.BirthDate)
                .HasColumnType("timestamp without time zone");
            
            entity.Property(e => e.HireDate)
                .HasColumnType("timestamp without time zone");
            
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW()");
            
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone");

        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(d => d.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW()");
            
            entity.Property(d => d.ModifiedAt)
                .HasColumnType("timestamp without time zone");
        });
        
        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(j => j.Id);
            
            entity.Property(j => j.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
        
        modelBuilder.Entity<EducationLevel>(entity =>
        {
            entity.HasKey(el => el.Id);
            
            entity.Property(el => el.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
        
        modelBuilder.Entity<ProfessionalProfile>(entity =>
        {
            entity.HasKey(pp => pp.Id);
            
            entity.Property(pp => pp.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
        
        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.From).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}