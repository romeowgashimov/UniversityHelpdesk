using Microsoft.EntityFrameworkCore;
using Universityhelpdesk.Domain.Entities;
using Universityhelpdesk.Domain.Enums;
using Universityhelpdesk.Domain.Statics;

namespace Universityhelpdesk.Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasOne(t => t.Applicant)
                .WithMany() 
                .HasForeignKey(t => t.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(t => t.Technician)
                .WithMany()
                .HasForeignKey(t => t.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ticket>()
                    .Property(t => t.CreatedAt)
                    .HasColumnType("timestamp with time zone");

        var users = new User[5];
        users[0] = new User
        {
            Id = 1,
            FullName = "Иванов Иван",
            Email = "student@univ.ru",
            Role = UserRole.Applicant,
            Login = "ivan",
            PasswordHash = HashProvider.HashPassword("111111")
        };
        users[1] = new User
        { 
            Id = 2,
            FullName = "Петров Пётр",
            Email = "dispatcher@univ.ru",
            Role = UserRole.Dispatcher,
            Login = "petr",
            PasswordHash = HashProvider.HashPassword("222222")
        };
        users[2] = new User 
        { 
            Id = 3,
            FullName = "Сидоров Сидор",
            Email = "technician@univ.ru",
            Role = UserRole.Technician,
            Login = "pidor",
            PasswordHash = HashProvider.HashPassword("333333")
        };
        users[3] = new User 
        { 
            Id = 4,
            FullName = "Романов Роман",
            Email = "roman@univ.ru",
            Role = UserRole.Technician,
            Login = "roman",
            PasswordHash = HashProvider.HashPassword("444444")
        };
        users[4] = new User 
        { 
            Id = 5,
            FullName = "Даниилов Данилл",
            Email = "danilo@univ.ru",
            Role = UserRole.Technician,
            Login = "danilo",
            PasswordHash = HashProvider.HashPassword("555555")
        };

        modelBuilder.Entity<User>().HasData(users);            
    }
}