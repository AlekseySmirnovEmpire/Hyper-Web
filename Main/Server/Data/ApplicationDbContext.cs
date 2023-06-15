using Microsoft.EntityFrameworkCore;
using Server.Data.Email;
using Server.Data.User;

namespace Server.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }

    public DbSet<UserRole> Roles { get; set; }
    
    public DbSet<EmailSendingQueue> EmailSendingQueue { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>(b =>
        {
            // Each User can have many entries in the UserRole join table
            b.HasOne(e => e.Role)
                .WithMany(e => e.Users)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired();
        });

        modelBuilder.Entity<UserRole>(b =>
        {
            // Each Role can have many entries in the UserRole join table
            b.HasMany(e => e.Users)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired();
        });

        modelBuilder.Entity<EmailSendingQueue>()
            .HasKey(esq => esq.Id);
    }
}